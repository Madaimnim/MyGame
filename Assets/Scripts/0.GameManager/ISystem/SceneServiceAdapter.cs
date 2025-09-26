

public class SceneServiceAdapter : ISceneService
{
    public void LoadGameStart() => GameSceneManager.Instance.LoadSceneGameStart();
    public void LoadPreparation() => GameSceneManager.Instance.LoadScenePreparation();
    public System.Collections.IEnumerator LoadByNameCo(string s)
        => GameSceneManager.Instance.LoadSceneForSceneName_Co(s);
    public UnityEngine.UI.Button GameStartButton => GameSceneManager.Instance.GameStartButton;
}
