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
       
        #endregion

        public int TerrainLayer
        {
            get;
            private set;
        }

        protected override void SingletonAwake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            TerrainLayer = LayerMask.GetMask("Terrain");
        }
        protected override void SingletonDestroy()
        {
            
        }

    }
}
