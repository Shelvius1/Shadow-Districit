using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        // Kódból is fixáljuk a forgást, de az Inspectorban lévő Constraints a legfontosabb!
        rb.freezeRotation = true;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Mozgási irány kiszámítása
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

        // --- TALAJ ELLENŐRZÉS (Raycast) ---
        // Egy 1.1 egység hosszú láthatatlan sugarat lövünk lefelé a karakter közepéből. 
        // A "rb.linearVelocity.y <= 0.1f" feltétel biztosítja, hogy ugrás (felfelé haladás) közben 
        // azonnal hamis legyen a földet érés, így nincs dupla ugrás.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f) && rb.linearVelocity.y <= 0.1f;

        if (movement.magnitude >= 0.1f)
        {
            // Forgatás a mozgás irányába
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // TÉNYLEGES HALADÁS
            rb.linearVelocity = new Vector3(movement.x * speed, rb.linearVelocity.y, movement.z * speed);
        }
        else
        {
            // Megálláskor ne csússzon tovább
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        // Animációk
        float moveAmount = movement.magnitude;
        animator.SetFloat("walking", moveAmount * 0.5f);
        animator.SetFloat("run", moveAmount);

        // Ugrás
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Nullázzuk a függőleges sebességet ugrás előtt, hogy mindig egyforma magasra ugorjon
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            animator.SetBool("jump", true);
        }
        
        // Ha újra stabilan a földön vagyunk, kapcsoljuk ki az ugrás animációt
        if (isGrounded)
        {
            animator.SetBool("jump", false);
        }
    }
}