using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class StartSceneUIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button startButton;

    private void Awake() {
        startButton.onClick.AddListener(OnClickStart);

        startButton.gameObject.SetActive(false);
    }


    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        startButton.gameObject.SetActive(true);
    }

    private void OnClickStart() {
        GameManager.Instance.GameStateSystem.SetState(GameStateSystem.GameState.Preparation); // °ª¼h API
        startButton.interactable=false;
    }
}
