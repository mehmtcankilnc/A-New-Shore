using UnityEngine;

public class RotateAroundEdge : MonoBehaviour
{
    public Vector3 pivotOffset; // Pivot noktas�n�n objeden uzakl���
    public float rotationSpeed = 50f; // D�n�� h�z� (derece/saniye)

    private Vector3 pivotPoint;

    void Start()
    {
        // Pivot noktas�n� objenin konumuna g�re belirle
        pivotPoint = transform.position + pivotOffset;
    }

    void Update()
    {
        // Obje pivot noktas� etraf�nda d�ner
        transform.RotateAround(pivotPoint, Vector3.back, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Pivot noktas�n� g�rmek i�in bir k�re �izelim
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + pivotOffset, 0.1f);
    }
}

