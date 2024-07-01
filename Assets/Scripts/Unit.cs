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

    private Animator animator;
    private SpriteRenderer sr;

    private float currentMoveSpeed;
    private GameObject enemy;
    private Sprite originSprite;

    private void Awake()
    {
        currentMoveSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        enemy = null;
        originSprite = sr.sprite;
    }

    private void Update()
    {
        float newX = transform.position.x + currentMoveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != baseColor.ToString())
        {
            currentMoveSpeed = 0;
            animator.SetBool("isFight", true);
            transform.GetChild(0).gameObject.SetActive(false);
            enemy = other.gameObject;
        }

        else if (other.gameObject.tag == baseColor.ToString())
        {
            currentMoveSpeed = 0;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnMoving();
    }

    public void OnMoving()
    {
        animator.SetBool("isFight", false);
        currentMoveSpeed = moveSpeed;
        transform.GetChild(0).gameObject.SetActive(true);
        StopCoroutine(Attack(enemy));
        enemy = null;
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
            while (animator.GetBool("isFight"))
            {
                sr.sprite = attackSprite;
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
}
