using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


/// <summary>
/// The different states the cat can be in.  The program will check its state when updating.
/// </summary>
public enum CatState {
    NONE,
    ROAMING,
    CURIOUS,
    DISTRESS,
    DEAD
}

/// <summary>
/// MonoBehavior controlling cat behavior
/// </summary>
public class CatAI : MonoBehaviour {
    /// <summary>
    /// Level of interest at which to trigger a cat approaching a hazard
    /// </summary>
    private static int INTEREST_TRIGGER = 95;

    //used for calculating bounds of curiosity, a stat that controls when a cat will enter danger
    private static int CURIOSITY_MIN = 0;
    private static int CURIOSITY_MAX = 100;
    //Amount to increase curiosity by every _decisiontime
    private static int CURIOSITY_INCREASE_AMT = 10;
    //How often to meow when approaching danger
    private static float CURIOSITY_MEOW_MIN_TIME = 2;
    private static float CURIOSITY_MEOW_MAX_TIME = 5;

    //Time between meows while in distress
    private static float DISTRESSED_MEOW_TIME = 1;

    /// <summary>
    /// Random cat sound effects
    /// </summary>
    public List<AudioClip> Meows;

    /// <summary>
    /// Prefab to play death effects
    /// </summary>
    public CatDeathController DeathPrefab;

    /// <summary>
    /// Particle system that is enabled when the cat is curious
    /// </summary>
    public ParticleSystem curiosityParticles;

    /// <summary>
    /// The initial color for this cat (defaults to white)
    /// </summary>
    public Color initialColor = Color.white;

    /// <summary>
    /// The distress color for this cat (Defaults to red)
    /// </summary>
    public Color distressColor = Color.red;

    /// <summary>
    /// The spawner for the cat prefabs.
    /// </summary>
    private CatSpawner catSpawner;

    /// <summary>
    /// Current state of the cat
    /// </summary>
    private CatState currentState = CatState.NONE;

    /// <summary>
    /// Random locations ofr the cat to wander
    /// </summary>
    private List<Transform> randomPositions = new List<Transform>();

    /// <summary>
    /// All hazards in the level
    /// </summary>
    private List<Hazard> hazards = new();

    /// <summary>
    /// Current target hazard
    /// </summary>
    public Hazard goalHazard;

    /// <summary>
    /// Time between making decisions
    /// </summary>
    private float decisionTime = 15f;

    /// <summary>
    /// Time to stay alive after triggering trap
    /// </summary>
    private float distressTime = 5f;

    /// <summary>
    /// The cat's curiousity levels.  This will increase over time.
    /// </summary>
    private float curiosity = 10f;

    /// <summary>
    /// Navmesh agent this cat is currently using
    /// </summary>
    public NavMeshAgent agent;

    /// <summary>
    /// Scalar indicating how close this cat is to death, 0 = healthy, 1 = dead.
    /// </summary>
    private float _distressLevel = 0;

    private MultiBillboard _billboardController;

