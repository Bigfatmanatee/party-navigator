using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float rotationIntensity = 5f;
    public float mouseRotationSpeed = 5f;
    public float movementTiltSpeed = 5f;
    public float walkBobIntensity = 5f;
    public float walkBobRate = 10f;

    // Checks if there is an object above the player's head preventing standing
    public Transform crouchDetector;

    public Vector3 crouchSize;
    private Vector3 startSize;

    private Rigidbody rb;
    private float speed;
    private int mode;

    // Keycodes
    private float horizontalMultiplier = 1;
    private float verticalMultiplier = 1;

    private Quaternion targetRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = moveSpeed;
        startSize = transform.localScale;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (GameStateManager.GetGameState() == 0)
        {
            // Mouse inputs
            float horizontalInput = Input.GetAxis("Horizontal") * horizontalMultiplier;
            float verticalInput = Input.GetAxis("Vertical") * verticalMultiplier;

            // Applying movement via velocity
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

            // Little bobbing efffect
            float bobValue = 0;

            if (movement != Vector3.zero)
            {
                bobValue = Mathf.Sin(Time.time * walkBobRate) * walkBobIntensity * movement.magnitude;
            }

            // Movement tilt
            Transform child1 = transform.GetChild(0);
            child1.localRotation = Quaternion.Lerp(child1.rotation, Quaternion.Euler(-verticalInput * rotationIntensity, 0f, horizontalInput * rotationIntensity + bobValue), movementTiltSpeed * Time.deltaTime);

            // Jump / Sprint / Crouch Keybinds
            if (Input.GetKeyDown(KeyCode.Space))
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (Input.GetKey(KeyCode.LeftShift))
                speed = Mathf.Lerp(speed, sprintSpeed, 4f * Time.deltaTime);
            else
                speed = Mathf.Lerp(speed, moveSpeed, 4f * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftControl))
                transform.localScale = Vector3.Lerp(transform.localScale, crouchSize, Time.deltaTime * 5f);

            else if (!IsCrouchColliding())
            {
                transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * 5f);
            }

            // Mouse rotation
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(cameraRay, out float rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                Vector3 directionToLook = pointToLook - transform.position;
                directionToLook.y = 0; // Keep the rotation flat
                targetRotation = Quaternion.LookRotation(directionToLook);
            }
        }
    }

    private void FixedUpdate()
    {
        // Applying mouse rotation
        Transform child2 = transform.GetChild(0).GetChild(0);
        child2.localRotation = Quaternion.Lerp(child2.localRotation, targetRotation, mouseRotationSpeed * Time.fixedDeltaTime);
    }


    private bool IsCrouchColliding()
    {
        Collider[] results = Physics.OverlapBox(crouchDetector.position, crouchDetector.lossyScale / 2, crouchDetector.rotation);

        int badColliders = 0;
        foreach (var collider in results)
        {

            if (!collider.transform.CompareTag("Player"))
                badColliders++;
        }

        return !(badColliders == 0);
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }
}

