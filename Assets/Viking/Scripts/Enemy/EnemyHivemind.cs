using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    class EnemyHivemind : Singleton<EnemyHivemind>
    {
        #region Inspector
        [Header("Settings")]
        [SerializeField]
        private int updatePathEveryFrames = 3;
        #endregion

        protected override void SingletonAwake()
        {
            
        }
        protected override void SingletonDestroy()
        {
           
        }

        [HideInInspector]
        public List<EnemyController> enemies = new List<EnemyController>();
        private int lastEnemyIndex = 0;
        private int currentFrame = 0;
        private void Update()
        {
            currentFrame++;
            if(currentFrame >= updatePathEveryFrames)
            {
                EnemyController current = GetEnemyToSetPath();
                if (current == null)
                    return;

                foreach (EnemyController enemy in enemies)
                {
                    if (current == enemy)
                        continue;
                    enemy.Agent.enabled = false;
                    enemy.Obstacle.enabled = true;
                }
                // doing like this cz i have no idea how to force update navmesh
                // other then using third party thing or making my own
                StartCoroutine(UpdateDestinationNextFrame(current));

                currentFrame = 0;
            }
        }
        private void LateUpdate()
        {
            foreach (EnemyController enemy in enemies)
            {
                enemy.ManualUpdate();
            }
        }

        private EnemyController GetEnemyToSetPath()
        {
            int maxIndex = enemies.Count - 1;
            lastEnemyIndex = Mathf.Min(lastEnemyIndex, maxIndex);

            // doing this instead of while(true) to prevent potential softlock
            for(int i = -1; i < maxIndex; i++)
            {
                EnemyController current = enemies[lastEnemyIndex];

                lastEnemyIndex++;
                if (lastEnemyIndex > maxIndex)
                    lastEnemyIndex = 0;

                if (current.ShouldRecalculatePath())
                {
                    return current; 
                }
            }
            return null;
        }

        #region Coroutines
        private IEnumerator UpdateDestinationNextFrame(EnemyController current)
        {
            yield return null;
            current.SetDestination(PlayerController.Instance.transform);
            foreach (EnemyController enemy in enemies)
            {
                if (current == enemy)
                    continue;
                enemy.Agent.enabled = true;
                enemy.Obstacle.enabled = false;
            }
        }
        #endregion
    }
}
