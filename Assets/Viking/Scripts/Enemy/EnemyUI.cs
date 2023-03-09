using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Viking
{
    // world space canvases suck when
    // there are a lot of them cz they are not being batched
    // better to use sprites, using world canvases because
    // they are easy to setup with healthbar i was given
    public class EnemyUI : LookAtCamera
    {
        #region Inspector
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Image healthbar;
        #endregion

        public Canvas Canvas
        {
            get { return canvas; }
        }

        public void SetHealthbarPerc(float perc)
        {
            perc = Mathf.Clamp(perc, 0.0f, 1.0f);
            healthbar.fillAmount = perc;
        }

        public void EnableSelf()
        {
            gameObject.SetActive(true);
        }
        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }

}

