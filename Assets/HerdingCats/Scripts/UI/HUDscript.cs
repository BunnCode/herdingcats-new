using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class HUDscript : MonoBehaviour {
    private static HUDscript _instance;

    /// <summary>
    /// Singleton accessor
    /// </summary>
    public static HUDscript Instance {
        get
        {
            if (!_instance) {
                _instance = GameObject.FindObjectOfType<HUDscript>();
                if (!_instance)
                    Debug.LogError("No HUDScript found in scene!");
            }

            return _instance;
        }
    }

    public float countdownTime = 300f;
    public TextMeshProUGUI countdownDisplay;

    /// <summary>
    /// Text element displaying lives left
    /// </summary>
    public TextMeshProUGUI livesText;

    /// <summary>
    /// Counter for lives
    /// </summary>
    public int lives;

    /// <summary>
    /// Counter for score
    /// </summary>
    [DoNotSerialize]
    public int score;

    /// <summary>
    /// Text element that displays how many "Free" action presses are required to free the cat
    /// </summary>
    public TextMeshProUGUI freeMeterText;

    /// <summary>
    /// Prompt the player to press the "Free" button
    /// </summary>
    public TextMeshProUGUI freePromptText;

    /// <summary>
    /// UI element for score
    /// </summary>
    public TextMeshProUGUI scoreText;

    /// <summary>
    /// Counter for how close the cat is to being freed
    /// </summary>
    public int freeMeter;

    public GameObject gameOverText;
    public GameObject playAgainButton;

    public GameObject MainMenuButton;
    public GameObject youWinText;

    /// <summary>
    /// Is the game in tutorial mode?
    /// </summary>
    public bool TutorialMode = false;

    /// <summary>
    /// Is the game in infinite mode?
    /// </summary>
    public bool InfiniteMode = false;

    // Start is called before the first frame update
    void Start() {
        //freeMeterText.alpha = 0.0F;
        Time.timeScale = 1;
        lives = 3;
        freeMeter = 0;
        UpdateLivesText();
        UpdateFreeMeterText();
        gameOverText.SetActive(false);
        playAgainButton.SetActive(false);
        MainMenuButton.SetActive(false);
        youWinText.SetActive(false);

        if (TutorialMode || InfiniteMode) {
            countdownDisplay.enabled = false;
        }
        else {
            StartCoroutine(CountdownToStart());
            countdownDisplay.enabled = true;
        }

        livesText.enabled = !TutorialMode;
    }

    // Update is called once per frame
    void Update() {
        //this could be better. would call UpdateLivesText() after a cat dies but not sure how to call inside the CatAI script
        UpdateLivesText();
        UpdateFreeMeterText();
        UpdateScoreText();
        if (lives == 0 && !TutorialMode) {
            Lose();
        }

        if (TutorialMode && score >= 9 * 3) {
            Win();
        }
    }

    /// <summary>
    /// Trigger the losing state
    /// </summary>
    public void Lose() {
        gameOverText.SetActive(true);
        playAgainButton.SetActive(true);
        MainMenuButton.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Trigger the winning state
    /// </summary>
    private void Win() {
        Time.timeScale = 0;
        youWinText.SetActive(true);
        playAgainButton.SetActive(true);
        MainMenuButton.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator CountdownToStart()
    {
        while(countdownTime >= 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(countdownTime);
            countdownDisplay.text = time.ToString(@"mm\:ss");
                //time.Minutes.ToString() + ":" + time.Seconds.ToString();

            yield return new WaitForSeconds(1f);

            if (countdownTime == 0) {
                Win();
            }
            countdownTime--;
        }
    }

    /// <summary>
    /// Update the Lives UI element
    /// </summary>
    public void UpdateLivesText() {
        livesText.text = "Lives: " + lives.ToString();
    }

    /// <summary>
    /// Show UI elements related to freeing the cat
    /// </summary>
    public void ShowFreeDialog() {
        freePromptText.gameObject.SetActive(true);
        freeMeterText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide UI elements related to freeing the cat
    /// </summary>
    public void HideFreeDialog() {
        freePromptText.gameObject.SetActive(false);
        freeMeterText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the free meter
    /// </summary>
    public void UpdateFreeMeterText() {
        freeMeterText.text = freeMeter.ToString() + "/3";
    }

    /// <summary>
    /// Update the score text
    /// </summary>
    private void UpdateScoreText() {
        scoreText.text = "Score: " + score.ToString();
    }

    /// <summary>
    /// Reload the level
    /// </summary>
    public void ReloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}