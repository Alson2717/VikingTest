using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class GameController : Singleton<GameController>
    {
        #region Inspector
        [Header("Stuff")]
        [SerializeField]
        private Terrain terrain;

        [Header("Settings")]
        [SerializeField]
        private string terrainLayerName = "Terrain";
        [SerializeField]
        private string enemyHitboxLayerName = "EnemyHitbox";
        [SerializeField]
        private string playerHitboxLayerName = "PlayerHitbox";
        #endregion

        public Terrain Terrain
        {
            get { return terrain; }
        }

        public int TerrainLayer
        {
            get;
            private set;
        }
        public int EnemyHitboxLayer
        {
            get;
            private set;
        }
        public int PlayerHitboxLayer
        {
            get;
            private set;
        }

        protected override void SingletonAwake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            TerrainLayer = LayerMask.GetMask(terrainLayerName);
            EnemyHitboxLayer = LayerMask.GetMask(enemyHitboxLayerName);
            PlayerHitboxLayer = LayerMask.GetMask(playerHitboxLayerName);
        }
        protected override void SingletonDestroy()
        {
            
        }

    }
}
