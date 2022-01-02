using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// Component that contains fields and functions that are used in many other Components.
// This Component should be attached to a single instantiated GameObject that can be references by others.

public class GameManager : MonoBehaviour
{
    [Tooltip("Base scroll speed of obstacles and coins.")]
    public float scrollSpeed = 10f;
    [Tooltip("Period in seconds to increase the scroll speed by the specified amount.")]
    public float increaseScrollSpeedPeriod = 15f;
    [Tooltip("Amount to increase the scroll speed after each period.")]
    public float increaseScrollSpeedAmount = 1f;
    [Tooltip("Maximum scroll speed amount.")]
    public float scrollSpeedMax = 40f;
    [Tooltip("Input Action Asset for the Player Input.")]
    public PlayerInput playerInput;

    // Position to the right/ left to spawn/ despawn obstacles.
    public readonly static float spawnPosX = 30;
    public readonly static float despawnPosX = -30f;
    // z coordinate of the helicopter.
    public static float scrollPosZ = 15f;

    // Awake is called before any Start functions
    void Awake()
    {
        // Set the maximal frame rate of the game.
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find an active loaded Component
        HeliController heliController = UnityEngine.Object.FindObjectOfType<HeliController>();
        if (heliController)
            scrollPosZ = heliController.transform.position.z;

        StartCoroutine(IncreaseScrollSpeed());
    }

    private IEnumerator IncreaseScrollSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(increaseScrollSpeedPeriod);
            scrollSpeed = Math.Min(scrollSpeed + increaseScrollSpeedAmount, scrollSpeedMax);
        }
    }

    // actionName: Name of the Action in the Player Input Action Asset
    // return: List of strings that contain the name of the Button that triggers the action (binding).
    public IEnumerable<string> GetButtonsForAction(string actionName)
    {
        List<string> buttonList = new();
        // Find the Action across all Action Maps
        InputAction inputAction = playerInput.actions.FindAction(actionName);

        if (inputAction == null)
            return buttonList;

        foreach (var binding in inputAction.bindings)
        {
            // An Input Control Scheme is for example "Mouse&Keyboard"
            // A binding group is used to divide bindings into InputControlSchemes.
            // Return only buttons that belong to the current Control Scheme.
            string[] bindingGroups = binding.groups.Replace(" ", "").Split(";");
            if (bindingGroups.Contains(playerInput.currentControlScheme))
            {
                buttonList.Add(binding.ToDisplayString());
            }
        }

        return buttonList;
    }

    private void OnCloseGame(InputValue _)
    {
        // Called when pressing the CloseGame button.
        Application.Quit();
    }
}
