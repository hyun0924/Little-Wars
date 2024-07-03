using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    [SerializeField] BaseColor baseColor;
    [SerializeField] UnitType unitType;
    [SerializeField] Sprite[] portraits;
    private Button button;
    private int level = 0;
    private Image portraitImage;

    private void Start()
    {
        portraitImage = transform.GetChild(0).GetComponent<Image>();
        portraitImage.sprite = portraits[level];

        button = GetComponent<Button>();
        button.onClick.AddListener(() => 
                GameManager.Instance.SpawnUnit(baseColor, unitType, level));
    }

    public void LevelUp()
    {
        if (level == 1) return;

        portraitImage.sprite = portraits[++level];
    }
}
