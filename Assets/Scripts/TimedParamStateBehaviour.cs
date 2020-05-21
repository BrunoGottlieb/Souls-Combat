using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TimedParamStateBehaviour : StateMachineBehaviour
{
    public string paramName;
    public bool setDefaultValue;

    public float start, end;

    private bool waitForExit;
    private bool OnTransitionExitTriggered;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        waitForExit = false;
        OnTransitionExitTriggered = false;
        animator.SetBool(paramName, !setDefaultValue); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*if (CheckOnTransitionExit(animator, layerIndex))
        {
            OnTransitionExit(animator);
        }
        if(!OnTransitionExitTriggered &&stateInfo.normalizedTime >= start && stateInfo.normalizedTime <= end)
        {
            animator.SetBool(paramName, setDefaultValue);
        }*/

        if(stateInfo.normalizedTime >= end)
        {
            Debug.Log(stateInfo.normalizedTime);
            animator.SetBool(paramName, !setDefaultValue);
        }
    }

    /*
    private void OnTransitionExit(Animator animator)
    {
        animator.SetBool(paramName, !setDefaultValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!OnTransitionExitTriggered)
        {
            animator.SetBool(paramName, !setDefaultValue);
        }
    }

    private bool CheckOnTransitionExit(Animator animator, int layerIndex)
    {
        if(!waitForExit && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0)
        {
            waitForExit = true;
        }
        if(!OnTransitionExitTriggered && waitForExit && animator.IsInTransition(layerIndex))
        {
            OnTransitionExitTriggered = true;
            return true;
        }
        return false;
    }*/

}
