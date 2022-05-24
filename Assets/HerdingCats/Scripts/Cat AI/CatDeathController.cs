using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Local

/// <summary>
/// Simple component to keep track of cat death effects
/// </summary>
public class CatDeathController : MonoBehaviour {
    [SerializeField] private float _secondsAlive = 3;
    [SerializeField] private ParticleSystem _deathParticles = null;
    [SerializeField] private AudioSource _deathAudioSource = null;

    /// <summary>
    /// Destroy this object after a given number of seconds
    /// </summary>
    /// <param name="seconds">Number of seconds to wait</param>
    /// <returns></returns>
    IEnumerator DestroyAfterSeconds(float seconds) {
       
        yield return new WaitForSeconds(seconds);
        Destroy(this);
    }

    /// <summary>
    /// Play effects and initiate death trigger
    /// </summary>
    void Start() {
        _deathParticles.Play();
        _deathAudioSource.Play();
        StartCoroutine(DestroyAfterSeconds(_secondsAlive));
    }
}