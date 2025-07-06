using UnityEngine;

public class IKinput : MonoBehaviour
{
    private float InputX;
    private float InputZ;
    private Vector3 desiredMoveDirection;
    [Range(0, 0.5f)] public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    private float Speed;
    [Range(0, 1f)] public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    private bool isGrounded;
    
    private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    [Header("Feet Grounder")]
    public bool enableFeetIk = true;
    [Range(0,2)][SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)]
    [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0, 1)]
    [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)]
    [SerializeField] private float feetToIkPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;


    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    private float verticalVel;
    private Vector3 moveVector;
    public Transform RightFootTrans;
    public Transform LeftFootTrans;
    public Quaternion originalRightFoot;
    public Quaternion originalLeftFoot;
    void Start()
    {
        anim = this.GetComponent<Animator>();
       

        if (anim == null)
            Debug.LogError("No Animater");

    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
        if(enableFeetIk == false) { return; }
        if(anim == null) { return; }



      
        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIkPosition,ref rightFootIkRotation,originalRightFoot);
        FeetPositionSolver(leftFootPosition,ref leftFootIkPosition,ref leftFootIkRotation, originalLeftFoot);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        
        if(enableFeetIk == false) { return; }

        MovePelvisHeight();

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        if(useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

            originalRightFoot = RightFootTrans.rotation;

            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
        }

        MoveFeetToIkPoint(AvatarIKGoal.RightFoot,rightFootIkPosition,rightFootIkRotation,ref lastRightFootPositionY);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        if (useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

            originalLeftFoot = LeftFootTrans.rotation;

            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,anim.GetFloat(leftFootAnimVariableName));
        }

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
        
    }

    void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = anim.GetIKPosition(foot);

        if(positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
            targetIkPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIkPosition = transform.TransformPoint(targetIkPosition);

            anim.SetIKRotation(foot, rotationIkHolder);
        }
        anim.SetIKPosition(foot, targetIkPosition);
    }
    void MovePelvisHeight()
    {
        if(rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero|| lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = anim.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

        float totalOffset = (lOffsetPosition < rOffsetPosition)? lOffsetPosition:rOffsetPosition;

        Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY,newPelvisPosition.y,pelvisUpAndDownSpeed);

        anim.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = anim.bodyPosition.y;
    }
    void FeetPositionSolver(Vector3 fromSkyPosition,ref Vector3 feetIkPositions,ref Quaternion feetIkRotations, Quaternion FootRot)
    {
        RaycastHit feetOutHit;

      

        if(Physics.Raycast(fromSkyPosition,Vector3.down,out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            feetIkPositions = fromSkyPosition;
            feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
            //feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
           
            HumanBodyBones bone = fromSkyPosition == rightFootPosition ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot;

            Transform footTransform = anim.GetBoneTransform(bone);

            Quaternion footRotation = Quaternion.Euler(0f, FootRot.eulerAngles.y, 0f);
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * footRotation;




            return;
        }
        feetIkPositions = Vector3.zero;
    }
    void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = anim.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }

}
