using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Viking
{
    public class PlayerUI : Singleton<PlayerUI>
    {
        #region Inspector
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private TextMeshProUGUI currentScoreText;
        #endregion

        private int currentScore = 0;

        protected override void SingletonAwake()
        {
            
        }
        protected override void SingletonDestroy()
        {
            
        }
        private void Start()
        {
            UpdateScoreText();
        }

        public void IncreaseScore(int amount)
        {
            currentScore++;
            UpdateScoreText();
        }
        public int GetScore()
        {
            return currentScore;
        }

        public void UpdateScoreText()
        {
            currentScoreText.text = currentScore.ToString();
        }

        public void SetHealthBarPercentage(float perc)
        {
            perc = Mathf.Clamp(perc, 0.0f, 1.0f);
            healthBar.fillAmount = perc;
        }
    }
}
