using UnityEngine;

public class RotateBone : MonoBehaviour
{
    public Transform leftHandTarget;
    public Transform leftElbowTarget;

    public Vector3 leftHandTarget2;
    public Vector3 rightHandTarget2;
    Animator animator;

   
    public Move move;
    public float value;
    public float passedTime;
    public bool AnimIKOn = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!move.IKActive && !AnimIKOn)
            return;

        if (move.IKActive && !AnimIKOn)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);
        }
        else if(!move.IKActive && AnimIKOn)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, value);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, value);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, value);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, value);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget2);

            if(!move.isLeft)
                animator.SetIKRotation(AvatarIKGoal.LeftHand,Quaternion.Euler(new Vector3(-90,0,180)));
            else
                animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(new Vector3(-90, 0, 0)));
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget2);
            if (!move.isLeft)
                animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(new Vector3(-90, 0, 180)));
            else
                animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }
}
