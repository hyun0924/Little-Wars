using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { Easy = 0, Normal, Hard }

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject[] Bases;
    [SerializeField] private GameObject[] UnitPrefabs;

    private Difficulty currentDifficulty;

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
        if (unitType == UnitType.W)
        {
            Transform t = Bases[(int)baseColor].transform;
            if (t.childCount == 1) Destroy(t.GetChild(0).gameObject);
            Instantiate(UnitPrefabs[index], t);
        }
        else Instantiate(UnitPrefabs[index]);
    }
}
