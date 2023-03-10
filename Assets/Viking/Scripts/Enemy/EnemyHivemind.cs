using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viking
{
    class EnemyHivemind : Singleton<EnemyHivemind>
    {
        #region Inspector
        [Header("Things")]
        [SerializeField]
        private EnemyController enemyPrefab;
        [SerializeField]
        private HealthGlobe healthGlobePrefab;
        [SerializeField]
        private Transform[] spawnPoints;
        [SerializeField]
        private Transform testPoint;

        [Header("Settings")]
        [SerializeField]
        private int updatePathEveryFrames = 3;
        [SerializeField]
        private float spawnRadius = 5.0f;
        [SerializeField]
        private int startingAmountOfEnemies = 10;
        [SerializeField]
        private float startingSpawnDelay = 2.0f;
        [SerializeField]
        private int healthGlobeChance = 10;
        #endregion

        [HideInInspector]
        public List<EnemyController> enemies = new List<EnemyController>();
        private int lastEnemyIndex = 0;
        private int currentFrame = 0;

        [HideInInspector]
        public Pool<EnemyController> enemyPool = new Pool<EnemyController>();
        [HideInInspector]
        public Pool<HealthGlobe> healthGlobesPool = new Pool<HealthGlobe>();

        private bool gameBegan = false;
        protected override void SingletonAwake()
        {
            
        }
        protected override void SingletonDestroy()
        {
           
        }
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

                Transform target = null;
                if (gameBegan)
                    target = PlayerController.Instance.transform;
                else
                    target = spawnPoints.Random();
                // doing like this cz i have no idea how to force update navmesh
                // other then using third party thing or making my own
                StartCoroutine(UpdateDestinationNextFrame(current, target));

                currentFrame = 0;
            }
        }
        private void LateUpdate()
        {
            foreach (EnemyController enemy in enemies)
            {
                if(enemy.ShouldCallUpdate())
                    enemy.ManualUpdate(!gameBegan);
            }
        }

        public void StartGame()
        {
            foreach (EnemyController enemy in enemies)
            {
                enemy.DisableAndAddToPool();
            }

            this.StopAllCoroutines();

            StartCoroutine(SpawnEnemiesCoroutine(startingSpawnDelay, startingAmountOfEnemies));
            gameBegan = true;
        }

        public void SpawnEnemy(int extraHealth)
        {
            EnemyController enemy = enemyPool.ExtractFirst(enemyPrefab);

            Terrain terrain = GameController.Instance.Terrain;

            Vector3 spawnPosition = new Vector3();
            Vector3 playerPosition = PlayerController.Instance.transform.position;
            // softlock prevention
            for (int i = 0; i < 100; i++)
            {
                spawnPosition = spawnPoints.Random().position;

                float terrainY = terrain.SampleHeight(spawnPosition) + terrain.GetPosition().y;

                // ignore y
                playerPosition.y = terrainY;
                spawnPosition.y = terrainY;

                float distance = Vector3.Distance(playerPosition, spawnPosition);
                if (distance > spawnRadius)
                    break;
            }

            //spawnPosition = testPoint.position;
            //spawnPosition.y = terrain.SampleHeight(spawnPosition) + terrain.GetPosition().y;
            enemy.transform.position = spawnPosition;

            enemy.ResetSelf(extraHealth);
        }
        public void TrySpawnHealthGlobe(Vector3 position)
        {
            if (UnityEngine.Random.Range(1, 101) > healthGlobeChance)
                return;

            HealthGlobe healthGlobe = healthGlobesPool.ExtractFirst(healthGlobePrefab);
            healthGlobe.transform.position = position;
            healthGlobe.ResetSelf();
        }

        private EnemyController GetEnemyToSetPath()
        {
            int maxIndex = enemies.Count - 1;
            if (maxIndex < 0)
                return null;
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
        private IEnumerator SpawnEnemiesCoroutine(float delay, int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                SpawnEnemy(0);
                yield return new WaitForSeconds(delay);
            }
        }
        private IEnumerator UpdateDestinationNextFrame(EnemyController current, Transform target)
        {
            yield return null;
            current.SetDestination(target);
            foreach (EnemyController enemy in enemies)
            {
                if (current == enemy)
                    continue;
                enemy.Obstacle.enabled = false;
                enemy.Agent.enabled = true;
            }
        }
        #endregion
    }
}
