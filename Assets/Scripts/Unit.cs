using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseColor { Blue, Red }

public class Unit : MonoBehaviour
{
    [SerializeField] private BaseColor baseColor;
    [SerializeField] private UnitData unitData;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Sprite hitRedSprite;
    [SerializeField] private Sprite dieSprite;

    private Animator animator;
    private SpriteRenderer sr;

    private float delay;
    private bool isFight; // 싸우는 중인지
    private bool wasAttack; // 한 번 공격했는지
    private bool isHit; // 맞고 있는 중인지
    private bool isDie; // 죽고 있는 중인지
    private float currentMoveSpeed;
    private float currentHP;
    private GameObject enemy;
    private Sprite originSprite;
    private ParticleSystem dust;

    private void Awake()
    {
        currentMoveSpeed = baseColor == BaseColor.Blue ? unitData.moveSpeed : -unitData.moveSpeed;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        dust = transform.GetComponentInChildren<ParticleSystem>();
        enemy = null;
        isFight = false;
        wasAttack = false;
        delay = 0f;
        originSprite = sr.sprite;
        currentHP = unitData.HP;
    }

    private void Update()
    {
        float newX = transform.position.x + currentMoveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, 0, 0);

        if (isDie) return;

        if (!isFight && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && enemy != null)
            isFight = true;

        if (isFight)
        {
            delay += Time.deltaTime;
            if (wasAttack && delay >= unitData.attackSpeed)
            {
                delay = 0f;
                wasAttack = false;
                if (!isHit) sr.sprite = originSprite;
            }
            else if (!isHit && !wasAttack && delay >= unitData.attackDelay)
            {
                delay = 0f;
                OnAttack();
                wasAttack = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDie) return;

        if (other.gameObject.tag != baseColor.ToString())
        {
            currentMoveSpeed = 0;
            enemy = other.gameObject;
            animator.SetBool("isMoving", false);
            dust.Stop();
        }

        else if (other.gameObject.tag == baseColor.ToString())
        {
            currentMoveSpeed = 0;
            dust.Stop();
            animator.SetBool("isMoving", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemy = null;
        OnMoving();
    }

    public void OnMoving()
    {
        if (isDie) return;
        Debug.Log($"Move: {baseColor}");
        isFight = false;
        sr.sprite = originSprite;
        animator.SetBool("isMoving", true);
        StopAllCoroutines();
        currentMoveSpeed = baseColor == BaseColor.Blue ? unitData.moveSpeed : -unitData.moveSpeed;
        dust.Play();
    }

    public void OnAttack()
    {
        if (enemy == null || isDie)
        {
            OnMoving();
            return;
        }

        StopAllCoroutines();
        isHit = false;
        Debug.Log($"Attack: {baseColor}");
        sr.sprite = attackSprite;
        Invoke("enemyHit", unitData.attackSpeed / 3f);
    }

    private void enemyHit()
    {
        enemy.GetComponent<Unit>().OnHit(unitData.attackDamage);
    }

    public void OnHit(float damage)
    {
        if (isDie) return;
        currentHP -= damage;

        if (currentHP <= 0)
        {
            CancelInvoke();
            StopAllCoroutines();
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(Hit());
        }
    }

    private IEnumerator Hit()
    {
        Debug.Log($"Hit: {baseColor}");
        isHit = true;
        sr.sprite = hitRedSprite;
        yield return new WaitForSeconds(0.15f);
        isHit = false;
        sr.sprite = hitSprite;
        yield return new WaitForSeconds(0.15f);
        sr.sprite = originSprite;
    }

    private IEnumerator Die()
    {
        Debug.Log($"Die: {baseColor}");
        isDie = true;
        sr.sortingOrder--;
        yield return StartCoroutine(Hit());
        GetComponent<BoxCollider2D>().enabled = false;
        sr.sprite = dieSprite;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
