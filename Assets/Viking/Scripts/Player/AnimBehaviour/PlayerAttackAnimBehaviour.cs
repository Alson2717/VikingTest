using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viking
{
    public class PlayerAttackAnimBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_SetAttackState();
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_ExitAttackState();
            PlayerController.Instance.AnimationEvent_StopCheckingForHits();
        }
    }

}