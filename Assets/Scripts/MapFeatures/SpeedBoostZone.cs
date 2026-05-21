using System.Collections;
using UnityEngine;

public class SpeedBoostZone : MonoBehaviour
{
    [Header("Ustawienia")]
    public float czasTrwania = 3f;
    public float silaBoosta = 5f;

    // Flaga, która mówi, czy pad jest gotowy, czy w³aœnie siê "³aduje"
    private bool padGotowy = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy to kot i czy lód jest gotowy do u¿ycia
        if (!other.CompareTag("Player")) return;
        if (!padGotowy) return;

        PlayerMovementV2 pm = other.transform.root.GetComponentInChildren<PlayerMovementV2>();

        if (pm != null)
        {
            StartCoroutine(DajBoosta(pm));
        }
    }

    private IEnumerator DajBoosta(PlayerMovementV2 pm)
    {
        // 1. ZABLOKOWANIE PADU NA CZAS TRWANIA BOOSTA
        padGotowy = false;

        // Robimy lód lekko przezroczystym, by pokazaæ, ¿e jest w trakcie odnawiania
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 1f, 0.5f); // 0.5f to po³owa przezroczystoœci
        }

        // 2. DAJEMY BOOSTA
        pm.BonusSpeed += silaBoosta;
        Debug.Log($"ZIUUUM! Wielorazowy lód aktywowany! Bonus: {pm.BonusSpeed}");

        // 3. ODLICZANIE (3 sekundy)
        yield return new WaitForSeconds(czasTrwania);

        // 4. ODBIERAMY BOOSTA
        if (pm != null)
        {
            pm.BonusSpeed -= silaBoosta;
            Debug.Log($"Koniec dopalacza. Bonus wraca do: {pm.BonusSpeed}");
        }

        // 5. REAKTYWACJA LODU (Zamiast niszczenia!)
        padGotowy = true; // Znowu mo¿na w niego wejœæ
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 1f, 1f); // Wracamy do pe³nego koloru
        }
    }
}
