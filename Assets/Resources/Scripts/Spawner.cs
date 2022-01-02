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
                Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z));

            // If the spawned Game Object intersects with others, despawn it or despawn others depending on its type.

            // Loop through every collider in the instantiated prefab.
            foreach (var boxCollider in spawnedObj.GetComponents<BoxCollider>())
            {
                // Make a temporary GameObject that has the exact dimensions, position and rotation of the collider.
                GameObject tmpObject = new();
                // The Transform of the Collider is the same as for its GameObject.
                tmpObject.transform.SetPositionAndRotation(boxCollider.transform.position, boxCollider.transform.rotation);
                // The BoxCollider size is unaffected by the GameObject scale, so it must be manually multiplied by the scale.
                tmpObject.transform.localScale = new(
                    boxCollider.size.x * boxCollider.transform.localScale.x,
                    boxCollider.size.y * boxCollider.transform.localScale.y,
                    boxCollider.size.z * boxCollider.transform.localScale.z
                );
                // The temporary GameObject must be translated to the BoxCollider position in local coordinates.
                // This is needed if the GameObject has a pivot off center and a rotation that would move the center of the GameObject.
                // The BoxCollider center is relative to the GameObject position/ rotation and unaffected by the GameObject scale.
                tmpObject.transform.Translate(
                    new(
                        boxCollider.center.x * boxCollider.transform.localScale.x,
                        boxCollider.center.y * boxCollider.transform.localScale.y,
                        boxCollider.center.z * boxCollider.transform.localScale.z
                    ),
                    Space.Self
                );

                // Use the temporary GameObject transform for OverlapBox.
                // All colliders that are intersecting with this box are returned.
                // This works because all GameObjects only use Box Collider.
                // second parameter: OverlapBox extends (half edge size).
                Collider[] hitColliders = Physics.OverlapBox(tmpObject.transform.position,
                    tmpObject.transform.localScale / 2, tmpObject.transform.rotation);

                Destroy(tmpObject);

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
