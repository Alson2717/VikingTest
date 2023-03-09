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
        private NavMeshObstacle obstacle;
        [SerializeField]
        private EnemyAnimator animator;
        [SerializeField]
        private Collider solidCollider;
        [SerializeField]
        private SphereCollider attackCollider;
        [SerializeField]
        private EnemyUI ui;

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
        [SerializeField]
        private int startingMaxHealth = 1;
        [SerializeField]
        private int healthIncrease = 1;
        #endregion

        #region Animation
        private int runID;
        private int damagedID;
        private int attackID;
        private int deathID;
        #endregion

        public NavMeshAgent Agent
        {
            get { return this.agent; }
        }
        public NavMeshObstacle Obstacle
        {
            get { return this.obstacle; }
        }

        private int maxHealth = 1;
        private int health;

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

            maxHealth = startingMaxHealth;
            health = startingMaxHealth;
            ui.SetHealthbarPerc(CalcHealthPerc());
        }
        private void Start()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;

            EnemyHivemind.Instance.enemies.Add(this);

            SetDestination(PlayerController.Instance.transform);
        }

        public void ManualUpdate()
        {
            PlayerController player = PlayerController.Instance;
            if (player.IsDead())
            {
                animator.animator.SetBool(runID, false);
                return;
            }

            if (checkForHits)
            {
                CheckForHits();
            }

            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;

            if (CanAttack())
            {
                Vector3 targetPosition = player.transform.position;
                Vector3 dir = targetPosition - currentPosition;
                Vector3 dirn = dir.normalized;

                float distance = Vector3.Distance(currentPosition, targetPosition);

                if (distance <= attackRadius)
                {
                    Vector3 forward = transform.forward;
                    float dot = Vector3.Dot(dirn, forward);

                    if (dot < attackDot)
                    {
                        animator.animator.SetBool(runID, true);
                        transform.rotation = GetSmoothRotation(currentRotation, dir);
                    }
                    else
                    {
                        Attack();
                    }
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
            if (direction.ApproximateEquals(new Vector3(0.0f, 0.0f, 0.0f)))
                return current;
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

        public bool ShouldRecalculatePath()
        {
            return !IsDead() && !isAttacking;
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
            ui.SetHealthbarPerc(CalcHealthPerc());
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
            PlayerUI.Instance.IncreaseScore(1);
            animator.animator.SetBool(deathID, true);
        }

        public float CalcHealthPerc()
        {
            return (float)health / (float)maxHealth;
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
            bool agentState = agent.enabled;
            agent.enabled = true;
            agent.CalculatePath(position, path);
            agent.enabled = agentState;

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

            agent.enabled = false;
            obstacle.enabled = true;
        }
        public void Animation_ExitAttackState()
        {
            isAttacking = false;
            AnimationEvent_StopCheckingForHits();

            agent.enabled = true;
            obstacle.enabled = false;
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

            ui.DisableCanvas();
        }
        public void Animation_ExitDeadState()
        {
            isDead = false;
            solidCollider.enabled = true;

            ui.EnableCanvas();
        }
        #endregion
    }
}
