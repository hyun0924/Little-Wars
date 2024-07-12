using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite destroySprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    private float currentHP;
    public float MaxHP { get { return maxHP; } }
    public float CurrentHP { get { return currentHP; } }
    private bool isDestroy;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
        isDestroy = false;
        hpSlider.value = currentHP / maxHP;
        hpText.text = currentHP.ToString("00");
    }

    public void OnHit(float damage)
    {
        if (isDestroy) return;
        currentHP = Mathf.Max(0, currentHP - damage);
        hpSlider.value = currentHP / maxHP;
        hpText.text = currentHP.ToString("00");

        if (currentHP <= 0)
        {
            isDestroy = true;
            sr.sprite = destroySprite;
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.GameOver(gameObject.tag);
        }
        else
        {
            StartCoroutine(Hit());
        }
    }

    private IEnumerator Hit()
    {
        sr.sprite = hitSprite;
        yield return new WaitForSeconds(0.2f);
        sr.sprite = normalSprite;
    }

    public void IncreaseHp(float hp)
    {
        currentHP = Mathf.Min(currentHP + hp, maxHP);
        hpSlider.value = currentHP / maxHP;
        hpText.text = currentHP.ToString("00");
    }

    public void DecreaseHp()
    {
        currentHP--;
        hpSlider.value = currentHP / maxHP;
        hpText.text = currentHP.ToString("00");
    }
}
