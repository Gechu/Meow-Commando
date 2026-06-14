using UnityEngine;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour
{
    public Text coinText;

    public void UpdateUI(int amount)
    {
        coinText.text = amount.ToString();
    }
}