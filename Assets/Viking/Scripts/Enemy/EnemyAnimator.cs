using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class EnemyAnimator : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private EnemyController linkedEnemy;
        [SerializeField]
        private Animator _animator;
        #endregion

        public EnemyController LinkedEnemy
        {
            get { return linkedEnemy; }
        }
        public Animator animator
        {
            get { return _animator; }
        }

        public void AnimationEvent_StartCheckingForHits()
        {
            linkedEnemy.AnimationEvent_StartCheckingForHits();
        }
        public void AnimationEvent_StopCheckingForHits()
        {
            linkedEnemy.AnimationEvent_StopCheckingForHits();
        }
    }
}
