using System.Collections;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Timing")]
    public float showDelay = 0.75f;
    public float visibleTime = 5f;

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        StartCoroutine(ShowPanelRoutine());
    }

    IEnumerator ShowPanelRoutine()
    {
        // Delay przed pokazaniem
        yield return new WaitForSeconds(showDelay);

        // Poka¿ panel
        panel.SetActive(true);

        // Czas wyœwietlania
        yield return new WaitForSeconds(visibleTime);

        // Ukryj panel
        panel.SetActive(false);
    }
}