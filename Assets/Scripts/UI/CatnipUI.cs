using UnityEngine;
using UnityEngine.UI;

public class CatnipUI : MonoBehaviour
{
    public Text amountText;

    public void UpdateUI(int amount)
    {
        amountText.text = amount.ToString();
    }
}