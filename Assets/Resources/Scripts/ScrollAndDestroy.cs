using UnityEngine;

// Move the GameObject from right to left and despawn the GameObject if its past the screen.

public class ScrollAndDestroy : MonoBehaviour
{
    // Factor that is multiplied with the base scroll speed
    public float speedMultiplicator = 1f;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < GameManager.despawnPosX)
        {
            Destroy(gameObject);
        }
        else
        {
            float scrollSpeed = 10f;
            if (gameManager)
                scrollSpeed = gameManager.scrollSpeed;
            transform.Translate(-scrollSpeed * speedMultiplicator * Time.deltaTime, 0, 0, Space.World);
        }
    }
}
