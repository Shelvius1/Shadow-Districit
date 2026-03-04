using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mozgási Beállítások")]
    public float walkSpeed = 2.5f;   
    public float runSpeed = 6f;      
    public float jumpForce = 5f;
    public float rotationSpeed = 450f; // Megemeltük, mert fokban számolunk!

    [Header("Finomítások")]
    public float speedSmoothness = 10f; 

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    private float currentSpeed;
    private float animatorSpeedValue;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Bemenet lekérése
        float moveInput = Input.GetAxis("Vertical");   // W/S - Előre/Hátra
        float turnInput = Input.GetAxisRaw("Horizontal"); // A/D - Azonnali forgás

        // 2. SHIFT ELLENŐRZÉS (Csak ha előre megyünk)
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveInput > 0.1f;

        // 3. CÉL SEBESSÉGEK MEGHATÁROZÁSA
        float targetSpeed = moveInput * (isRunning ? runSpeed : walkSpeed);
        
        // Az animációnak az abszolút értéket adjuk át (hátrafelé is sétáljon)
        float targetAnimatorValue = Mathf.Abs(moveInput) * (isRunning ? 1f : 0.5f);

        // --- SIMÍTÁS ---
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedSmoothness);
        animatorSpeedValue = Mathf.Lerp(animatorSpeedValue, targetAnimatorValue, Time.deltaTime * speedSmoothness);

        if (animator != null)
        {
            animator.SetFloat("Speed", animatorSpeedValue);
        }

        // 4. TALAJ ELLENŐRZÉS
        Vector3 rayStart = transform.position + new Vector3(0f, 0.5f, 0f);
        isGrounded = Physics.Raycast(rayStart, Vector3.down, 0.6f);
        Debug.DrawRay(rayStart, Vector3.down * 0.6f, isGrounded ? Color.green : Color.red);

        // 5. FORGÁS (A/D gombokra a karakter elfordul)
        // Használj nagy értéket az Inspectorban (pl. 700-1000)
transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime, Space.Self);
        // 6. MOZGÁS (Mindig a saját előre-irányába megy)
        // Unity 6-ban a linearVelocity-t használjuk
        Vector3 velocity = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        // 7. UGRÁS
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