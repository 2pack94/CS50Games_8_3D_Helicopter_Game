using UnityEngine;

// Pickup that can be collected by the Player to increase the coin count.

public class Pickup : MonoBehaviour
{
    [Tooltip("The amount of coins that the player gets when collected.")]
    public int value = 1;
    // The Prefabs also have the ScrollAndDestroy component.
    // Particle system and Audio that are components of the Pickup prefab cannot be used,
    // because they would get destroyed when the Pickup gets destroyed.
    // It's also important that each Pickup can instantiate its own audio/ particle prefab and not reference
    // the same GameObject, so multiple audios/ particles can be played at the same time.
    [Tooltip("Optional Particle System Prefab to play on pickup collection.")]
    public ParticleSystem particlesPrefab;
    [Tooltip("Optional Audio Source Prefab to play on pickup collection.")]
    public AudioSource audioPrefab;
    private bool isDestroyed = false;
    // in degrees / s
    private readonly float rotateSpeed = 180f;

    // Update is called once per frame
    void Update ()
    {
        // The Pickup is rotated using the Update function and the
        // helicopter rotors for example are animated using Unity animators.
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed)
            return;

        PickupCollector pickupCollector = other.GetComponentInParent<PickupCollector>();
        if (pickupCollector)
        {
            if (audioPrefab)
            {
                AudioSource audio = Instantiate(audioPrefab, transform.position, Quaternion.identity);
                audio.Play();
            }
            if (particlesPrefab)
            {
                ParticleSystem particles = Instantiate(particlesPrefab, transform.position, Quaternion.identity);
                particles.Play();
            }

            pickupCollector.Pickup(value);

            // Set a flag to avoid that the Pickup is collected multiple times if it collides with multiple
            // helicopter colliders in the same frame.
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
