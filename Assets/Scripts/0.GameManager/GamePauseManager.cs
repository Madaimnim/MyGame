using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    #region 公開定義
    public static GamePauseManager Instance;
    public bool Pause = false;
    #endregion
    #region 私有定義
    private bool isPaused = false;
    #endregion

    #region Awake()方法
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("⚠️ 嘗試創建第二個 GamePauseManager，將其刪除");
            Destroy(gameObject);
        }
    }
    #endregion

    private void Update() {
        if (Pause)
            isPaused = true;
        else
            isPaused = false;
    }

    #region 公開方法PauseGame()
    public void PauseGame() {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;  // 🔧【新增】暫停遊戲
            AudioListener.pause = true;  // 🔧【新增】靜音遊戲音效
        }
    }
    #endregion
    #region 公開方法ResumeGame() 
    public void ResumeGame() {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }


    #endregion
}
