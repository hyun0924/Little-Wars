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

    public int maxMoney;
    public static int money;

    public static List<LinkedList<Unit>> unitLinkedLists;
    public GameObject EnemyBase { get { return Bases[1];}}

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

        unitLinkedLists = new List<LinkedList<Unit>>
        {
            new LinkedList<Unit>(),
            new LinkedList<Unit>()
        };

        money = 0;
        moneySlider.value = 0;
        moneySlider.maxValue = maxMoney;
        moneySlider.GetComponent<RectTransform>().sizeDelta = new Vector2(110f * maxMoney, 70);
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
        GameObject unit;
        int index = (int)baseColor * 5 + (int)unitType + level * 10;
        if (unitType == UnitType.B)
        {
            if (Bases[(int)baseColor].transform.childCount == 1)
                Destroy(Bases[(int)baseColor].transform.GetChild(0).gameObject);

            unit = Instantiate(UnitPrefabs[index], Bases[(int)baseColor].transform);
            unitLinkedLists[(int)baseColor].AddLast(unit.GetComponent<Unit>());
        }
        else
        {
            unit = Instantiate(UnitPrefabs[index]);
            unitLinkedLists[(int)baseColor].AddLast(unit.GetComponent<Unit>());
        }
    }

    public void RecoveryBaseHP(BaseColor baseColor, float hp)
    {
        Bases[(int)baseColor].GetComponent<BaseController>().IncreaseHp(hp);
    }

    public void IncreaseMoney()
    {
        if (money == maxMoney) return;

        moneySlider.value = ++money;
        moneyText.text = "" + money;
    }

    public bool CheckFrontUnitExist(BaseColor baseColor, Unit unit)
    {
        return unitLinkedLists[(int)baseColor].Find(unit).Previous != null;
    }

    public bool CheckFrontUnit(BaseColor baseColor, Unit myUnit, Unit otherUnit)
    {
        LinkedListNode<Unit> frontUnit = unitLinkedLists[(int)baseColor].Find(myUnit).Previous;
        if (frontUnit == null) return false;
        else
        {
            if (frontUnit.Equals(unitLinkedLists[(int)baseColor].Find(otherUnit))) return true;
            else return false;
        }
    }

    /// <summary>
    /// 앞 유닛 없거나 이동 중이거나 죽었으면 true
    /// 앞 유닛 있는데 살아있고 움직이면 false
    /// </summary>
    public bool CheckFrontUnitMoving(BaseColor baseColor, Unit unit)
    {
        LinkedListNode<Unit> frontUnit = unitLinkedLists[(int)baseColor].Find(unit).Previous;
        if (frontUnit != null)
        {
            return frontUnit.Value.isMoving || frontUnit.Value.isDie;
        }
        else return true;
    }

    public void DecreaseMoney(int price, BaseColor baseColor)
    {
        if (baseColor == BaseColor.Blue)
        {
            // money = money - price;
            // moneySlider.value = money;
            // moneyText.text = "" + money;
        }
        else EnemySimulator.money -= price;
    }

    public void GameOver(string loser)
    {
        Debug.Log(loser + " lose");
    }
}
