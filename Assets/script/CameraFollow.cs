using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;      // a Player objektum
    public Vector3 offset;        // távolság a Player-től
    public float smoothSpeed = 0.125f; // simítás a mozgásnál

    void LateUpdate()
    {
        if(player == null) return;

        // célpozíció a Player + offset
        Vector3 desiredPosition = player.position + offset;
        // simított mozgatás
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // kamera nézzen a Player-re
        transform.LookAt(player);
    }
}
