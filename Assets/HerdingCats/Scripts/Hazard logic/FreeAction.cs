using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FreeAction : MonoBehaviour {
    //todo: move this to a settings file
    private const KeyCode FREE_KEY_CODE = KeyCode.F;

    /// <summary>
    /// Reference to the Player object
    /// </summary>
    public GameObject player;


    public bool playerInFreeZone = false;
    public Hazard hazard;

    void Start() {
        HUDscript.Instance.HideFreeDialog();
    }

    private void Update() {
        //If the player is in this free zone
        if (playerInFreeZone == true) {
            //and they press the free button
            if (Input.GetKeyDown(FREE_KEY_CODE)) {
                //todo: This needs to be encapsulated in its own script or something
                HUDscript.Instance.freeMeter += 1;
                if (HUDscript.Instance.freeMeter >= 3) {
                    Debug.Log("F pressed!");
                    hazard.freeCat();
                    HUDscript.Instance.freeMeter = 0;
                    IncreaseScore();
                }
            }
        }
    }

    // While player is near Hazard, they can perform actions and text appears on screen
    void OnTriggerStay(Collider other) {
        if (other.gameObject == player) {
            //Debug.Log("I can free the TrappedCat!");
            if (hazard.trapped) {
                HUDscript.Instance.ShowFreeDialog();
                playerInFreeZone = true;
            }
            else {
                HUDscript.Instance.HideFreeDialog();
                playerInFreeZone = false;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == player) {
            playerInFreeZone = false;
            HUDscript.Instance.HideFreeDialog();
        }
    }

    void IncreaseScore() {
        //todo: This needs to be a different method instead of being directly controlled
        HUDscript.Instance.score += 9;
    }
}