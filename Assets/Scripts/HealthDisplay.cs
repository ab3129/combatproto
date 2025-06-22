using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Vector3 textOffset = new Vector3(0, .5f, 0); // Lowered Y offset
    [SerializeField] private GameObject textPrefab;

    private HealthSystem healthSystem;
    private TextMeshPro textObj;

    void Start()
    {
        // Get reference to the health system on this object
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError("Missing HealthSystem component!");
            return;
        }

        if (textPrefab != null)
        {
            // Instantiate text object above the sprite
            GameObject textGO = Instantiate(textPrefab, transform.position + textOffset, Quaternion.identity, transform);
            textObj = textGO.GetComponent<TextMeshPro>();

            // Ensure the text is centered for visibility
            textObj.alignment = TextAlignmentOptions.Center;
            
            // Render above other sprites
            Renderer rend = textObj.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.sortingOrder = 10;
            }

            UpdateHealthText();
        }
        else
        {
            Debug.LogError("HealthDisplay is missing a TextMeshPro prefab reference!");
        }
    }

    void Update()
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (textObj != null && healthSystem != null)
        {
            textObj.text = healthSystem.currentHealth.ToString();
        }
    }
}
