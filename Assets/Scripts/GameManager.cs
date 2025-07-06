using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 targetPos1;
    public Vector3 targetPos2;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 100;
    }
    public void GetPos(Vector3 TargetPos)
    {
        targetPos1 = TargetPos;
    }
    public void GetPos2(Vector3 TargetPos)
    {
        targetPos2 = TargetPos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(targetPos1, 0.1f);
        Gizmos.DrawSphere(targetPos2, 0.1f);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
