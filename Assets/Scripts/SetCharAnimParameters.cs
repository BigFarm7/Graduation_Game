using UnityEngine;

public class SetCharAnimParameters : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void OverGoThreshold()
    {
        anim.SetTrigger("goTrigger");
    }

    public void OverGoThresholdTurn()
    {
        anim.SetBool("goThresh", true);
        anim.SetBool("isChange",false);
    }

    public void TurnTrigger()
    {
        anim.SetTrigger("turnTrigger");
    }

    public void GoRight()
    {
        anim.Play("GoRight");
    }
    public void Acceling()
    {
        anim.SetBool("isAccel", true);
    }

    public void GoIdle()
    {
        anim.Play("Idle");
    }

    public void ResetParam()
    {
        anim.SetBool("isAccel", false);
        anim.SetBool("goIdle", false);
        anim.ResetTrigger("FirstGoTrigger");
        anim.ResetTrigger("goTrigger");
    }

    public void Go()
    {
        anim.Play("Go");
    }

    public void IdleRight()
    {
        anim.CrossFade("IdleRight", 0.1f);
    }
   
}
