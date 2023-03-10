using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Viking
{
    public class GameController : Singleton<GameController>
    {
        #region Inspector
        [Header("Stuff")]
        [SerializeField]
        private Terrain terrain;
        [SerializeField]
        private Canvas startingCanvas;
        [SerializeField]
        private Canvas playerCanvas;
        [SerializeField]
        private Canvas endGameCanvas;
        [SerializeField]
        private TextMeshProUGUI endGameScoreText;
        [SerializeField]
        private CanvasGroup blackScreen;

        [Header("Settings")]
        [SerializeField]
        private string terrainLayerName = "Terrain";
        [SerializeField]
        private string enemyHitboxLayerName = "EnemyHitbox";
        [SerializeField]
        private string playerHitboxLayerName = "PlayerHitbox";
        [SerializeField]
        private float blackScreenTime = 0.2f;
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

        private CoroutineBehaviourHandler blackScreenCoroutine;
        private static bool wasReset = false;
        protected override void SingletonAwake()
        {
            ShowPlayerUI();

            TerrainLayer = LayerMask.GetMask(terrainLayerName);
            EnemyHitboxLayer = LayerMask.GetMask(enemyHitboxLayerName);
            PlayerHitboxLayer = LayerMask.GetMask(playerHitboxLayerName);

            blackScreenCoroutine = new CoroutineBehaviourHandler(this);
        }
        protected override void SingletonDestroy()
        {
            
        }
        private void Start()
        {
            PlayerController.Instance.DisableSelf();

            ShowStartGameUI();
            if(wasReset)
            {
                Callback_SetBlackScreenAlpha(1.0f);
                blackScreenCoroutine.Start(BlackScreenHideCoroutine(blackScreenTime));
                wasReset = false;
            }
        }

        public void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        public void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public bool IsReseting()
        {
            return wasReset;
        }

        public void ShowStartGameUI()
        {
            ShowCursor();

            playerCanvas.gameObject.SetActive(false);
            endGameCanvas.gameObject.SetActive(false);
            startingCanvas.gameObject.SetActive(true);
        }
        public void ShowPlayerUI()
        {
            HideCursor();

            startingCanvas.gameObject.SetActive(false);
            endGameCanvas.gameObject.SetActive(false);
            playerCanvas.gameObject.SetActive(true);
        }
        public void ShowEndGameUI(int score)
        {
            ShowCursor();

            endGameScoreText.text = score.ToString();

            startingCanvas.gameObject.SetActive(false);
            playerCanvas.gameObject.SetActive(false);
            endGameCanvas.gameObject.SetActive(true);
        }

        public void Button_StartGame()
        {
            blackScreenCoroutine.Start(StartGameCoroutine(blackScreenTime));
        }
        public void Button_ResetGame()
        {
            if (IsReseting())
                return;
            blackScreenCoroutine.Start(ResetGameCoroutine(blackScreenTime));
        }
        public void Button_ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Callback_SetBlackScreenAlpha(float alpha)
        {
            blackScreen.alpha = alpha;
        }

#region Coroutines
        private IEnumerator BlackScreenShowCoroutine(float time)
        {
            blackScreen.gameObject.SetActive(true);
            yield return this.Lerp(time, blackScreen.alpha, 1.0f, Mathf.Lerp, Callback_SetBlackScreenAlpha);
        }
        private IEnumerator BlackScreenHideCoroutine(float time)
        {
            blackScreen.gameObject.SetActive(true);
            yield return this.Lerp(time, blackScreen.alpha, 0.0f, Mathf.Lerp, Callback_SetBlackScreenAlpha);
            blackScreen.gameObject.SetActive(false);
        }

        private IEnumerator StartGameCoroutine(float blackScreenTime)
        {
            blackScreen.alpha = 0.0f;
            yield return BlackScreenShowCoroutine(blackScreenTime);

            ShowPlayerUI();
            CameraController.Instance.SwitchToPlayerFollowing();
            PlayerController.Instance.EnableSelf();
            EnemyHivemind.Instance.StartGame();

            yield return BlackScreenHideCoroutine(blackScreenTime);
        }
        private IEnumerator ResetGameCoroutine(float blackScreenTime)
        {
            wasReset = true;
            yield return BlackScreenShowCoroutine(blackScreenTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
#endregion
    }
}
