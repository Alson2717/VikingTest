using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Viking
{
    public class EnemyController : MonoBehaviour
    {
        #region Inspector
        [Header("Components")]
        [SerializeField]
        private NavMeshAgent agent;
        [SerializeField]
        private EnemyAnimator animator;
        [SerializeField]
        private Collider solidCollider;
        [SerializeField]
        private SphereCollider attackCollider;

        [Header("Animations")]
        [SerializeField]
        private string runName = "Run";
        [SerializeField]
        private string damagedName = "Damaged";
        [SerializeField]
        private string attackName = "Attack";
        [SerializeField]
        private string deathName = "Dead";

        [Header("Settings")]
        [SerializeField]
        private float speed = 3.0f;
        [SerializeField]
        private float rotationSpeed = 0.25f;
        [SerializeField]
        private float attackRadius = 1.0f;
        [SerializeField]
        private float attackDot = 0.5f;
        #endregion

        #region Animation
        private int runID;
        private int damagedID;
        private int attackID;
        private int deathID;
        #endregion

        private int health = 5;

        private bool isAttacking = false;
        private bool isDamaged = false;
        private bool isDead = false;

        private Vector3[] currentPath;
        private int currentPathIndex = -1;
        private Vector3 currentPathPosition;

        private int frame = 0;

        private bool checkForHits = false;
        private bool hitPlayer = false;
        private void Awake()
        {
            runID = Animator.StringToHash(runName);
            damagedID = Animator.StringToHash(damagedName);
            attackID = Animator.StringToHash(attackName);
            deathID = Animator.StringToHash(deathName);
        }
        private void Start()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;

            SetDestination(PlayerController.Instance.transform);
        }

        private void Update()
        {
            if(checkForHits)
            {
                CheckForHits();
            }

            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;

            if (CanAttack())
            {
                Vector3 targetPosition = PlayerController.Instance.transform.position;
                Vector3 dir = targetPosition - currentPosition;
                Vector3 dirn = dir.normalized;

                Vector3 forward = transform.forward;
                float dot = Vector3.Dot(dirn, forward);

                if (dot < attackDot)
                {
                    animator.animator.SetBool(runID, true);
                    transform.rotation = GetSmoothRotation(currentRotation, dir);
                    return;
                }

                float distance = Vector3.Distance(currentPosition, targetPosition);

                if (distance <= attackRadius)
                {
                    Attack();
                    return;
                }

            }

            if (!CanMove() || IsPathDone())
            {
                animator.animator.SetBool(runID, false);
                return;
            }

            currentPosition.y = 0.0f;
            Vector3 targetPathPosition = currentPathPosition;
            targetPathPosition.y = currentPosition.y;

            frame++;
            if (frame == 10)
            {
                // recalculate path from time to time
                SetDestination(PlayerController.Instance.transform);
                frame = 0;
            }

            animator.animator.SetBool(runID, true);

            Vector3 nextPosition = Vector3.MoveTowards(currentPosition, targetPathPosition, speed * Time.deltaTime);

            Terrain terrain = GameController.Instance.Terrain;
            nextPosition.y = terrain.SampleHeight(nextPosition) + terrain.GetPosition().y;

            Quaternion nextRotation = GetSmoothRotation(currentRotation, targetPathPosition - currentPosition);
            transform.SetPositionAndRotation(nextPosition, nextRotation);
            agent.nextPosition = nextPosition;

            if (nextPosition.ApproximateEqualsIgnoreY(currentPathPosition))
            {
                GetNextPathPosition();
            }
        }

        private void GetNextPathPosition()
        {
            if (currentPath == null)
                return;

            if (currentPathIndex < currentPath.Length)
            {
                currentPathPosition = currentPath[currentPathIndex];
            }
            currentPathIndex++;

        }
        private bool IsPathDone()
        {
            return currentPath == null || currentPathIndex < 0 || currentPathIndex > currentPath.Length;
        }

        private Quaternion GetSmoothRotation(Quaternion current, Vector3 direction)
        {
            Quaternion target = Quaternion.LookRotation(direction);
            return Quaternion.Slerp(current, target, rotationSpeed * Time.deltaTime);
        }

        private void CheckForHits()
        {
            Vector3 position = attackCollider.bounds.center;
            float radius = attackCollider.radius;

            Collider[] colliders = Physics.OverlapSphere(position, radius,
                GameController.Instance.PlayerHitboxLayer);
            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.name, collider.gameObject);

                PlayerHitbox hitbox = collider.GetComponent<PlayerHitbox>();
                if(hitbox != null && hitbox.ShouldRegisterHit())
                {
                    PlayerController.Instance.ReceiveHit(1);
                    hitPlayer = true;
                    AnimationEvent_StopCheckingForHits();
                    break;
                }
            }
        }

        public bool CanMove()
        {
            return !isDead && !isAttacking && !isDamaged;
        }
        public bool CanReceiveHit()
        {
            return !IsDead();
        }
        public bool CanAttack()
        {
            return !isAttacking && !isDamaged && !isDead;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void Attack()
        {
            animator.animator.SetTrigger(attackID);
        }
        public void ReceiveHit(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                animator.animator.SetTrigger(damagedID);
            }
        }
        public void Die()
        {
            animator.animator.SetBool(deathID, true);
        }

        #region Animation Event
        public void AnimationEvent_StartCheckingForHits()
        {
            hitPlayer = false;
            checkForHits = true;
        }
        public void AnimationEvent_StopCheckingForHits()
        {
            checkForHits = false;
        }
        #endregion

        public void SetDestination(Transform target)
        {
            currentPathIndex = 0;

            Vector3 position = target.position;
            Terrain terrain = GameController.Instance.Terrain;
            position.y = terrain.GetPosition().y + terrain.SampleHeight(position);

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(position, path);

            if(path.status == NavMeshPathStatus.PathComplete)
            {
                currentPath = path.corners;
                GetNextPathPosition();

                if (transform.position.ApproximateEqualsIgnoreY(currentPathPosition))
                    GetNextPathPosition();
            }
        }

        #region Animation
        public void Animation_SetAttackState()
        {
            isAttacking = true;
        }
        public void Animation_ExitAttackState()
        {
            isAttacking = false;
            AnimationEvent_StopCheckingForHits();
        }

        public void Animation_SetDamagedState()
        {
            isDamaged = true;
        }
        public void Animation_ExitDamagedState()
        {
            isDamaged = false;
        }

        public void Animation_SetDeadState()
        {
            isDead = true;
            solidCollider.enabled = false;
        }
        public void Animation_ExitDeadState()
        {
            isDead = false;
            solidCollider.enabled = true;
        }
        #endregion
    }
}
