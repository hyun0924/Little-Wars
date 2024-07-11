using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { A, G, S, W, B }

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitData", order = 0)]
public class UnitData : ScriptableObject
{
    public UnitType unitType;
    public float moveSpeed;
    public float attackSpeed;
    public float attackDelay;
    public float attackDamage;
    public float HP;
    public int price;
    [SerializeField] private Sprite attackSpriteB;
    [SerializeField] private Sprite hitSpriteB;
    [SerializeField] private Sprite hitRedSpriteB;
    [SerializeField] private Sprite dieSpriteB;
    [SerializeField] private Sprite attackSpriteR;
    [SerializeField] private Sprite hitSpriteR;
    [SerializeField] private Sprite hitRedSpriteR;
    [SerializeField] private Sprite dieSpriteR;
    [SerializeField] private GameObject projectiveB;
    [SerializeField] private GameObject projectiveR;
    public Sprite portrait;

    public Sprite GetAttackSprite(BaseColor baseColor)
    {
        return baseColor == BaseColor.Blue ? attackSpriteB : attackSpriteR;
    }

    public Sprite GetHitSprite(BaseColor baseColor)
    {
        return baseColor == BaseColor.Blue ? hitSpriteB : hitSpriteR;
    }
    
    public Sprite GetHitRedSprite(BaseColor baseColor)
    {
        return baseColor == BaseColor.Blue ? hitRedSpriteB : hitRedSpriteR;
    }

    public Sprite GetDieSprite(BaseColor baseColor)
    {
        return baseColor == BaseColor.Blue ? dieSpriteB : dieSpriteR;
    }

    public GameObject GetProjective(BaseColor baseColor)
    {
        return baseColor == BaseColor.Blue ? projectiveB : projectiveR;
    }
}