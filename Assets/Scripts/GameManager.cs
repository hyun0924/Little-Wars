using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty { Easy = 0, Normal, Hard }

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject[] Bases;
    [SerializeField] private GameObject[] UnitPrefabs;
    [SerializeField] private Slider moneySlider;
    [SerializeField] private TextMeshProUGUI moneyText;

    private Difficulty currentDifficulty;

    [SerializeField] private int maxMoney;
    public static int money;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        money = 0;
        moneySlider.value = 0;
        moneyText.text = "0";
        InvokeRepeating("IncreaseMoney", 0, 1);
    }

    public void OnDifficultyButtonClick(int difficulty)
    {
        GameSet();
        currentDifficulty = (Difficulty)difficulty;
    }

    private void GameSet()
    {
        Title.SetActive(false);
        foreach (var b in Bases) b.SetActive(true);
    }

    public void SpawnUnit(BaseColor baseColor, UnitType unitType, int level)
    {
        int index = (int)baseColor * 4 + (int)unitType + level * 8;
        Instantiate(UnitPrefabs[index]);
    }

    public void RecoveryBaseHP(BaseColor baseColor, float hp)
    {
        Bases[(int)baseColor].GetComponent<BaseController>().IncreaseHp(hp);
    }

    private void IncreaseMoney()
    {
        if (money == maxMoney) return;

        moneySlider.value = ++money;
        moneyText.text = "" + money;
    }

    public void DecreaseMoney(int price)
    {
        money = money - price;
        moneySlider.value = money;
        moneyText.text = "" + money;
    }

    public void GameOver(string loser)
    {
        Debug.Log(loser + " lose");
    }
}
