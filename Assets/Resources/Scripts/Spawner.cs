using UnityEngine;
using System.Collections;

// Instantiates prefabs past the right edge of the screen.

public class Spawner : MonoBehaviour
{
    [Tooltip("Array of prefabs to spawn. They are chosen randomly.")]
    public GameObject[] prefabs;
    [Tooltip("Minimum Height (y-coordinate) to spawn the prefab.")]
    public float spawnHeightMin = -10f;
    [Tooltip("Maximum Height (y-coordinate) to spawn the prefab.")]
    public float spawnHeightMax = 10f;
    [Tooltip("Minimum Time to wait to spawn the next prefab.")]
    [Min(0.01f)]
    public float spawnPeriodMin = 3;
    [Tooltip("Maximum Time to wait to spawn the next prefab.")]
    [Min(0.01f)]
    public float spawnPeriodMax = 3;
    [Tooltip("Rotation to spawn the prefab at.")]
    public Vector3 spawnRotation = new(0, 0, 0);

    // Start is called before the first frame update
    void Start ()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true) {
            yield return new WaitForSeconds(Random.Range(spawnPeriodMin, spawnPeriodMax));

            GameObject spawnedObj = Instantiate(prefabs[Random.Range(0, prefabs.Length)],
                new Vector3(GameManager.spawnPosX, Random.Range(spawnHeightMin, spawnHeightMax), GameManager.scrollPosZ),
                Quaternion.Euler(spawnRotation));

            // If the spawned Game Object intersects with others, despawn it or despawn others depending on its type.

            // Loop through every collider in the instantiated prefab.
            foreach (var boxCollider in spawnedObj.GetComponents<BoxCollider>())
            {
                // Get the global dimensions, position and rotation of the box collider.
                // The Transform of the Collider is the Transform its GameObject.
                // Vector3.Scale() multiplies two vectors component-wise.
                // The BoxCollider size is unaffected by the GameObject scale, so it must be manually multiplied by the scale.
                // The BoxCollider center is relative to the GameObject position/ rotation and unaffected by the GameObject scale.
                // Translate transform.position in local coordinates towards the relative box collider position to get the
                // absolute box collider position.
                // transform.right, transform.up, transform.forward are vectors that represent the local x, y, z axis.
                Vector3 boxColliderRelPos = Vector3.Scale(boxCollider.center, boxCollider.transform.localScale);
                Vector3 boxColliderPos = boxCollider.transform.position +
                    boxCollider.transform.right * boxColliderRelPos.x +
                    boxCollider.transform.up * boxColliderRelPos.y +
                    boxCollider.transform.forward * boxColliderRelPos.z;
                Quaternion boxColliderRotation = boxCollider.transform.rotation;
                // Extends means half the edge size.
                Vector3 boxColliderExtends = Vector3.Scale(boxCollider.size, boxCollider.transform.localScale) / 2;

                // All colliders that are intersecting with this box are returned.
                // This works because all GameObjects only use Box Collider.
                Collider[] hitColliders = Physics.OverlapBox(boxColliderPos, boxColliderExtends, boxColliderRotation);

                // Loop through every intersecting collider.
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject == spawnedObj)
                        continue;

                    if (spawnedObj.GetComponent<Indestructible>())
                    {
                        // Destroy any other Game Object that intersects with an Indestructible.
                        if (!hitCollider.gameObject.GetComponent<Indestructible>())
                        {
                            Destroy(hitCollider.gameObject);
                        }
                    }
                    else if (spawnedObj.GetComponent<Destructible>())
                    {
                        // Destroy the spawned Game Object if it intersects with other solid Game Objects.
                        if (hitCollider.gameObject.GetComponent<Indestructible>() || hitCollider.gameObject.GetComponent<Destructible>())
                        {
                            Destroy(spawnedObj);
                            break;
                        }
                    }
                    else if (spawnedObj.GetComponent<Pickup>())
                    {
                        // Destroy the Pickup if it intersects with other Pickups or an Indestructible.
                        if (hitCollider.gameObject.GetComponent<Indestructible>() || hitCollider.gameObject.GetComponent<Pickup>())
                        {
                            Destroy(spawnedObj);
                            break;
                        }
                    }
                }
            }
        }
    }
}
