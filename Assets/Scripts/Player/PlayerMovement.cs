using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public bool canMove = true;
    [Header("State")]
    public bool isGrounded;
    private float horizontalInput;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        // Ground Check: Shoots a ray down from the bottom of the collider
        isGrounded = Physics.Raycast(transform.position+0.1f*Vector3.up, Vector3.down, (col.bounds.extents.y +groundCheckDistance), groundLayer);

        // Visualize the ray in the Scene view
        //Debug.DrawRay(transform.position+0.1f * Vector3.up, Vector3.down * (col.bounds.extents.y + groundCheckDistance), isGrounded ? Color.green : Color.red);
    }

    private void FixedUpdate()
    {
        // Apply velocity for horizontal movement
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, horizontalInput * moveSpeed);

        // Optional: Flip the character model to face the direction of movement
        if (horizontalInput != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, horizontalInput));
        }
    }

    public void SetDirection(float direction)
    {
        horizontalInput = direction;
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


}