    private void Awake() {
        //Generate NavMesh.
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Set the spawner associated with this cat
    /// </summary>
    /// <param name="catSpawner"></param>
    public void setSpawner(CatSpawner catSpawner) {
        this.catSpawner = catSpawner;
    }

    /// <summary>
    /// Set the objects the cat interacts with
    /// </summary>
    /// <param name="randomPositions">Safe positions to wander to</param>
    /// <param name="hazards">List of hazards</param>
    public void setPositions(List<Transform> randomPositions, List<Hazard> hazards) {
        this.randomPositions = randomPositions;
        this.hazards = hazards;
    }

    /// <summary>
    /// Set default state.  Once this calls, the AI loop begins!
    /// </summary>
    private void Start() {
        setState(CatState.ROAMING);
        _billboardController = GetComponentInChildren<MultiBillboard>();
    }

    private void Update() {
        //Set the color of the cat based on distress level
        _billboardController.block.SetColor(
            "_Color", Color.Lerp(initialColor, distressColor, _distressLevel)
            );
    }

    /// <summary>
    /// Triggered when a cat dies
    /// </summary>
    private void Die() {
        StopAllCoroutines();
        //Instantiate death effects
        Instantiate(DeathPrefab, transform.position, Quaternion.identity);
        //If the cat dies, de-occupy the hazard and destroy the cat prefab.
        Debug.Log("A cat has fallen.");
        goalHazard.occupied = false;
        goalHazard.trapped = false;
        Destroy(this.gameObject);
        HUDscript.Instance.lives -= 1;
        HUDscript.Instance.freeMeter = 0;
    }
    /// <summary>
    /// Trigger a random meow sound
    /// </summary>
    private void Meow() {
        var source = GetComponent<AudioSource>();
        source.clip = Meows[Random.Range(0, Meows.Count)];
        source.Play();
    }

    /// <summary>
    /// This switch statement is called whenever states change around.  Very handy.
    /// </summary>
    /// <param name="newState"></param>
    public void setState(CatState newState) {

        //If the state isn't changing, don't do anything.
        if (currentState == newState)
            return;

        //Cancel previous coroutines generated by previous state, and assign new state.
        StopAllCoroutines();
        currentState = newState;

        //Trigger appropriate behavior
        switch (currentState) {
            case CatState.ROAMING:
                StartCoroutine(roam());
                break;
            case CatState.CURIOUS:
                StartCoroutine(ApproachHazard());
                break;
            case CatState.DISTRESS:
                StartCoroutine(CatInTrouble());
                break;
            case CatState.DEAD:
                Die();
                break;
        }
    }

    /// <summary>
    /// Return a currently-available hazard. Returns null if none are free.
    /// </summary>
    /// <returns>A free </returns>
    private Hazard GetFreeHazard() {
        //Counter to prevent deadlock
        var iter = 0;
        var hazardBag = new List<Hazard>(hazards);
        while (iter < hazards.Count) {
            //Grab a random hazard from the bag
            var checkedHazard = hazardBag[Random.Range(0, hazards.Count)];
            hazardBag.Remove(checkedHazard);
            //Return if if it's free
            if (!checkedHazard.occupied)
                return checkedHazard;
            iter++;
        }

        Debug.LogWarning("Tried to find an open trap... but couldn't.");
        return null;
    }

    //Roaming Script.
    private IEnumerator roam() {
        while (currentState == CatState.ROAMING) {
            //Pick a random number between the cat's current curiosity level and 100.
            //If the number is above the threshold, the cat enters its curious state.
            //Ensures that, eventually, the cat's curiosity will overwhelm it.
            if (Random.Range(curiosity, CURIOSITY_MAX) >= INTEREST_TRIGGER) {
                goalHazard = GetFreeHazard();
                if (goalHazard == null) {
                    //Could not find a free hazard
                    yield return null;
                }
                else {
                    //no longer roaming
                    setState(CatState.CURIOUS);
                    yield return null;
                }
            }
            //Otherwise, the cat's curiosity grows.
            else {
                chooseRandomTarget();
                curiosity = Mathf.Clamp(curiosity + 10, CURIOSITY_MIN, CURIOSITY_MAX);
            }

            //Make a new decision in N seconds
            yield return new WaitForSeconds(decisionTime);
        }
    }

    //Utility function to make the cat walk to a "random" postion.
    private void chooseRandomTarget() {
        if (randomPositions.Count == 0) return;

        Debug.Log("choosing a random position. Curiosity: " + curiosity);
        var target = randomPositions[Random.Range(0, randomPositions.Count)];
        agent.destination = target.position;
    }

    //Utility function to make the cat walk to a hazard.
    private IEnumerator ApproachHazard() {
        curiosityParticles.gameObject.SetActive(true);
        Debug.Log("I have chosen death. Curiosity: " + curiosity);
        //Claim the hazard
        goalHazard.occupied = true;
        //Targets a random hazard to go "investigate."
        agent.destination = goalHazard.transform.position;

        while (
            currentState == CatState.CURIOUS ||
            Vector3.Distance(transform.position, goalHazard.transform.position) > agent.stoppingDistance
        ) {
            //Meow randomly while approaching danger
            yield return new WaitForSeconds(Random.Range(CURIOSITY_MEOW_MIN_TIME, CURIOSITY_MEOW_MAX_TIME));
            Meow();
        }
        //setState(CatState.DISTRESS);
    }

   
    //Utility function that waits for nine seconds, then changes the cat's
    // state to DEAD
    private IEnumerator CatInTrouble() {
        curiosityParticles.gameObject.SetActive(false);
        //Coroutine for triggering ~meows of distress~
        IEnumerator MeowTrouble() {
            while (currentState == CatState.DISTRESS) {
                yield return new WaitForSeconds(DISTRESSED_MEOW_TIME);
                Meow();
            }
        }

        //Rev up those meowers
        StartCoroutine(MeowTrouble());
        Debug.Log("I'm hella distressed!");
        var startTime = Time.time;

        //Increase distress level
        while (Time.time - startTime < distressTime) {
            var scalar = (Time.time - startTime) / distressTime;
            _distressLevel = scalar;
            yield return new WaitForEndOfFrame();
        }

        setState(CatState.DEAD);
    }

    /// <summary>
    /// Free the cat
    /// </summary>
    public void rescue() {
        _distressLevel = 0;
        curiosity = 0;
        Debug.Log("Rescued!");
        StopAllCoroutines();
        goalHazard.occupied = false;
        goalHazard = null;
        setState(CatState.ROAMING);
        //todo: Deprecated, delete
        /*
        catSpawner.spawnCat();
        Destroy(this.gameObject);*/
    }
}