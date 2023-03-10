using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viking
{
    public class PlayerDamagedAnimBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_SetDamagedState();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.Animation_ExitDamagedState();
        }
    }
}