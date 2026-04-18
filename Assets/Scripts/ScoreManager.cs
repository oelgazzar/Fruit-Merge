using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score => _currentScore;

    int _baseScore = 10;
    int _currentScore;

    public static event Action<int> OnScoreChanged;
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void MergeManager_OnFruitMerge(Fruit fruit1, Fruit fruit2, Fruit newFruit)
    {
        var finalScore = _baseScore * (int)Mathf.Pow(2, fruit1.Model.Tier);
        _currentScore += finalScore;
        
        OnScoreChanged(_currentScore);
    }

    public void RestoreScore(int score)
    {
        _currentScore = score;

        OnScoreChanged(_currentScore);
    }

    private void OnEnable()
    {
        MergeManager.OnFruitMerge += MergeManager_OnFruitMerge;
    }

    private void OnDisable()
    {
        MergeManager.OnFruitMerge -= MergeManager_OnFruitMerge;
    }
}
