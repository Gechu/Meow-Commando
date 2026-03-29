using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartContainer;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    public void CreateHearts(int maxHP)
    {
        // wyczyœæ stare
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        hearts.Clear();

        // stwórz nowe
        for (int i = 0; i < maxHP; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            Image img = heart.GetComponent<Image>();
            hearts.Add(img);
        }
    }

    public void UpdateHearts(int currentHP)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHP)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}