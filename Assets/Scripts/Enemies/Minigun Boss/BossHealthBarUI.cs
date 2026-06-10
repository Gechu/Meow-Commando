using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    // sygnatura zgodna z UnityEvent<float,float>
    public void UpdateHealth(float current, float max)
    {
        if (!slider) return;
        slider.maxValue = max;
        slider.value = current;
    }

    public void SetMax(float max)
    {
        if (!slider) return;

        slider.maxValue = max;
        slider.value = max;
    }
}