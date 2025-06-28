// HealthDisplay.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(HealthSystem))]
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Vector3   textOffset = new(0, 0.5f, 0);
    [SerializeField] private GameObject textPrefab;      // world-space TextMeshPro prefab

    private HealthSystem health;
    private TextMeshPro  textMesh;

    void Start()
    {
        health = GetComponent<HealthSystem>();

        if (textPrefab == null)
        {
            Debug.LogError($"{name} → HealthDisplay missing textPrefab reference");
            enabled = false;
            return;
        }

        // Spawn the text object as a child so it follows this transform
        var go = Instantiate(textPrefab, transform.position + textOffset, Quaternion.identity, transform);
        textMesh = go.GetComponent<TextMeshPro>();

        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.GetComponent<Renderer>().sortingOrder = 10;   // render on top

        UpdateText(health.CurrentHP, health.MaxHP);

        // Subscribe to health changes
        health.HealthChanged += UpdateText;
    }

    void OnDisable()
    {
        if (health != null)
            health.HealthChanged -= UpdateText;
    }

    /* -------------------------------------------------------------- */
    private void UpdateText(int current, int max)
    {
        if (textMesh != null)
            textMesh.text = current.ToString();   // or $"{current}/{max}" for full bar
    }
}
