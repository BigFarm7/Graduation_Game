using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 lastPos;
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void OnAnimatorMove()
    {
        
        if (animator.applyRootMotion)
        {
            Vector3 deltaPosition = animator.deltaPosition;
            Quaternion deltaRotation = animator.deltaRotation;


            Debug.Log("aa");
           // deltaPosition.x = 0;

            rb.MovePosition(rb.position + deltaPosition);
            rb.MoveRotation(rb.rotation * deltaRotation);

        }
        
    }
}
