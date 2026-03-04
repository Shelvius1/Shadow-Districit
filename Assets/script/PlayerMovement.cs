using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mozgási Beállítások")]
    public float walkSpeed = 2.5f;   // W - Séta sebessége
    public float runSpeed = 6f;      // Shift + W - Futás sebessége
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        // Megakadályozzuk, hogy a fizika felborítsa a karaktert
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Bemenet lekérése
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Irány és mozgás erőssége
        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        float inputMagnitude = movement.magnitude;

        // 2. SHIFT ELLENŐRZÉS (Séta vagy Futás)
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f;

        float currentSpeed = 0f;
        float animatorSpeedValue = 0f;

      if (inputMagnitude > 0.1f)
{
    if (Input.GetKey(KeyCode.LeftShift)) // Ha nyomod a Shiftet
    {
        currentSpeed = runSpeed;     // Pl. 6
        animatorSpeedValue = 1.0f;   // Ez a Blend Tree-ben a FUTÁS
    }
    else // Ha CSAK a W-t (vagy más irányt) nyomsz
    {
        currentSpeed = walkSpeed;    // Pl. 2.5
        animatorSpeedValue = 0.5f;   // Ez a Blend Tree-ben a SÉTA
    }
}
else
{
    currentSpeed = 0f;
    animatorSpeedValue = 0f;         // Állás
}

animator.SetFloat("Speed", animatorSpeedValue);
      

        // Érték átadása az Animator "Speed" paraméterének
        if (animator != null)
        {
            animator.SetFloat("Speed", animatorSpeedValue);
        }

        // 3. TALAJ ELLENŐRZÉS
        // A biztonság kedvéért rb.velocity-t használunk linearVelocity helyett
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f) && rb.linearVelocity.y <= 0.1f;

        // 4. MOZGÁS ÉS FORGÁS
        if (inputMagnitude >= 0.1f)
        {
            Vector3 moveDir = movement.normalized;
            
            // Forgatás
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Haladás (velocity-t használunk a kompatibilitás miatt)
            rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
        }
        else
        {
            // Megállás, de megőrizzük a zuhanási sebességet
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        // 5. UGRÁS
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            if (animator != null)
                animator.SetBool("jump", true);
        }
        else if (isGrounded)
        {
            if (animator != null)
                animator.SetBool("jump", false);
        }
    }
}