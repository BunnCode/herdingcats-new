using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRotatorButton : MonoBehaviour {
    public int NewState = 0;
    private MenuRotator _rotator;
    // Start is called before the first frame update
    void Start() {
        _rotator = GetComponentInParent<MenuRotator>();
        //GetComponent<Button>().onClick += () => { };
    }
}
