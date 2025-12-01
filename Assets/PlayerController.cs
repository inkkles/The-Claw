using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float turnSpeed = 90f; // degrees per second
    public float jumpForce = 5f;
    public float groundCheckDist = 0.2f;
    public float fallSpeedMultiplier = 2.5f;

    private Rigidbody rb;
    private Collider col;
    private bool turnClockwise = true;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>(); 
    }


    void Update()
    {
        // Toggle turn direction
        if (Input.GetKeyDown(KeyCode.P))
            turnClockwise = !turnClockwise;

        // Jump
        // if (Input.GetKeyDown(KeyCode.P) && isGrounded)
        //     rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        //if we are falling, fall faster
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * -9.81f * (fallSpeedMultiplier - 1) * Time.fixedDeltaTime;
        }
        
        isGrounded = Physics.Raycast(new Vector3(col.bounds.center.x, col.bounds.min.y + 0.01f, col.bounds.center.z)
            , Vector3.down, groundCheckDist, 3);
        // Rotate
        float rotationDirection = turnClockwise ? 1f : -1f;
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationDirection * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Move forward relative to current facing
        Vector3 forwardVelocity = transform.forward * playerSpeed;
        rb.linearVelocity = new Vector3(forwardVelocity.x, rb.linearVelocity.y, forwardVelocity.z);
    }

    bool IsGrounded() {
        Vector3 origin = new Vector3(col.bounds.center.x, col.bounds.min.y + 0.01f, col.bounds.center.z);
        return Physics.Raycast(origin, Vector3.down, groundCheckDist, 3);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // Reflect velocity based on collision normal
            Vector3 normal = other.contacts[0].normal;
            Vector3 reflectedVelocity = Vector3.Reflect(rb.linearVelocity, normal);

            // Preserve vertical velocity (e.g., jumping)
            reflectedVelocity.y = rb.linearVelocity.y;

            rb.linearVelocity = reflectedVelocity;

            // Flip character to face new direction
            Vector3 newForward = Vector3.Reflect(transform.forward, normal);
            Quaternion targetRotation = Quaternion.LookRotation(newForward, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
}

