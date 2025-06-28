#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

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
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
}
#endif