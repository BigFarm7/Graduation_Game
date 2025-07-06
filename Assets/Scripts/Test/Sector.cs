using UnityEngine;

[System.Serializable]
public class Sector : MonoBehaviour
{
    public GameObject sectorObject;         // �� ������ GameObject ������ �ν��Ͻ�
    public Transform sectorCenter;          // ���� �߽� ��ġ
    public float activationRadius = 100f;   // �ε� �Ÿ�

    public bool isLoaded { get; private set; } = false;
    public bool isDirty { get; private set; } = true;

    public bool isPlayerClose(Vector3 playerPos)
    {
        float distance = Vector3.Distance(playerPos, sectorCenter.position);
        return distance <= activationRadius;
    }

    public void markDirty()
    {
        isDirty = true;
    }

    public void Clean()
    {
        isDirty = false;
    }

    public void LoadContent()
    {
        if (sectorObject != null)
            sectorObject.SetActive(true);

        isLoaded = true;
    }

    public void UnloadContent()
    {
        if (sectorObject != null)
            sectorObject.SetActive(false);

        isLoaded = false;
    }
}
