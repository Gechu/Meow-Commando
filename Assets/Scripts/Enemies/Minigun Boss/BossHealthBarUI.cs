using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public void HideBar()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();

        for (float t = 1f; t >= 0f; t -= Time.deltaTime)
        {
            cg.alpha = t;
            yield return null;
        }

        gameObject.SetActive(false);
    }

}