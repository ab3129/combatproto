# Projectile Smooth Movement Plan

This document outlines the plan to modify the `Projectile.cs` script to enable smooth visual movement across the screen while maintaining its logical hitbox on the current tile.

## Detailed Plan:

1.  **Modify `Projectile.cs` for smooth movement:**
    *   Introduce two new `Vector3` variables: `_startWorldPos` and `_targetWorldPos`. These will store the world positions of the tile the projectile is moving *from* and *to*, respectively.
    *   Modify the `Init` method to set the initial `_startWorldPos` and `_targetWorldPos` based on the starting tile and the first step.
    *   Adjust `FixedUpdate` to interpolate the `transform.position` between `_startWorldPos` and `_targetWorldPos` using `Vector3.Lerp` and the `accumulator` as a progress value (0 to 1).
    *   The `StepOneTile()` method will remain responsible for updating the `tile` variable (the logical current tile), removing the attack from the old tile, adding it to the new tile, and resolving combat. It will also update `_startWorldPos` and `_targetWorldPos` for the next interpolation.

2.  **Combat Resolution:**
    *   The combat resolution logic within `StepOneTile()` will remain unchanged. This ensures that the projectile's hitbox is always active on the tile it has *logically* entered, even if its visual representation is still in transit between tiles.
    *   Upon a successful hit, the projectile will be destroyed immediately, stopping any ongoing smooth movement interpolation.

## Mermaid Diagram for Projectile Movement Logic:

```mermaid
graph TD
    A[Projectile Init] --> B{Set initial tile};
    B --> C[Call StepOneTile (initial)];
    C --> D[FixedUpdate];
    D --> E{accumulator >= 1f?};
    E -- No --> F[Interpolate transform.position between _startWorldPos and _targetWorldPos using accumulator (Vector3.Lerp)];
    E -- Yes --> G[accumulator -= 1f];
    G --> H[Call StepOneTile];
    H --> I{IsInsideGrid(new tile)?};
    I -- No --> J[Destroy Projectile];
    I -- Yes --> K[Update current tile];
    K --> L[Remove attack from old tile];
    L --> M[Add attack to new tile];
    M --> N[Resolve Combat for new tile];
    N --> O{Combat resolved (hit)?};
    O -- Yes --> J;
    O -- No --> P[Set _startWorldPos to current tile's world pos];
    P --> Q[Set _targetWorldPos to next tile's world pos];
    Q --> D;
    F --> D;