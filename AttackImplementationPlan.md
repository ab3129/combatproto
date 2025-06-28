# Attack Implementation Plan

## Goal: Implement a new attack that deals 1 damage to the hit target and 2 damage to enemies above and below the hit target.

### **Phase 1: Define Area Damage Structure**

1.  **Create `AreaDamageEffect.cs`**:
    *   Define a new C# struct `AreaDamageEffect` to encapsulate an offset and damage properties for area-of-effect attacks.
    *   This struct will contain:
        *   `Vector2Int offset`: The relative tile offset from the primary hit target.
        *   `int damage`: The damage value for this specific area effect.
        *   `StatusEffect statusEffect`: Any status effect applied by this area effect.
        *   A `BuildDamageEvent()` method to create a `DamageEvent`.

### **Phase 2: Enhance AttackDescriptor**

1.  **Modify `AttackDescriptor.cs`**:
    *   Introduce a new `enum AttackType` (e.g., `SingleTarget`, `VerticalSplash`) to categorize different attack behaviors.
    *   Add a `public AttackType attackType;` field.
    *   Add a `public List<AreaDamageEffect> areaEffects;` field to define multiple area damage effects for attacks like "Vertical Splash".

### **Phase 3: Implement Attack Logic in Projectile**

1.  **Modify `Projectile.cs`**:
    *   In the `StepOneTile()` method, after applying primary damage to the `pair.target`:
        *   Check the `attack.AttackData.attackType`.
        *   If `attackType` is `VerticalSplash`:
            *   Iterate through the `attack.AttackData.areaEffects` list.
            *   For each `AreaDamageEffect` in the list:
                *   Calculate the `affectedTile` by adding the `areaEffect.offset` to the `tile` where the primary hit occurred.
                *   Use `GridManager.I.GetPairsToResolveForTile(affectedTile)` to find any `IDamageable` targets on the `affectedTile`.
                *   For each target found, apply damage using `target.ApplyDamage(areaEffect.BuildDamageEvent())`.
        *   Ensure the `Destroy(gameObject);` call for the projectile happens only after all primary and area damage has been processed.

### **Phase 4: Create New Attack Descriptor Asset**

1.  **Modify `CreateAttackDescriptors.cs`**:
    *   Add a new `MenuItem` or extend the existing one to create a new `AttackDescriptor` asset specifically for the "Vertical Splash" attack.
    *   Set its `damage` (for the primary target) to 1.
    *   Set its `attackType` to `VerticalSplash`.
    *   Populate its `areaEffects` list with two `AreaDamageEffect` entries:
        *   One with `offset = new Vector2Int(0, 1)` and `damage = 2`.
        *   One with `offset = new Vector2Int(0, -1)` and `damage = 2`.

### **Diagram of Attack Flow (Simplified)**

```mermaid
graph TD
    A[Projectile Hits Target] --> B{Check AttackType};
    B -- SingleTarget --> C[Apply Primary Damage to Target];
    B -- VerticalSplash --> D[Apply Primary Damage to Target];
    D --> E{Iterate AreaEffects};
    E --> F[Calculate Affected Tile];
    F --> G[Find Targets on Affected Tile];
    G --> H[Apply Area Damage to Targets];
    H --> E;
    E -- No More AreaEffects --> I[Destroy Projectile];
    C --> I;