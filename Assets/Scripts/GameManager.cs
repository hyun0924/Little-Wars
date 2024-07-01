using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { Easy = 0, Normal, Hard }

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject[] Bases;

    private Difficulty currentDifficulty;

    private void Awake()
    {
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
}
