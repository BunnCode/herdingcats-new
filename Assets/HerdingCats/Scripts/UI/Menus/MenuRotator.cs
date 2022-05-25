using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotator : MonoBehaviour {
    public int NumStates = 4;
    public float TransitionTime = 1;

    private int _currentState = 0;
    private bool _currentlyRotating = false;

    Quaternion AngleFromState(int state) {
        float angle = ((float)state / (float)NumStates) * 360.0f;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator RotateToState(float transitionTime, int newState) {
        var endOfFrame = new WaitForEndOfFrame();
        var startTime = Time.time;
        var scalar = 0.0f;
        while (Time.time - startTime <= transitionTime)
        {
            scalar = (Time.time - startTime) / transitionTime;
            transform.rotation = Quaternion.Lerp(AngleFromState(_currentState), AngleFromState(newState),
                Mathf.SmoothStep(0, 1, scalar));
            yield return endOfFrame;
        }

        _currentlyRotating = false;
        _currentState = newState;
    }

    public void ChangeState(int newState) {
        if (_currentlyRotating) {
            Debug.Log("Tried to rotate menu while already rotating");
            return;
        }

        _currentlyRotating = true;
        StartCoroutine(RotateToState(TransitionTime, newState));
        //return true;
    }
}
