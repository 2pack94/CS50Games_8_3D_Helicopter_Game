using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

// Displays text when the Player died and reloads the scene if the Confirm button is pressed.

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameOverText : MonoBehaviour
{
    public GameObject helicopter;
    private TextMeshProUGUI textMesh;
    private int coins;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "";
        // register an event handler on Helicopter Destruction
        Destructible heliDestructable = helicopter.GetComponent<Destructible>();
        if (heliDestructable)
            heliDestructable.BeforeDestroyObject += OnHeliDestroy;
    }

    private void OnHeliDestroy(object sender, GameObject destroyedObject)
    {
        // construct text to display
        coins = 0;
        PickupCollector pickupCollector = destroyedObject.GetComponent<PickupCollector>();
        if (pickupCollector)
            coins = pickupCollector.coinTotal;

        string confirmButton = "";
        if (gameManager)
            confirmButton = gameManager.GetButtonsForAction("Confirm").FirstOrDefault();
        string coinText = coins == 1 ? "Coin" : "Coins";
        textMesh.text = 
            $"Game Over\n" +
            $"Your Score:\n" +
            $"{coins} {coinText}\n" +
            $"Press [{confirmButton}] to Restart!";
    }

    private void OnConfirm(InputValue _)
    {
        // Called when pressing the Confirm button. If Helicopter is destroyed reload scene.
        if (!helicopter)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
