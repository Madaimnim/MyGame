using System.Collections;
using UnityEngine;

public class WeatherManager : MonoBehaviour {
    public RainController RainController;

    private void Start() {
        RainController.StartRain();
        StartCoroutine(StopRainAfterDelay(15f)); 
    }

    private IEnumerator StopRainAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        RainController.StopRain();
    }
}
