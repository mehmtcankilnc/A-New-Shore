using UnityEngine;

public class RotateAroundEdge : MonoBehaviour
{
    public Vector3 pivotOffset; // Pivot noktasýnýn objeden uzaklýðý
    public float rotationSpeed = 50f; // Dönüþ hýzý (derece/saniye)

    private Vector3 pivotPoint;

    void Start()
    {
        // Pivot noktasýný objenin konumuna göre belirle
        pivotPoint = transform.position + pivotOffset;
    }

    void Update()
    {
        // Obje pivot noktasý etrafýnda döner
        transform.RotateAround(pivotPoint, Vector3.back, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Pivot noktasýný görmek için bir küre çizelim
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + pivotOffset, 0.1f);
    }
}

