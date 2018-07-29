using UnityEngine;

public class PunkSpawnPosition : MonoBehaviour {

    public AimingDirection m_startDirection;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.9f);

        // Draw line showing facing direction on spawn
        Vector3 dir = Quaternion.AngleAxis((float)m_startDirection, Vector3.up) * Vector3.forward;
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + dir + Vector3.up * 0.5f);
    }
}
