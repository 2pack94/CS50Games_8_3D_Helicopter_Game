using System;
using UnityEngine;

// Destroys the GameObject when it collides with a GameObject that has a Destructible or Indestructible Component.

public class Destructible : MonoBehaviour
{
    // The Prefabs also have the ScrollAndDestroy component.
    [Tooltip("Optional Particle System Prefab to play before destruction.")]
    public ParticleSystem destroyParticlesPrefab;
    [Tooltip("Optional Audio Source Prefab to play before destruction.")]
    public AudioSource destroyAudioPrefab;
    // Publish an event before destroying the object.
    public event EventHandler<GameObject> BeforeDestroyObject;
    private bool isDestroyed = false;

    public void DestroyObject()
    {
        if (destroyAudioPrefab)
        {
            AudioSource destroyAudio = Instantiate(destroyAudioPrefab, transform.position, Quaternion.identity);
            destroyAudio.Play();
        }
        if (destroyParticlesPrefab)
        {
            ParticleSystem destroyParticles = Instantiate(destroyParticlesPrefab, transform.position, Quaternion.identity);
            destroyParticles.Play();
        }

        BeforeDestroyObject?.Invoke(this, gameObject);
        
        // Set a flag to avoid entering this function multiple times if the GameObject collides with
        // multiple Colliders in the same frame.
        isDestroyed = true;
        Destroy(gameObject);
    }

    // The OnTriggerEnter function is always called on both colliding GameObjects.
    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed)
            return;

        // GetComponentInParent retrieves the Component of Type T in the GameObject or any of its parents.
        if (other.GetComponentInParent<Destructible>() || other.GetComponentInParent<Indestructible>())
        {
            DestroyObject();
        }
    }
}
