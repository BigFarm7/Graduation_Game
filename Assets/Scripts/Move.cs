using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class Move : MonoBehaviour
{
    
    private Animator anim;
    public Rigidbody rb;
    private IKinput ikinput;
    private float flightTime;
    public bool isLeft = false;
    public bool canPressA = true;
    public bool canPressD = true;
    private bool isGrounded = true;
    private bool isFalling = false;
    private float elapsedTime = 0f;
    private bool isShot = false;
    private float lastJumpTime;
    private float jumpIgnoreGroundTime = 0.1f;
    private bool isOutholeFinished = false;
    private float onAirTime = 0.0f;
    private int JumpType = 0;
    public bool canMove = true;
    private string SpeedCurve = "SpeedCurve";

    public PlayerAnimState currentState = PlayerAnimState.IdleR;


    public Transform objectA; 
    public Transform objectB;

    public Transform spine1;
    public Transform spine2;
    public Transform spine3;
    public Transform hip;
    public Transform hipPos;
    public Transform handPos;
    public Transform weaponPos;

    public GameObject StepRayUpper;
    public GameObject StepRayLower;

    public Material Bar;
    public Material CircleBar;
    public Material LLight;
    public GameObject muzzleFlash;
    public GameObject Lamp;
    public Rig rigWeight;
    public bool IKActive;

    public GameObject nearbyObstacle;

    public float h = 0.0f; 
    public float v = 0.0f;


    public enum PlayerAnimState
    {
        IdleL, IdleR,
        TurnToL, TurnToR,
        FirstRunL, FirstRunR,
        RunL, RunR,
        StopL, StopR,
        JumpInPlaceL, JumpInPlaceR,
        JumpMovingL, JumpMovingR,
        LandedL, LandedR,
        LandedAndMoveL, LandedAndMoveR,
        BlockedR, BlockedL,
        OverObstacleL, OverObstacleR,
        FallingL, FallingR,
        TurntoLandStop, TurntoRandStop

    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Bar.SetFloat("_Cutoff", -0.25f);
        CircleBar.SetFloat("_Cutoff", 0f);
        ikinput = GetComponent<IKinput>();
        anim.applyRootMotion = false;

    }


    void Update()
    {/*
        if(!isOutholeFinished)
        {
            CheckOutHoleFinished();
            IKActive = false;
            return;
        }
        */
        HandleDirectionInput();

        if (Input.GetMouseButton(1))
        {
            IKActive = true;
            rigWeight.weight = 1;
            float mouseX = Input.GetAxis("Mouse X"); 
            float mouseY = Input.GetAxis("Mouse Y"); 

            h += mouseX * 4;
            v += mouseY * 4;

            objectA.rotation = Quaternion.Euler(-v, h, 0);
            
            weaponPos.SetParent(handPos);
            weaponPos.localPosition = new Vector3(0, 0.0005f, 0);
            weaponPos.localRotation = Quaternion.Euler(-90, -90, 0);

            if (Input.GetMouseButtonDown(0))
            {
                isShot = true;
                muzzleFlash.SetActive(true);
               
            }
        }
        else
        {
            IKActive = false;
            rigWeight.weight = 0;

            weaponPos.SetParent(hip);
            weaponPos.localPosition = new Vector3(-0.000665f, 0.00147f, 0.001782f);
            weaponPos.localRotation = Quaternion.Euler(79.337f, 95.992f, 4.94f);
        }

        



 

        if (isShot)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 5); // 0~1 사이로 제한
            float cutoffValue = Mathf.Lerp(1.5f, -0.25f, t);

            Bar.SetFloat("_Cutoff", cutoffValue); 
            CircleBar.SetFloat("_Cutoff", 5f);


            float loseLightValue = Mathf.Lerp(0.8f, 0f, t*8);
            LLight.SetFloat("_Cutoff", loseLightValue);

            if (t>=0.01f)
            {
                muzzleFlash.SetActive(false);
            }

            if (t>=1f)
            {
                isShot = false;
                elapsedTime = 0f;
                CircleBar.SetFloat("_Cutoff", 0f);
            }
        }

    }

   

    void FixedUpdate()
    {
      /*  if (!isOutholeFinished)
            return;

        */
         FindObstacle();
        AlwaysCheckGround();
        //StepClimb();
        

        if (isGrounded == true && isFalling == false)
        {
            onAirTime = 0.0f;
            Vector3 currentVelocity = rb.linearVelocity;
            float zSpeed = anim.GetFloat(SpeedCurve) * 4.11f;
            rb.linearVelocity = new Vector3(0, currentVelocity.y, zSpeed);
        }
        else if (isGrounded == false || isFalling == true)
        {

            if (JumpType == 0)
            {

            }
            else if (JumpType == 1)
            {

                float time = Mathf.Clamp01(onAirTime / 1f);
                if (Input.GetKey(KeyCode.A))
                {
                    onAirTime += Time.fixedDeltaTime;
                    float zSpeed = Mathf.Lerp(-4.11f, 0f, time);
                    Vector3 currentVelocity = rb.linearVelocity;
                    rb.linearVelocity = new Vector3(0, currentVelocity.y, zSpeed);
                }
                else
                {

                    float zSpeed = Mathf.Lerp(-4.11f, 0f, time);
                    Vector3 currentVelocity = rb.linearVelocity;
                    rb.linearVelocity = new Vector3(0, currentVelocity.y, zSpeed);
                }
            }
            else if (JumpType == 2)
            {
                Vector3 currentVelocity = rb.linearVelocity;
                rb.linearVelocity = new Vector3(0, currentVelocity.y, 0);
            }
            else if (JumpType == 3)
            {
                float time = Mathf.Clamp01(onAirTime / 1f);
                if (Input.GetKey(KeyCode.D))
                {
                    onAirTime += Time.fixedDeltaTime;
                    float zSpeed = Mathf.Lerp(4.11f, 0f, time);
                    Vector3 currentVelocity = rb.linearVelocity;
                    rb.linearVelocity = new Vector3(0, currentVelocity.y, zSpeed);
                }
                else
                {

                    float zSpeed = Mathf.Lerp(4.11f, 0f, time);
                    Vector3 currentVelocity = rb.linearVelocity;
                    rb.linearVelocity = new Vector3(0, currentVelocity.y, zSpeed);
                }
            }
            
        }

        

        Vector3 targetPosition = objectA.position + objectA.forward * 0.6f;
        
        
        objectB.position = targetPosition;
        objectB.rotation = Quaternion.Euler(-v,h,90);

        Vector3 hipEuler = hip.eulerAngles;
        hipEuler.y = (hipEuler.y > 350) ? hipEuler.y = 0:hipEuler.y;

        hipEuler.y = Mathf.Clamp(hipEuler.y, 0.0f, 180.0f);
       
        float t = Mathf.InverseLerp(0, 180, Mathf.Abs(hipEuler.y)); 

        // 첫 번째 회전 방식 (hip이 0일 때)
        Vector3 rotA1 = new Vector3(10 - v / 4, h / 4 + hipEuler.y, 0);
        Vector3 rotA2 = new Vector3(10 - v / 4 * 2, h / 4 * 2 + hipEuler.y, 0);
        Vector3 rotA3 = new Vector3(10 - v / 4 * 3, h / 4 * 3 + hipEuler.y, 0);

        // 두 번째 회전 방식 (hip이 180일 때)
        Vector3 rotB1 = new Vector3(10 - v / 4, -(180 - h) / 8 + hipEuler.y, 0);
        Vector3 rotB2 = new Vector3(20 - v / 4 * 2, -(180 - h) / 8 * 2 + hipEuler.y, 0);
        Vector3 rotB3 = new Vector3(20 - v / 4 * 3, -(180 - h) / 8 * 4 + hipEuler.y, 0);

        // 보간하여 부드럽게 전환
        spine1.rotation = Quaternion.Euler(Vector3.Lerp(rotA1, rotB1, t));
        spine2.rotation = Quaternion.Euler(Vector3.Lerp(rotA2, rotB2, t));
        spine3.rotation = Quaternion.Euler(Vector3.Lerp(rotA3, rotB3, t));
    }

    void HandleDirectionInput()
    {

        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);
        bool pressW = Input.GetKey(KeyCode.W);

        if(!canPressA)
        {
            pressA = false;
        }

        if(!canPressD)
        {
            pressD = false;
        }
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        CheckUntilCanMove(stateInfo);

        if (!canMove)
            return;

        if (isFalling)
        {
            if (isLeft)
            {
                PlayAnim("Falling L", PlayerAnimState.FallingL, 0.1f);
                JumpType = 3;
                return;
            }
            else
            {
                PlayAnim("Falling R", PlayerAnimState.FallingR, 0.1f);
                JumpType = 1;
                return;
            }
           
        }
        else if (!isFalling && currentState == PlayerAnimState.FallingR)
        {
            if (!pressA && pressD)
            {
                PlayAnim("Landed and Move R", PlayerAnimState.LandedAndMoveR, 0.1f);
                JumpType = 0;
            }
            else
            {
                PlayAnim("Landed R", PlayerAnimState.LandedR, 0.05f);
                JumpType = 0;
            }
        }
        else if(!isFalling && currentState == PlayerAnimState.FallingL)
        {
            if (pressA && !pressD)
            {
                PlayAnim("Landed and Move L", PlayerAnimState.LandedAndMoveL, 0.1f);
                JumpType = 0;
            }
            else
            {
                PlayAnim("Landed L", PlayerAnimState.LandedL, 0.05f);
                JumpType = 0;
            }
        }
        if (pressW && isGrounded)
        {
            isGrounded = false;
           
          

            if (pressA)
            {
                PlayAnim("Jump Moving L", PlayerAnimState.JumpMovingL, 0.1f);
                JumpType = 3;
                Jump();
            }
            else if (pressD)
            {
                PlayAnim("Jump Moving R", PlayerAnimState.JumpMovingR, 0.15f);
                JumpType = 1;
                Jump();
            }
            else
            {
                if (isLeft)
                {
                    PlayAnim("Jump in Place L", PlayerAnimState.JumpInPlaceL, 0.1f);
                    JumpType = 2;
                    Jump();
                }
                else
                {
                    PlayAnim("Jump in Place R", PlayerAnimState.JumpInPlaceR, 0.1f);
                    JumpType = 2;
                    Jump();
                }
            }

            return;
        }

        if (!isGrounded && IsGrounded())
        {
            
            isGrounded = true;

            if (currentState == PlayerAnimState.OverObstacleR)
                return;

            if (pressA)
            {
                PlayAnim("Landed and Move L", PlayerAnimState.LandedAndMoveL, 0.025f);
                JumpType = 0;
            }
            else if (pressD)
            {
                PlayAnim("Landed and Move R", PlayerAnimState.LandedAndMoveR, 0.025f);
                JumpType = 0;
            }
            else
            {
                if (isLeft)
                {
                    PlayAnim("Landed L", PlayerAnimState.LandedL, 0.025f);
                    JumpType = 0;
                }
                else
                {
                    PlayAnim("Landed R", PlayerAnimState.LandedR, 0.025f);
                    JumpType = 0;
                }
            }

            return;
        }


        if (pressA && !pressD)
        {
            if (!isGrounded)
                return;

            if (!isLeft)
            {
                PlayAnim("Turn to L", PlayerAnimState.TurnToL, 0.1f);
                isLeft = true;
            }
            else
            {
                if (currentState == PlayerAnimState.IdleL)
                    PlayAnim("First Run L", PlayerAnimState.FirstRunL, 0.1f);
                else if (currentState == PlayerAnimState.StopL)
                    PlayAnim("First Run L", PlayerAnimState.FirstRunL, 0.1f);
                else if (currentState == PlayerAnimState.LandedL)
                    PlayAnim("First Run L", PlayerAnimState.FirstRunL, 0.1f);
                else if (currentState == PlayerAnimState.TurntoLandStop)
                    PlayAnim("First Run L", PlayerAnimState.FirstRunL, 0.1f);
                else if (IsAnimFinished("First Run L", stateInfo))
                    PlayAnim("Run L", PlayerAnimState.RunL, 0.0f);
                else if (IsAnimFinished("Landed and Move L", stateInfo))
                    PlayAnim("Run L", PlayerAnimState.RunL, 0.0f);
                else if (IsAnimFinished("Turn to L", stateInfo))
                    PlayAnim("Run L", PlayerAnimState.RunL, 0.0f);
                else if (IsAnimFinishedEarly("OverObstacle L", stateInfo, 0.425f))
                    PlayAnim("First Run L", PlayerAnimState.FirstRunL, 0.1f);

            }
        }
        else if (!pressA && pressD)
        {
            if (!isGrounded)
                return;

            if (isLeft)
            {
                PlayAnim("Turn to R", PlayerAnimState.TurnToR, 0.1f);
                isLeft = false;
            }
            else
            {
                if (currentState == PlayerAnimState.IdleR)
                    PlayAnim("First Run R", PlayerAnimState.FirstRunR, 0.1f);
                else if (currentState == PlayerAnimState.StopR)
                    PlayAnim("First Run R", PlayerAnimState.FirstRunR, 0.1f);
                else if (currentState == PlayerAnimState.LandedR)
                    PlayAnim("First Run R", PlayerAnimState.FirstRunR, 0.1f);
                else if (currentState == PlayerAnimState.TurntoRandStop)
                    PlayAnim("First Run R", PlayerAnimState.FirstRunR, 0.1f);
                else if (IsAnimFinished("First Run R", stateInfo))
                    PlayAnim("Run R", PlayerAnimState.RunR, 0.0f);
                else if (IsAnimFinished("Landed and Move R", stateInfo))
                    PlayAnim("Run R", PlayerAnimState.RunR, 0.0f);
                else if (IsAnimFinished("Turn to R", stateInfo))
                    PlayAnim("Run R", PlayerAnimState.RunR, 0.0f);
                else if (IsAnimFinishedEarly("OverObstacle R", stateInfo, 0.425f))
                    PlayAnim("First Run R", PlayerAnimState.FirstRunR, 0.1f);

            }
        }
        else if (!pressA && !pressD)
        {
            if (currentState == PlayerAnimState.RunL)
                PlayAnim("Stop L", PlayerAnimState.StopL, 0.1f);
            else if (currentState == PlayerAnimState.FirstRunR)
                PlayAnim("Stop R", PlayerAnimState.StopR, 0.1f);
            else if (currentState == PlayerAnimState.RunR)
                PlayAnim("Stop R", PlayerAnimState.StopR, 0.1f);
            else if (currentState == PlayerAnimState.LandedAndMoveR)
                PlayAnim("Stop R", PlayerAnimState.StopR, 0.1f);
            else if (currentState == PlayerAnimState.LandedAndMoveL)
                PlayAnim("Stop L", PlayerAnimState.StopL, 0.1f);

            else if (currentState == PlayerAnimState.TurnToL)
                CheckTurningPoint(stateInfo);
            else if (currentState == PlayerAnimState.TurnToR)
                CheckTurningPoint(stateInfo);

            else if (currentState == PlayerAnimState.FirstRunL)
                PlayAnim("Stop L", PlayerAnimState.StopL, 0.1f);
            else if (IsAnimFinished("Stop L", stateInfo))
                PlayAnim("Idle L", PlayerAnimState.IdleL, 0.1f);
            else if (IsAnimFinished("Stop R", stateInfo))
                PlayAnim("Idle R", PlayerAnimState.IdleR, 0.25f);
            else if (IsAnimFinished("Landed R", stateInfo))
                PlayAnim("Idle R", PlayerAnimState.IdleR, 0.25f);
            else if (IsAnimFinished("Landed L", stateInfo))
                PlayAnim("Idle L", PlayerAnimState.IdleL, 0.25f);
            else if (IsAnimFinished("OverObstacle R", stateInfo))
            {
                if (currentState == PlayerAnimState.OverObstacleR)
                    PlayAnim("Idle R", PlayerAnimState.IdleR, 0.25f);
            }
            else if (IsAnimFinished("OverObstacle L", stateInfo))
            {
                if (currentState == PlayerAnimState.OverObstacleL)
                    PlayAnim("Idle L", PlayerAnimState.IdleL, 0.25f);
            }
            else if (IsAnimFinished("Turn to R and Stop", stateInfo))
            {
                if (currentState == PlayerAnimState.TurntoRandStop)
                    PlayAnim("Idle R", PlayerAnimState.IdleR, 0.25f);
            }
            else if (IsAnimFinished("Turn to L and Stop", stateInfo))
            {
                if (currentState == PlayerAnimState.TurntoLandStop)
                    PlayAnim("Idle L", PlayerAnimState.IdleL, 0.25f);
            }
        }

       
    }

    // 공통 애니메이션 재생 함수
    void PlayAnim(string animName, PlayerAnimState nextState,float TransitionDuration)
    {
        if (currentState == nextState) return;

        anim.CrossFade(animName, TransitionDuration);
        currentState = nextState;
      
    }
    public void Convey(float TransitionDuration, bool leftSide)
    {
        if (leftSide)
        {
            PlayRootMotionAnim("OverObstacle R", PlayerAnimState.OverObstacleR, TransitionDuration);
        }
        else if(!leftSide)
        {
            PlayRootMotionAnim("OverObstacle L", PlayerAnimState.OverObstacleR, TransitionDuration);
        }
    }

    void PlayRootMotionAnim(string animName, PlayerAnimState nextState,float TransitionDuration)
    {
        canMove = false;
        transform.GetComponent<CapsuleCollider>().isTrigger = true;
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(0, 0, 0);
        if (currentState == nextState) return;

        anim.CrossFade(animName, TransitionDuration, 0, 0.00f);
        currentState = nextState;
    }

    bool IsAnimFinished(string animName, AnimatorStateInfo stateInfo)
    {
       
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 1.0f;
    }

    bool IsAnimFinishedEarly(string animName, AnimatorStateInfo stateInfo,float time)
    {
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= time;
    }


    bool IsGrounded()
    {
        if (Time.time - lastJumpTime < jumpIgnoreGroundTime)
            return false;
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.11f,LayerMask.GetMask("Ground"));
    }

    void AlwaysCheckGround()
    {
        if(!Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.11f, LayerMask.GetMask("Ground")))
        {
            if(isGrounded)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
        }
        else
        {
            isFalling = false;
        }


    }

    void CheckTurningPoint(AnimatorStateInfo stateInfo)
    {
        float startState =  stateInfo.normalizedTime *5/4 + 0.05f;

        if (isLeft)
        {
            if (stateInfo.IsName("Turn to L"))
            {
                
                if (stateInfo.normalizedTime > 0.3f)
                {
                    PlayAnim("Stop L", PlayerAnimState.StopL, 0.1f);
                    currentState = PlayerAnimState.StopL;
                }
                else
                {
                    anim.CrossFade("Turn to L and Stop", 0.15f, 0, startState);
                    currentState = PlayerAnimState.TurntoLandStop;
                }
            }
            else
            {
                anim.CrossFade("Turn to L and Stop", 0.05f, 0, 0.05f);
                currentState = PlayerAnimState.TurntoLandStop;
            }
        }
        else
        {
            if (stateInfo.IsName("Turn to R"))
            {
                if (stateInfo.normalizedTime > 0.3f)
                {
                    PlayAnim("Stop R", PlayerAnimState.StopR, 0.1f);
                    currentState = PlayerAnimState.StopR;
                }
                else
                {
                    anim.CrossFade("Turn to R and Stop", 0.15f, 0, startState);
                    currentState = PlayerAnimState.TurntoRandStop;
                }
            }
            else
            {
                anim.CrossFade("Turn to R and Stop", 0.05f, 0, 0.05f);
                currentState = PlayerAnimState.TurntoRandStop;
            }
        }

    }

    void Jump()
    {
        rb.AddForce(Vector3.up * 4.5f, ForceMode.Impulse);
        lastJumpTime = Time.time;
    }
    void FindObstacle()
    {

        Collider[] colliders = Physics.OverlapSphere(StepRayLower.transform.position, 1);

        GameObject closestObstacle = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObstacle = col.gameObject;
                }
            }
        }
        nearbyObstacle = closestObstacle;

        if (nearbyObstacle != null)
        {
           
        }
    }
    void CheckUntilCanMove(AnimatorStateInfo stateInfo)
    {
        
        if(currentState == PlayerAnimState.OverObstacleR)
        {
            if (IsAnimFinishedEarly("OverObstacle R", stateInfo, 0.425f))
            {

                /*
                canMove = true;
                transform.GetComponent<CapsuleCollider>().isTrigger = false;
                rb.useGravity = true;
                */
                
            }
        }
    }

   
    public void Blocked(bool LeftSide)
    {
        if (LeftSide)
        {
            if(!isLeft)
                PlayAnim("Blocked R", PlayerAnimState.BlockedL, 0.1f);
        }
        else
        {
            /*
            if(isLeft)
                PlayAnim("Blocked L", PlayerAnimState.BlockedR, 0.1f);
            */
        }
       
    }

    void CheckOutHoleFinished()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        if (!isOutholeFinished)
        {
            anim.applyRootMotion = true;
            if (stateInfo.IsName("OutHole") && stateInfo.normalizedTime >= 1f)
            {
                isOutholeFinished = true;
                anim.applyRootMotion = false;
                transform.GetComponent<CapsuleCollider>().isTrigger = false;
                rb.useGravity = true;
                Debug.Log("Outhole 끝 루트모션 꺼짐");
            }
           
            return; // outhole 끝나기 전엔 다른 로직 무시
        }

    }

    
}
