using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class TestManager : MonoBehaviour
{
    public Enemy Enemy;
    public Button TestFunctionButton;
    public Vector2 KnockbackForce;
    public float FloatPower = 5f;

    private void Start()
    {
        Enemy = FindObjectOfType<Enemy>();
        if (TestFunctionButton == null) Debug.LogError(" TestFunctionButton ¥¼¸j©w¡I");
        TestFunctionButton.onClick.AddListener(OnTestFunctionButtonClicked);
    }

    private void OnTestFunctionButtonClicked()
    {
        Enemy.Interact(new InteractInfo
        {
            Source = null,
            KnockbackForce=KnockbackForce,
            FloatPower = FloatPower
        });
    }
}
