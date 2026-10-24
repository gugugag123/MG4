using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3.0f;
    [SerializeField] private float _turnSpeed = 10.0f;

    // ------------------------------------------------------------------------
    // This method, CALLED EVERY FRAME, detects input from the player's keyboard
    //      and moves the fish accordingly.
    private void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movement.z = 1;
        }
        // STEP 1 -------------------------------------------------------------
        // The below else if() statment currently ONLY checks for S KEY input.
        // Make this an OR statement (like the one above!) so that it also
        //      checks for input from the DOWN ARROW key.
        // Key code reference: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/KeyCode.html 
        //
        // Feature 1 of the rubric should be complete after this.
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement.z = -1;
        }
        // STEP 1 -------------------------------------------------------------

        // STEP 2 -------------------------------------------------------------
        //  Add one more IF and ELSE/IF statement to move the player UP and DOWN.
        //  If the player hits the Q key, they should move UP.
        //  If the player hits the E key, they should move DOWN.
        //  
        //  This is going to look very similar to the statements above, but using
        //      different keys to change a different axis of movement.
        //
        //  Ask yourself: which axis (X, Y, or Z) of movement should I change to
        //      move up and down? Is negative up or down? Is positive up or down?
        //
        // Feature 2 of the rubric should be complete after this.
        


        // STEP 2 -------------------------------------------------------------
        if (Input.GetKey(KeyCode.Q))
            movement.y = 1;
        else if (Input.GetKey(KeyCode.E))
            movement.y = -1;

        transform.Translate(movement * _moveSpeed * Time.deltaTime);

        float rotation = Input.GetAxis("Horizontal") * _turnSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

    }
}
