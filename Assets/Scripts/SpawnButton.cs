using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    [SerializeField] BaseColor baseColor;
    [SerializeField] UnitType unitType;
    [SerializeField] UnitData[] unitDatas;
    [SerializeField] GameObject upgradeBtn;
    [SerializeField] TextMeshProUGUI priceText;

    private TextMeshProUGUI hpText;
    private TextMeshProUGUI atkText;
    private Button button;
    private int level = 0;
    private Image portraitImage;

    private void Start()
    {
        portraitImage = transform.GetChild(0).GetComponent<Image>();

        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
                GameManager.Instance.SpawnUnit(baseColor, unitType, level));
        button.onClick.AddListener(() => 
                GameManager.Instance.DecreaseMoney(unitDatas[level].price, baseColor));

        hpText = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (transform.childCount > 2)
            atkText = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

        SetText();
    }

    private void Update()
    {
        if (level == 0 && GameManager.money >= unitDatas[level].upgradePrice) upgradeBtn.SetActive(true);
        else upgradeBtn.SetActive(false);

        if (GameManager.money < unitDatas[level].price) button.interactable = false;
        else button.interactable = true;
    }

    public void LevelUp()
    {
        if (level == 1) return;

        GameManager.Instance.DecreaseMoney(unitDatas[level++].upgradePrice, baseColor);
        SetText();
    }

    private void SetText()
    {
        portraitImage.sprite = unitDatas[level].portrait;
        priceText.text = "" + unitDatas[level].price;
        hpText.text = unitDatas[level].HP.ToString();
        if (atkText != null) atkText.text = unitDatas[level].attackDamage.ToString();
    }
}
