using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Beállítások")]
    public Vector3 offset = new Vector3(0f, 3.5f, -6f);
    public float smoothSpeed = 5f; // Ez vezérli a mozgás és forgás lágyságát egyszerre

    void LateUpdate()
    {
        if (target == null) return;

        // 1. KISZÁMÍTJUK A CÉL POZÍCIÓT (Mindig a karakter mögött)
        // A TransformPoint biztosítja, hogy az offset a karakter helyi irányaihoz igazodjon
        Vector3 targetPosition = target.TransformPoint(offset);

        // 2. SIMÍTOTT MOZGÁS
        // Egyszerre mozgatjuk a kamerát a célpont felé
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 3. SIMÍTOTT FORGÁS
        // A Slerp biztosítja a lágy nézést
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, smoothSpeed * Time.deltaTime);
    }
}