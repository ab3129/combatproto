using UnityEngine;

[DefaultExecutionOrder(+1000)]      // ❶ runs after projectiles move
public class CombatResolver : MonoBehaviour
{
    [SerializeField] GridManager grid;

    void Awake()                    // ❷ auto-fill if you forgot in inspector
    {
        if (grid == null) grid = GridManager.I;
    }

    // FixedUpdate is no longer needed as combat resolution is now handled by projectiles directly.
    // void FixedUpdate()
    // {
    //     foreach (var pair in grid.GetPairsToResolve())
    //     {
    //         Debug.Log($"Combat: {pair.attack.Faction} attacking {pair.target.Faction}");
    //         pair.target.ApplyDamage(pair.attack.AttackData.BuildDamageEvent());
    //         // optional: ((MonoBehaviour)pair.attack).gameObject.SetActive(false);
    //         ((MonoBehaviour)pair.attack).gameObject.SetActive(false); // Deactivate projectile after hit
    //     }
    // }
}
