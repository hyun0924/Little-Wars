using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySimulator : MonoBehaviour
{
    public static int money;
    public static float moneyDelay;

    private int[] currentSpawnLevels;
    private BaseController baseController;
    [SerializeField] private GameObject[] buttons;

    private void Awake()
    {
        money = 0;
        moneyDelay = 1f;
        currentSpawnLevels = new int[5] { 0, 0, 0, 0, 0 };
        InvokeRepeating("IncreaseMoney", 0, moneyDelay);
    }

    private void Start()
    {
        baseController = GameManager.Instance.EnemyBase.GetComponent<BaseController>();
    }

    private void Update()
    {
        if (money == 12) SpawnUnit();
    }

    public void IncreaseMoney()
    {
        if (money == GameManager.Instance.maxMoney) return;
        money++;

        SpawnUnit();
    }

    private void SpawnUnit()
    {
        while (true)
        {
            int useMoney = Random.Range(0, Mathf.Min(5, money + 1));

            if (currentSpawnLevels[0] != 0 && useMoney == 1) useMoney = 0;
            else if (currentSpawnLevels[0] != 1 && useMoney == 2) useMoney = 0;

            Debug.Log("UseMoney: " + useMoney + " -> Current Money: " + (money - useMoney));

            switch (useMoney)
            {
                case 1:
                case 2:
                    ChooseHP(useMoney);
                    return;
                case 3:
                case 4:
                    float threshold = 0.5f;
                    float decision = Random.Range(0, 1f);

                    // 회복 업그레이드 됐거나 아직 2단계 아닌 경우
                    if (currentSpawnLevels[0] != useMoney - 1f) threshold += 0.5f;
                    // S 업그레이드 된 경우
                    if (currentSpawnLevels[(useMoney - 1) / 2] != (useMoney - 1) % 2) threshold -= 0.5f;

                    // 둘 다 업그레이드 된 경우
                    if (threshold == 0.5f && currentSpawnLevels[(useMoney - 1) / 2] != (useMoney - 1) % 2)
                    {
                        // 가지고 있는 돈이 3인 경우 - 할 수 있는 게 없음
                        if (money == useMoney) return;
                        else continue;
                    }
                    else
                    {
                        if (decision > threshold) ChooseHP(useMoney);
                        else ChooseSpawn(1, (useMoney - 1) % 2);
                        return;
                    }

                default:
                    return;
            }
        }
    }

    private void ChooseHP(int level)
    {
        if (baseController.CurrentHP < baseController.MaxHP)
        {
            float decision = Random.Range(0, 1f);
            if (decision > 0.5f)
            {
                Debug.Log("Recovery HP");
                buttons[0].transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
                return;
            }
        }
        Debug.Log("HP Upgrade");
        buttons[0].transform.GetChild(2).GetComponent<Button>().onClick.Invoke();
        currentSpawnLevels[0] = level;
    }

    private void ChooseSpawn(int type, int level)
    {
        if (level == 1)
        {
            Debug.Log($"Spawn {(UnitType)(type - 1)}U");
            buttons[type].transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
            return;
        }

        float threshold = 0.5f;
        float decision = Random.Range(0, 1f);

        // 4명 이상 소환돼있으면 업그레이드 확률 높이기
        if (GameManager.unitLinkedLists[1].Count >= 4) threshold -= 0.25f;

        if (decision > threshold)
        {
            Debug.Log($"Upgrade {(UnitType)(type - 1)}");
            buttons[type].transform.GetChild(2).GetComponent<Button>().onClick.Invoke();
            currentSpawnLevels[type] = 1;
            return;
        }

        Debug.Log($"Spawn {(UnitType)(type - 1)}");
        buttons[type].transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
        return;
    }
}
