using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamStateBehaviour : StateMachineBehaviour
{
    public SetParamStateData[] ParamStateData;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(SetParamStateData data in ParamStateData)
        {
            animator.SetBool(data.paramName, data.setDefaultState);
        }
    }

    [Serializable]
    public struct SetParamStateData
    {
        public string paramName;
        public bool setDefaultState;
    }
}
