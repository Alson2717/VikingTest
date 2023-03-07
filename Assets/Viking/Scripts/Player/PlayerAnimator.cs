using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    // this exists cz using OnAnimatorMove deltaPosition
    // in parent makes it go bananas for some reason,
    // much easier to just split OnAnimatorMove to object
    // that actually has animator attached
    public class PlayerAnimator : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private Animator _animator;
        #endregion

        // lower case name cz it makes it much easier 
        // to copy paste things without the need to 
        // change first letter to lower/upper case
        public Animator animator
        {
            get { return _animator; }
        }

        private void OnAnimatorMove()
        {
            PlayerController.Instance.OnPlayerAnimatorMove();
        }

        public void AnimationEvent_StartCheckingForHits()
        {
            PlayerController.Instance.AnimationEvent_StartCheckingForHits();
        }
        public void AnimationEvent_StopCheckingForHits()
        {
            PlayerController.Instance.AnimationEvent_StopCheckingForHits();
        }

        #region Editor
        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }
        #endregion
    }
}
