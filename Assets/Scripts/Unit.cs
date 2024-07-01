using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseColor { Blue, Red }
public enum UnitType { A, G, S, W }

public class Unit : MonoBehaviour
{
    [SerializeField] private BaseColor baseColor;
    [SerializeField] private UnitType unitType;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackDamage;
    [SerializeField] private Sprite dieSprite;
    [SerializeField] private float HP;

    private Animator animator;
    private SpriteRenderer sr;

    private bool isFight;
    private float currentMoveSpeed;
    private GameObject enemy;
    private Sprite originSprite;
    private ParticleSystem dust;

    private void Awake()
    {
        currentMoveSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        dust = transform.GetComponentInChildren<ParticleSystem>();
        enemy = null;
        isFight = false;
        originSprite = sr.sprite;
    }

    private void Update()
    {
        float newX = transform.position.x + currentMoveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, 0, 0);

        if (!isFight && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && enemy != null)
        {
            Debug.Log("Fight!");
            isFight = true;
            OnAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
        Debug.Log("Move");
        isFight = false;
        animator.SetBool("isMoving", true);
        StopCoroutine(Attack(enemy));
        currentMoveSpeed = moveSpeed;
        dust.Play();
    }

    public void OnAttack()
    {
        if (enemy == null)
        {
            OnMoving();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Attack(enemy));
    }

    private IEnumerator Attack(GameObject enemy)
    {
        if (unitType == UnitType.S)
        {
            WaitForSeconds waitAttack = new WaitForSeconds(attackDelay);
            WaitForSeconds waitIdle = new WaitForSeconds(0.15f);
            while (isFight)
            {
                Debug.Log("Attack");
                sr.sprite = attackSprite;
                enemy.GetComponent<Unit>().OnAttacked(attackDamage);
                yield return waitIdle;
                sr.sprite = originSprite;
                yield return waitAttack;
            }
        }
        else
        {
            yield return null;
        }
    }

    public void OnAttacked(float damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        sr.sprite = dieSprite;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
