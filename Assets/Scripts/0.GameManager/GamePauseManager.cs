using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    //變數
    #region 
    public static GamePauseManager Instance;

    private bool isPaused = false;
    #endregion
    //生命週期
    #region Awake()方法
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
  
    private void Update() {

    }
    #endregion

    #region 公開方法PauseGame()
    public void PauseGame() {
        if (!isPaused)
        {
            isPaused = true;
            TextPopupManager.Instance.TextPrefab_Resume.SetActive(true);
            Time.timeScale = 0;  // 暫停遊戲
            AudioListener.pause = true;  // 靜音遊戲音效
        }
    }
    #endregion
    #region 公開方法ResumeGame() 
    public void ResumeGame() {
        if (isPaused)
        {
            TextPopupManager.Instance.TextPrefab_Resume.SetActive(false);
            isPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }


    #endregion
}
