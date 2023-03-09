using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Viking
{
    public class EnemyUI : LookAtCamera
    {
        #region Inspector
        [SerializeField]
        private Image healthbar;
        #endregion

        public void SetHealthbarPerc(float perc)
        {
            perc = Mathf.Clamp(perc, 0.0f, 1.0f);
            healthbar.fillAmount = perc;
        }

        public void EnableCanvas()
        {
            gameObject.SetActive(true);
        }
        public void DisableCanvas()
        {
            gameObject.SetActive(false);
        }
    }

}

