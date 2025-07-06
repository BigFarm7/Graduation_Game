using UnityEngine;

public class laserWithRaycast : MonoBehaviour
{
    public Transform gunMuzzle; // �ѱ� ��ġ
    public float laserDistance = 10f;
    public Move move;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!move.IKActive)
        {
            lineRenderer.enabled = false;
            return;
        }
            

        lineRenderer.enabled = true;

        Vector3 startPoint = gunMuzzle.position;
        Vector3 direction = gunMuzzle.forward;
        Vector3 endPoint = startPoint + direction * laserDistance;

        // Raycast�� �̿��� �浹 üũ
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, laserDistance))
        {
            endPoint = hit.point; // �浹 �������� ����
        }

        // ������ ���� ����
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
