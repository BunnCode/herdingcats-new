using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
    /// <summary>
    /// Lead time on hazard triggering
    /// </summary>
    private const float TIME_BEFORE_HAZARD_SPRINGS = 1;

    //A 'trapped' boolean to notify the player object(s) that the hazard object
    // can be interacted with.
    public bool trapped = false;

    //An 'occupied' boolean for the cat objects so they don't end up dog-piling
    // (pun intended) the hazard.
    public bool occupied = false;

    //A cat object.  To be defined in TrapCat()
    public CatAI TrappedCat;

    //Since the hazard is totally reactionary, there's not really anything going
    // on during startup.
    void Start() { }

    //Probly should've put something in here, too.. Everything seems to work though
    // soooo...
    void Update() { }

    //Calls the rescue function in the CatAI script and resets the cat object.
    //Will change this to call the help() function once we get a health bar going.
    public void freeCat() {
        trapped = false;
        TrappedCat.rescue();
        TrappedCat = null;
        Debug.Log("The cat is free!");
    }

    /// <summary>
    /// After a delay, trap the given cat
    /// </summary>
    /// <param name="cat">TrappedCat to trap</param>
    /// <returns></returns>
    IEnumerator TrapCat(CatAI cat) {
        //Wait until trap can spring
        yield return new WaitForSeconds(TIME_BEFORE_HAZARD_SPRINGS);
        //Once the trap has waited, spring if the cat is close enough
        trapped = true;
        Debug.Log("The cat is trapped!");
        TrappedCat.setState(CatState.DISTRESS);
    }

    //The trigger to detect a cat collision! Calls the TrapCat function and
    // passes it the cat.
    private void OnTriggerEnter(Collider col) {
        var cat = col.GetComponent<CatAI>();
        //Only trigger if we have a cat that's actively going for this hazard
        if (cat && cat.goalHazard == this) {
            TrappedCat = cat;
            StartCoroutine(TrapCat(cat));
        }
    }
}