using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PresentationHost : MonoBehaviour {
    private IPresentationController _presentationController;
    private Player _player;
    private void Awake() {
        _player = GetComponent<Player>();
    }

    private void Update() {
        _presentationController?.Tick();
    }

    public void Play(IPresentationController presentationController) {
        Stop();
        _presentationController = presentationController;
        _presentationController.Enter(_player);
    }
    public void Stop() {
        _presentationController?.Exit();
        _presentationController = null;
    }

}
