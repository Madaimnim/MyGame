using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PresentationHost : MonoBehaviour {
    private IPresentationController _presentationController;
    private void Awake() {}

    private void Update() {
        _presentationController?.Tick();
    }

    public void SetController(Player player,IPresentationController presentationController) {
        Exit();
        _presentationController = presentationController;
        _presentationController.Set(player);
    }
    public void Exit() {
        _presentationController?.Exit();
        _presentationController = null;
    }

}
