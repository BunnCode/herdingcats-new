using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroController : MonoBehaviour {
    public Animator animator;
    public float MenuFadeTime = 2.0f;
    public CanvasGroup MenuCanvasGroup;
    public void AdvanceIntro()
    {
        print("I was clicked");
        animator.SetTrigger("Open Letter");
    }

    IEnumerator FadeIn(float transitionTime)
    {
        var endOfFrame = new WaitForEndOfFrame();
        var startTime = Time.time;
        var scalar = 0.0f;
        while (Time.time - startTime <= transitionTime)
        {
            scalar = (Time.time - startTime) / transitionTime;
            MenuCanvasGroup.alpha = scalar;
            yield return endOfFrame;
        }

        MenuCanvasGroup.interactable = true;
    }

    public void FadeInMenu() {
        StartCoroutine(FadeIn(MenuFadeTime));
    }
}
