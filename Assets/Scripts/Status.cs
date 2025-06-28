using UnityEngine;

public enum Faction { Player, Enemy }
public enum StatusEffect { None, Poison, Stun }

public readonly struct DamageEvent
{
    public readonly int   amount;
    public readonly float knockBack;
    public readonly StatusEffect status;

    public DamageEvent(int amount, float knockBack, StatusEffect status)
    {
        this.amount    = amount;
        this.knockBack = knockBack;
        this.status    = status;
    }
} 
 