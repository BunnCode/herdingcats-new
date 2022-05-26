using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroController : MonoBehaviour {
    public float BlackFadeTime = 1.0f;
    public float BlackFadeDelay = 1.0f;
    public float LetterSoundDelay = 0.5f;

    public float MenuFadeTime = 2.0f;

    public Animator animator;

    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup BlackFadeCanvasGroup;

    public bool _enabled;

    IEnumerator FadeInLetter(float transitionTime)
    {
        yield return new WaitForSeconds(BlackFadeDelay);
        var endOfFrame = new WaitForEndOfFrame();
        var startTime = Time.time;
        var scalar = 0.0f;
        while (Time.time - startTime <= transitionTime)
        {
            scalar = (Time.time - startTime) / transitionTime;
            Debug.Log(scalar);
            BlackFadeCanvasGroup.alpha = 1 - scalar;
            yield return endOfFrame;
        }

        MainMenuCanvasGroup.interactable = false;
        _enabled = true;
    }

    void Start() {
        StartCoroutine(FadeInLetter(BlackFadeTime));
    }
    void Update() {
        if(Input.GetMouseButtonDown(0))
            AdvanceIntro();
    }
    public void AdvanceIntro()
    {
        print("I was clicked");
        animator.SetTrigger("Open Letter");
    }

    IEnumerator FadeInMainMenu(float transitionTime)
    {
        var endOfFrame = new WaitForEndOfFrame();
        var startTime = Time.time;
        var scalar = 0.0f;
        while (Time.time - startTime <= transitionTime)
        {
            scalar = (Time.time - startTime) / transitionTime;
            MainMenuCanvasGroup.alpha = scalar;
            yield return endOfFrame;
        }

        MainMenuCanvasGroup.interactable = true;
    }

    public void FadeInMenu() {
        if (!_enabled)
            return;
        StartCoroutine(FadeInMainMenu(MenuFadeTime));
    }
}
