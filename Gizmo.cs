using UnityEngine;

public class DebugMarker : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; // colore a scelta
        Gizmos.DrawSphere(transform.position, 0.1f); // piccolo punto nella scena
    }
}
