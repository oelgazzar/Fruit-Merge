using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;

    private void ScoreManager_OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= ScoreManager_OnScoreChanged;
    }

    
}
