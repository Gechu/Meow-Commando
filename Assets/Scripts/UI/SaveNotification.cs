using System.Collections;
using UnityEngine;

public class SaveNotification : MonoBehaviour
{
    public static SaveNotification Instance;

    [SerializeField] private GameObject notification;
    [SerializeField] private float showTime = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (notification != null)
            notification.SetActive(false);
    }

    public void Show()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        notification.SetActive(true);

        yield return new WaitForSecondsRealtime(showTime);

        notification.SetActive(false);

        currentRoutine = null;
    }
}