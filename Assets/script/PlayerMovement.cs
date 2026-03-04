using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mozgási Beállítások")]
    public float walkSpeed = 2.5f;   
    public float runSpeed = 6f;      
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    [Header("Finomítások")]
    public float speedSmoothness = 10f; // Mennyire "csússzon" a megállás/indulás (kisebb szám = lassabb megállás)

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    // Ezeket osztályszintre emeltük, hogy a program "emlékezzen" rájuk!
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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        float inputMagnitude = movement.magnitude;

        // 2. SHIFT ELLENŐRZÉS
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f;

        // 3. CÉL SEBESSÉGEK MEGHATÁROZÁSA
        float targetSpeed = 0f;
        float targetAnimatorValue = 0f;

        if (inputMagnitude > 0.1f)
        {
            if (isRunning)
            {
                targetSpeed = runSpeed;
                targetAnimatorValue = 1f;  
            }
            else
            {
                targetSpeed = walkSpeed;
                targetAnimatorValue = 0.5f; 
            }
        }

        // --- A VARÁZSLAT ITT TÖRTÉNIK (LERP) ---
        // A jelenlegi sebesség fokozatosan "csúszik" a cél sebesség felé
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedSmoothness);
        animatorSpeedValue = Mathf.Lerp(animatorSpeedValue, targetAnimatorValue, Time.deltaTime * speedSmoothness);

        // Érték átadása az Animatornak
        if (animator != null)
        {
            animator.SetFloat("Speed", animatorSpeedValue);
        }

        // 4. TALAJ ELLENŐRZÉS
        Vector3 rayStart = transform.position + new Vector3(0f, 0.5f, 0f);

// 2. Kilövünk egy sugarat lefelé 0.6f hosszan. Így pontosan 0.1 méterrel a cipőtalp alá is leér!
isGrounded = Physics.Raycast(rayStart, Vector3.down, 0.6f);

// 3. Rajzolunk egy tesztvonalat a Scene ablakba
Debug.DrawRay(rayStart, Vector3.down * 0.6f, isGrounded ? Color.green : Color.red);
        // 5. MOZGÁS ÉS FORGÁS
        if (inputMagnitude >= 0.1f)
        {
            Vector3 moveDir = movement.normalized;
            
            // Forgatás
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Haladás az "elsimított" sebességgel
            rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
        }
        else
        {
            // Ha elengedjük a gombot, még mindig a "currentSpeed" mozgatja picit előre a lassulás alatt!
            // Így a fizika és az animáció EGYÜTT lassul le, nem csúszik a lába!
            rb.linearVelocity = new Vector3(transform.forward.x * currentSpeed, rb.linearVelocity.y, transform.forward.z * currentSpeed);
        }

        // 6. UGRÁS
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