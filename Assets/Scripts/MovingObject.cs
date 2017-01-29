using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // Time object will take to move in seconds
    public float moveTime = 0.1f;

    // Layer on which collision will be checked
    public LayerMask blockingLayer;

    // Components attached to this object
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    // Used to make movement more efficient
    private float inverseMoveTime;          

    protected virtual void Start ()
    {
        // Get components references
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // Complicated calculations for complex reasons
        inverseMoveTime = 1f / moveTime;
    }

    // Returns true if it was able to move ; RaycastHit2D is passed by reference (out keyword) and is used to check collision
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        // Store start position to move from, based on objects current transform position (actually returning a Vector3 casted to a Vector2)
        Vector2 start = transform.position;

        // Calculate end position
        Vector2 end = start + new Vector2(xDir, yDir);

        // Disable the boxCollider so that linecast doesn't hit this object's own collider
        boxCollider.enabled = false;

        // Cast a line from start point to end point checking collision on blockingLayer
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // Re-enable boxCollider after linecast
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            // If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));

            // Move was successful !
            return true;
        }

        else
            // Move was unsuccessful...
            return false;
    }

    // Co-routine for moving units from one space to next, takes a parameter end to specify where to move to
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        // Calculate the remaining distance based on the square magnitude (cheaper ?) of the difference between current position and end parameter
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // Move while that distance is greater than a very small amount (Epsilon ~ zero)
        while (sqrRemainingDistance > float.Epsilon)
        {
            // Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            // Move the RigidBody to the calculated position
            rb2D.MovePosition(newPostion);

            // Recalculate the remaining distance after moving
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            // Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    // The generic parameter T specifies the type of component we expect our object to interact with if blocked (Player for Enemies, Walls for Player)
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        // Hit will store whatever our linecast hits when Move is called
        RaycastHit2D hit;

        // Attempt to move
        bool canMove = Move(xDir, yDir, out hit);

        if (canMove)
            // If nothing was hit, return
            return;

        else
        {
            // Get a component reference to the component of type T attached to the object that was hit
            T hitComponent = hit.transform.GetComponent<T>();

            if (hitComponent != null)
                // If MovingObject is blocked and has hit something it can interact with, call OnCantMove and pass it hitComponent
                OnCantMove(hitComponent);
        }
    }

    // Abstract function to be overriden
    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
