using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { A, G, S, W }

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitData", order = 0)]
public class UnitData : ScriptableObject
{
    public UnitType unitType;
    public float moveSpeed;
    public float attackSpeed;
    public float attackDelay;
    public float attackDamage;
    public float HP;
}