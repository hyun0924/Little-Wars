using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatButton : MonoBehaviour
{
    [SerializeField] BaseColor baseColor;
    [SerializeField] float[] stats;
    [SerializeField] int[] prices;
    [SerializeField] int[] upgradePrices;
    [SerializeField] GameObject upgradeBtn;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI statText;

    private Button button;
    private int level = 0;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.RecoveryBaseHP(baseColor, stats[level]));
        button.onClick.AddListener(() =>
                GameManager.Instance.DecreaseMoney(prices[level]));
        SetText();
    }
    
    private void Update()
    {
        if (level < upgradePrices.Length && GameManager.money >= upgradePrices[level]) upgradeBtn.SetActive(true);
        else upgradeBtn.SetActive(false);

        if (GameManager.money < prices[level]) button.interactable = false;
        else button.interactable = true;
    }

    public void LevelUp()
    {
        if (level == stats.Length - 1) return;

        GameManager.Instance.DecreaseMoney(upgradePrices[level++]);
        SetText();
        
        if (level == 2) GameManager.Instance.SpawnUnit(baseColor, UnitType.B, 0);
        else if (level == 6) GameManager.Instance.SpawnUnit(baseColor, UnitType.B, 1);
    }

    private void SetText()
    {
        priceText.text = "" + prices[level];
        statText.text = stats[level].ToString();
    }
}
