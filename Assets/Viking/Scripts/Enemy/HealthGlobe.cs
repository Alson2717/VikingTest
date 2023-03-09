using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class HealthGlobe : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private int health = 1;
        #endregion

        public void ResetSelf()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null)
                return;

            player.Heal(1);
            gameObject.SetActive(false);

            EnemyHivemind.Instance.healthGlobesPool.Add(this);
        }
    }
}
