using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroController : MonoBehaviour {
    public Animator animator;
    public void AdvanceIntro()
    {
        print("I was clicked");
        animator.SetTrigger("Open Letter");
    }
}
