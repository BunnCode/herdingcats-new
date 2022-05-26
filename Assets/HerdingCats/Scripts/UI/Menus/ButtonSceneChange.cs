using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Component that makes a button change the current scene
/// </summary>
public class ButtonSceneChange : MonoBehaviour {
    /// <summary>
    /// The scene to load when this button is pressed
    /// </summary>
    public int SceneID;
    /// <summary>
    /// Initialize the behavior
    /// </summary>
    void Start() {
        GetComponent<Button>().onClick.AddListener(
            () => {
                SceneManager.LoadScene(SceneID);
            });
    }

}
