using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Takes the Player input and moves the GameObject accordingly.
// This Component is attached to the Helicopter GameObject.

[RequireComponent(typeof(Rigidbody))]
public class HeliController : MonoBehaviour
{
    public float speed = 20f;
    private Rigidbody rigidBody;
    private Vector2 movementVector;
    private Vector2 boundaryRightTop = new(20f, 9f);
    private Vector2 boundaryLeftBottom = new(-20f, -9.5f);

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called on a reliable timer, independent of the frame rate.
    // All physics calculations occur immediately after FixedUpdate.
    private void FixedUpdate()
    {
        Vector3 movement = new(movementVector.x, movementVector.y, 0f);

        // Constrain movement to the static camera view.
        if (transform.position.x >= boundaryRightTop.x)
        {
            movement.x = Math.Min(0, movement.x);
        }
        if (transform.position.x <= boundaryLeftBottom.x)
        {
            movement.x = Math.Max(0, movement.x);
        }
        if (transform.position.y >= boundaryRightTop.y)
        {
            movement.y = Math.Min(0, movement.y);
        }
        if (transform.position.y <= boundaryLeftBottom.y)
        {
            movement.y = Math.Max(0, movement.y);
        }
        rigidBody.velocity = movement * speed;
    }

    // The Player Input Component is attached to a Game Object that is at the root of the scene.
    // The behavior field is set to Broadcast Messages. This makes the Input Action callbacks available to every child object in the hierarchy.
    private void OnMovePlayer(InputValue movementValue)
    {
        // Called on pressing the movement keys. movementVector is a unit vector.
        movementVector = movementValue.Get<Vector2>();
    }
}
