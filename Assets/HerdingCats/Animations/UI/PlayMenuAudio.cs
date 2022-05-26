using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenuAudio : MonoBehaviour {
    public AudioClip clip;
    public void PlaySound() {
        var source = GetComponentInParent<AudioSource>();
        source.clip = clip;
        source.Play();
    }
}
