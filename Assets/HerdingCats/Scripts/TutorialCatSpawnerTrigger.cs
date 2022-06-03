using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCatSpawnerTrigger : MonoBehaviour {
    public CatSpawner spawner;

    void OnTriggerEnter(Collider col) {
        spawner.enabled = true;
    }
}
