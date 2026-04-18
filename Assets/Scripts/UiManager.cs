using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Button _undoButton;

    private void Start()
    {
        _undoButton.onClick.AddListener(OnUndoButtonClicked);
    }

    private void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString();
    }

    void OnUndoButtonClicked()
    {
        HistoryManager.Instance.Undo();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= OnScoreChanged;
        _undoButton.onClick.RemoveAllListeners();
    }

    
}
