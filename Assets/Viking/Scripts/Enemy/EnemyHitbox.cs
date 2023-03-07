using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class EnemyHitbox : MonoBehaviour 
    {
        #region Inspector
        [SerializeField]
        private EnemyController linkedEnemy;
        #endregion

        public EnemyController LinkedEnemy
        {
            get { return linkedEnemy; }
        }

        public bool ShouldRegisterHit()
        {
            return linkedEnemy.CanReceiveHit();
        }

        #region Editor
        private void Reset()
        {
            linkedEnemy = GetComponentInParent<EnemyController>();
        }
        #endregion
    }
}
