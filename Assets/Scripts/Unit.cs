using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BaseColor { Blue, Red }

public class Unit : MonoBehaviour
{
    [SerializeField] private BaseColor baseColor;
    [SerializeField] private UnitData unitData;

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
    private List<GameObject> enemies = new List<GameObject>();

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
        transform.position = new Vector3(newX, transform.position.y, 0);

        if (isDie) return;

        if (!isFight && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && (enemy != null || enemies.Count > 0))
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

        if (unitData.unitType == UnitType.W)
        {
            if (!enemies.Contains(other.gameObject))
            {
                enemies.Add(other.gameObject);
                animator.SetBool("isMoving", false);
                if (!isFight) delay = unitData.attackDelay;
            }
            return;
        }

        BaseColor enemyBaseColor = (BaseColor)(((int)baseColor + 1) % 2);
        if (other.gameObject.tag == enemyBaseColor.ToString())
        {
            currentMoveSpeed = 0;
            enemy = other.gameObject;
            animator.SetBool("isMoving", false);
            delay = unitData.attackDelay;
            if (dust != null) dust.Stop();
        }

        else if (other.gameObject.tag == baseColor.ToString())
        {
            Debug.Log("TriggerEnter");
            bool isFrontUnit = (transform.position.x - other.transform.position.x) / currentMoveSpeed > 0;
            if (isFrontUnit) return;

            currentMoveSpeed = 0;
            if (dust != null) dust.Stop();
            animator.SetBool("isMoving", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("TriggerExit");
        if (other.gameObject.tag != "Detect")
        {
            if (unitData.unitType == UnitType.W && enemies.Contains(other.gameObject))
            {
                enemies.Remove(other.gameObject);
                if (enemies.Count == 0)
                {
                    isFight = false;
                    animator.SetBool("isMoving", true);
                }
            }
            else
            {
                enemy = null;
                OnMoving();
            }
        }
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
        if (dust != null) dust.Play();
    }

    public void OnAttack()
    {
        if ((enemy == null && enemies.Count == 0) || isDie) return;

        StopAllCoroutines();
        isHit = false;
        Debug.Log($"Attack: {baseColor}");
        sr.sprite = unitData.GetAttackSprite(baseColor);

        if (unitData.unitType == UnitType.S || unitData.unitType == UnitType.G)
            Invoke("enemyHit", unitData.attackSpeed / 3f);

        else if (unitData.unitType == UnitType.A)
        {
            Vector3 pos = transform.position;
            if (baseColor == BaseColor.Red) pos += new Vector3(-0.25f, 0.25f, 0);
            else pos += new Vector3(0.25f, 0.25f, 0);
            GameObject arrow = Instantiate(unitData.GetProjective(baseColor), pos, Quaternion.identity);
            arrow.GetComponent<Projectile>().damage = unitData.attackDamage;
        }
        else
        {
            foreach (GameObject e in enemies)
            {
                Vector3 pos = e.transform.position;
                if (baseColor == BaseColor.Red) pos += new Vector3(-0.3f, 0, 0);
                else pos += new Vector3(0.3f, 0, 0);
                Instantiate(unitData.GetProjective(baseColor), pos, Quaternion.identity);
                e.GetComponent<Unit>().OnHit(unitData.attackDamage);
            }
        }
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
        float speed = currentMoveSpeed;
        currentMoveSpeed = 0;
        sr.sprite = unitData.GetHitRedSprite(baseColor);

        yield return new WaitForSeconds(0.15f);

        currentMoveSpeed = speed;
        isHit = false;
        sr.sprite = unitData.GetHitSprite(baseColor);

        yield return new WaitForSeconds(0.15f);

        sr.sprite = originSprite;
    }

    private IEnumerator Die()
    {
        Debug.Log($"Die: {baseColor}");
        isDie = true;
        currentMoveSpeed = 0;
        if (dust != null) dust.Stop();
        animator.SetBool("isMoving", false);
        sr.sortingOrder--;

        yield return StartCoroutine(Hit());

        GetComponent<BoxCollider2D>().enabled = false;
        sr.sprite = unitData.GetDieSprite(baseColor);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
