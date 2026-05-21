using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    [Header("Który portal mam odkryæ?")]
    public GameObject ukrytyPortal; // Tu podepniemy portal, który stoi na mapie

    private float checkTimer = 0f;
    private bool hasPortalSpawned = false;

    void Update()
    {
        // Jeœli portal ju¿ jest widoczny, koñczymy pracê
        if (hasPortalSpawned) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= 0.5f)
        {
            checkTimer = 0f;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Jeœli wszyscy nie ¿yj¹...
            if (enemies.Length == 0)
            {
                Debug.Log("Wszyscy wrogowie pokonani! W³¹czam portal.");

                // Zdejmujemy pelerynê niewidkê z portalu
                if (ukrytyPortal != null)
                {
                    ukrytyPortal.SetActive(true);
                }

                hasPortalSpawned = true; // Zabezpieczenie
            }
        }
    }
}