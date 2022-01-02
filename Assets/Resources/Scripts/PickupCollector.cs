using System;
using UnityEngine;

// Allows a Game Object to collect pickups.

public class PickupCollector : MonoBehaviour
{
    [System.NonSerialized]
    public int coinTotal;
    // Publish an event after pickup.
    public event EventHandler<GameObject> AfterPickup;

    public void Pickup(int pickupValue = 1)
    {
        coinTotal += pickupValue;
        AfterPickup?.Invoke(this, gameObject);
    }
}
