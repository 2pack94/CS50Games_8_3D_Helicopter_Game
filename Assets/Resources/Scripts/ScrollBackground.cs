using UnityEngine;

// Scroll the Texture of the GameObject from right to left.

[RequireComponent(typeof(Renderer))]
public class ScrollBackground : MonoBehaviour
{
    [Tooltip("Scroll speed of the texture.")]
    public float scrollSpeed = 0.1f;
    private Renderer rend;
    private float offset = 0;

    // Use this for initialization
    void Start ()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update ()
    {
        offset += scrollSpeed * Time.deltaTime;

        // The texture offset defines how the texture is drawn onto the 3D object.
        // Applying an offset on one axis results in a scrolling effect.
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));

        // If the Material had other mappings, they would need to be shifted as well.
    }
}
