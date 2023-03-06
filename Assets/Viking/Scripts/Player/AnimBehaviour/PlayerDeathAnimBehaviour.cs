using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viking
{
    public class PlayerDeathAnimBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_SetDeathState();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_ExitDeathState();
        }
    }
}