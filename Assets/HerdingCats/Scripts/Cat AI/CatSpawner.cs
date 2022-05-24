using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    public int InitialCatsToSpawn = 3;
    //Creates all the objects the cat will interact with.
    public List<Transform> randomPositions;
    public List<Hazard> hazards;
    public Hazard hazard;

    //The spawn rate.
    private const float SPAWN_RATE = 30;

    //The TrappedCat prefab.
    public CatAI catPrefab;

    //Instantiates the cat prefab, and gives it the objects it will interact with.
    public void spawnCat()
    {
        CatAI catAI = Instantiate(catPrefab, transform.position, Quaternion.identity);
        catAI.setSpawner(this);
        catAI.setPositions(randomPositions, hazards);
    }

    //Calls the spawning loop at startup.
    private void Start()
    {
        //Spawn N initial cats
        for (int i = 0; i < InitialCatsToSpawn; i++) {
            spawnCat();
        }
        StartCoroutine("spawnTimer");
    }

    //At an interval of SPAWN_RATE, this loop will spawn a new cat prefab!
    //At this time, there is no breakout case.  There probly won't be one,
    // since it's a survival game ;)
    private IEnumerator spawnTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(SPAWN_RATE);
            spawnCat();
        }
    }
}
