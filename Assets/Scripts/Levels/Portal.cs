using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // Ta linijka pozwoli nam wpisaæ w Inspectorze nazwê kolejnego poziomu!
    [Header("Ustawienia przejœcia")]
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy w portal wszed³ kot (Gracz)
        if (other.CompareTag("Player"))
        {
            // Jeœli wpisaliœmy nazwê poziomu, to go ³adujemy
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                Debug.Log("Gracz wszed³ w portal! £adujê: " + nextLevelName);
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogError("Zapomnia³eœ wpisaæ nazwy kolejnego poziomu w Inspectorze!");
            }
        }
    }
}