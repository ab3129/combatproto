#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateAttackDescriptors
{
    [MenuItem("Combat/Create Attack Descriptors")]
    public static void CreateAttackDescriptorAssets()
    {
        // Create basic player shot
        var playerShot = ScriptableObject.CreateInstance<AttackDescriptor>();
        playerShot.damage = 1;
        playerShot.knockBack = 0f;
        playerShot.statusEffect = StatusEffect.None;
        
        AssetDatabase.CreateAsset(playerShot, "Assets/PlayerBasicShot.asset");
        
        // Create basic enemy shot
        var enemyShot = ScriptableObject.CreateInstance<AttackDescriptor>();
        enemyShot.damage = 1;
        enemyShot.knockBack = 0f;
        enemyShot.statusEffect = StatusEffect.None;
        
        AssetDatabase.CreateAsset(enemyShot, "Assets/EnemyBasicShot.asset");

        // Create vertical splash attack
        var verticalSplashShot = ScriptableObject.CreateInstance<AttackDescriptor>();
        verticalSplashShot.damage = 1; // Primary target damage
        verticalSplashShot.knockBack = 0f;
        verticalSplashShot.statusEffect = StatusEffect.None;
        verticalSplashShot.attackType = AttackType.VerticalSplash;
        verticalSplashShot.areaEffects = new List<AreaDamageEffect>
        {
            new AreaDamageEffect { offset = new Vector2Int(0, 1), damage = 2, statusEffect = StatusEffect.None }, // Above
            new AreaDamageEffect { offset = new Vector2Int(0, -1), damage = 2, statusEffect = StatusEffect.None }  // Below
        };
        
        AssetDatabase.CreateAsset(verticalSplashShot, "Assets/VerticalSplashShot.asset");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
}
#endif