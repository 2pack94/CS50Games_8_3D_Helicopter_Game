using UnityEngine;
using TMPro;

// Displays the number of collected coins of the helicopter as UI Text.

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinText : MonoBehaviour
{
    public GameObject helicopter;
    private TextMeshProUGUI textMesh;
    private int coins = 0;

    // Start is called before the first frame update
    void Start ()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        // register an event handler on Helicopter Coin Pickup
        PickupCollector pickupCollector = helicopter.GetComponent<PickupCollector>();
        if (pickupCollector)
            pickupCollector.AfterPickup += OnHeliPickup;

        UpdateText();
    }

    private void UpdateText()
    {
        textMesh.text = "Coins: " + coins;
    }

    private void OnHeliPickup(object sender, GameObject collector)
    {
        coins = ((PickupCollector)sender).coinTotal;
        UpdateText();
    }
}
