using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // A Player (karakter) Transformja
    public Vector3 offset = new Vector3(0f, 3f, -5f); // Távolság a karaktertől (fent és hátul)
    public float smoothSpeed = 10f; // Követési finomság

    void LateUpdate() // A kamera mozgását mindig LateUpdate-be tesszük!
    {
        if (target == null) return;

        // Kiszámoljuk, hová akarunk menni
        Vector3 desiredPosition = target.position + offset;
        
        // Finoman átúsztatjuk a kamerát a jelenlegi helyéről a célhelyre
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        transform.position = smoothedPosition;

        // Mindig nézzen a karakterre
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}