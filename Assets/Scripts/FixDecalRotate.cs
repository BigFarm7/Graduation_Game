using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FixDecalRotate : MonoBehaviour
{
    public enum Foot
    {
        Left, Right
    }
  

    [Header("Foot Select")]
    public Foot selectedFoot;
    public Transform LeftFootPos;
    public Transform RightFootPos;
    DecalProjector projector;


    private GameObject UrpDecalProjection;
    void Start()
    {
        UrpDecalProjection = this.gameObject;
        projector = this.GetComponent<DecalProjector>();
    }

    void LateUpdate()
    {

        if(selectedFoot == Foot.Left)
        {
            UrpDecalProjection.transform.position = LeftFootPos.position + new Vector3(0, 0.1f, 0f);
        }
        else
        {
            UrpDecalProjection.transform.position = RightFootPos.position + new Vector3(0, 0.1f, 0f);
        }


        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            float distance = hit.distance;
            float t = Mathf.InverseLerp(0.1f, 0.2f, distance);

            
            projector.fadeFactor = Mathf.Lerp(0.55f, 0.0f, t);
        }

    }
}
