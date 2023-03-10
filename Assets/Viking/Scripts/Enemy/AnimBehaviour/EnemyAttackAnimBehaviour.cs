using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viking
{
    public class EnemyAttackAnimBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnemyController enemy = animator.GetComponentInParent<EnemyController>();
            enemy.Animation_SetAttackState();
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnemyController enemy = animator.GetComponentInParent<EnemyController>();
            enemy.Animation_ExitAttackState();
        }
    }

}