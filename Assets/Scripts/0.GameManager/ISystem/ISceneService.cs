public interface ISceneService 
{
    void LoadGameStart();
    void LoadPreparation();
    System.Collections.IEnumerator LoadByNameCo(string sceneName);
    UnityEngine.UI.Button GameStartButton { get; }
}
