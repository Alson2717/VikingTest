using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class PlayerController : Singleton<PlayerController>
    {
        #region Inspector
        [Header("Components")]
        [SerializeField]
        private PlayerAnimator animator;
        [SerializeField]
        private new Rigidbody rigidbody;

        [Header("Animations")]
        [SerializeField]
        private string runName = "Run";
        [SerializeField]
        private string attackName = "Attack";
        [SerializeField]
        private string damagedName = "Damage";
        [SerializeField]
        private string deathName = "Dead";

        [Header("Input")]
        [SerializeField]
        private KeyCode key_forward = KeyCode.W;
        [SerializeField]
        private KeyCode key_backwards = KeyCode.S;
        [SerializeField]
        private KeyCode key_right = KeyCode.D;
        [SerializeField]
        private KeyCode key_left = KeyCode.A;

        [Header("Settings")]
        [SerializeField]
        private float rotationSpeed = 0.25f;
        [SerializeField]
        private float minSprintSpeed = 1.0f;
        [SerializeField]
        private float maxSprintSpeed = 1.5f;
        #endregion

        #region Animations
        private int runID;
        private int attackID;
        private int damagedID;
        private int deathID;
        #endregion

        private Vector3 prevGlobalDirection = new Vector3();
        private bool doneRotating = true;
        private Vector3 accMovement = new Vector3();

        private bool isAttacking = false;
        private bool isDamaged = false;
        private bool isDead = false;
        protected override void SingletonAwake()
        {
            runID = Animator.StringToHash(runName);
            attackID = Animator.StringToHash(attackName);
            damagedID = Animator.StringToHash(damagedName);
            deathID = Animator.StringToHash(deathName);
        }
        protected override void SingletonDestroy()
        {
            
        }

        private void Update()
        {
            if (IgnoreInput())
                return;

            HandleMovementInput();
            HandleOtherInput();
        }
        private void FixedUpdate()
        {
            rigidbody.MovePosition(rigidbody.position + accMovement);
            accMovement = new Vector3();
        }

        private void HandleMovementInput()
        {
            if (!CanMove())
            {
                animator.animator.SetBool(runID, false);
                doneRotating = false;
                return;
            }

            Vector3 initial = new Vector3();
            Vector3 direction = initial;

            if (Input.GetKey(key_forward))
            {
                direction += new Vector3(0.0f, 0.0f, 1.0f);
            }
            if (Input.GetKey(key_backwards))
            {
                direction += new Vector3(0.0f, 0.0f, -1.0f);
            }
            if(Input.GetKey(key_right))
            {
                direction += new Vector3(1.0f, 0.0f, 0.0f);
            } 
            if (Input.GetKey(key_left))
            {
                direction += new Vector3(-1.0f, 0.0f, 0.0f);
            }

            if(direction != initial)
            {
                animator.animator.SetBool(runID, true);

                if (direction != prevGlobalDirection)
                {
                    prevGlobalDirection = direction;
                    doneRotating = false;
                }

                direction = CameraController.Instance.transform.rotation * direction;

                Vector3 currentForward = transform.forward;

                direction.y = 0.0f;
                direction.Normalize();

                if(!doneRotating)
                {
                    Vector3 temp = direction;
                    direction = Vector3.Slerp(currentForward, direction, rotationSpeed * Time.deltaTime);

                    if (direction.ApproximateEquals(temp))
                        doneRotating = true;
                }

                transform.forward = direction;
            }
            else
            {
                animator.animator.SetBool(runID, false);
                doneRotating = false;
            }
        }
        private void HandleOtherInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(CanAttack())
                {
                    animator.animator.SetTrigger(attackID);
                }
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(1);
            }
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Die();
            }
        }

        private bool IgnoreInput()
        {
            return isDead;
        }
        private bool CanAttack()
        {
            return !isAttacking && !isDamaged;
        }
        private bool CanMove()
        {
            return true;
        }

        public void TakeDamage(int amount)
        {
            animator.animator.SetTrigger(damagedID);
        }
        public void Die()
        {
            animator.animator.SetLayerWeight(1, 0.0f);
            animator.animator.SetBool(deathID, true);
        }

        #region Animation
        public void Animation_SetAttackState()
        {
            isAttacking = true;
        }
        public void Animation_ExitAttackState()
        {
            isAttacking = false;
        }
        public void Animation_SetDamagedState()
        {
            isDamaged = true;
        }
        public void Animation_ExitDamagedState()
        {
            isDamaged = false;
        }
        public void Animation_SetDeathState()
        {
            isDead = true;
        }
        public void Animation_ExitDeathState()
        {
            isDead = false;
        }
        #endregion

        public void OnPlayerAnimatorMove()
        {
            accMovement += animator.animator.deltaPosition;
        }
    }
}
