using System.Collections;
using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    Vector3 centerPos;
    BoxCollider parentCollider;

    public Animator anim;
    public Transform leftHand;
    public Transform rightHand;

    private Vector3 leftHandTarget;
    private Vector3 rightHandTarget;

    float distance;

    void Start()
    {
        centerPos = transform.TransformPoint(transform.position);
        parentCollider = transform.parent.GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {


        if (other.transform.position.z > centerPos.z)
        {
            other.gameObject.GetComponent<Move>().Blocked(true);
        }
        else
        {
            other.gameObject.GetComponent<Move>().Blocked(false);
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform.position.z > centerPos.z)
            distance = Mathf.Abs(other.transform.position.z - parentCollider.bounds.min.z);
        else
            distance = Mathf.Abs(other.transform.position.z - parentCollider.bounds.max.z);
        float t = Mathf.InverseLerp(1.3f, 0.85f, distance);


        if (other.transform.position.z > centerPos.z)
        {
            if (!other.GetComponent<Move>().isLeft)
            {
                RotateBone rotateBone = other.GetComponent<RotateBone>();

                // passedTime 증가
                rotateBone.passedTime += Time.deltaTime / 0.3f; 
                rotateBone.passedTime = Mathf.Clamp01(rotateBone.passedTime); 

                rotateBone.value = t * rotateBone.passedTime;
                rotateBone.AnimIKOn = true;
                rotateBone.leftHandTarget2 = new Vector3(other.transform.position.x + 0.2f, other.transform.position.y + 1.2f, parentCollider.bounds.max.z - 0.1f);
                rotateBone.rightHandTarget2 = new Vector3(other.transform.position.x - 0.2f, other.transform.position.y + 1.2f, parentCollider.bounds.max.z - 0.1f);
               
            }
            else
            {
                StartCoroutine(LerpValueToZero(other.GetComponent<RotateBone>()));
            }
        }
        else
        {
            if (other.GetComponent<Move>().isLeft)
            {
                RotateBone rotateBone = other.GetComponent<RotateBone>();

                // passedTime 증가
                rotateBone.passedTime += Time.deltaTime / 0.35f; 
                rotateBone.passedTime = Mathf.Clamp01(rotateBone.passedTime); 

                rotateBone.value = t * rotateBone.passedTime;
                rotateBone.AnimIKOn = true;
                rotateBone.leftHandTarget2 = new Vector3(other.transform.position.x - 0.2f, other.transform.position.y + 1.2f, parentCollider.bounds.min.z + 0.08f);
                rotateBone.rightHandTarget2 = new Vector3(other.transform.position.x + 0.2f, other.transform.position.y + 1.2f, parentCollider.bounds.min.z + 0.08f);
            }
            else
            {
                StartCoroutine(LerpValueToZero(other.GetComponent<RotateBone>()));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        other.GetComponent<RotateBone>().AnimIKOn = false;
        other.GetComponent<RotateBone>().passedTime = 0f; // 0으로 초기화!
    }


    IEnumerator LerpValueToZero(RotateBone rotateBone)
    {
        float startValue = rotateBone.value;
        float duration = 0.2f * startValue; // value에 비례한 시간
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            rotateBone.value = Mathf.Lerp(startValue, 0f, time / duration);
            yield return null;
        }

        rotateBone.value = 0f; // 확실히 0으로 맞춰줌
        rotateBone.passedTime = 0f; // 0으로 초기화!
    }
}
