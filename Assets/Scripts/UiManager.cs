using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Button _undoButton;
    [SerializeField] Button _reduceButton;
    [SerializeField] Button _restartbutton;

    private void Start()
    {
        _undoButton.onClick.AddListener(OnUndoButtonClicked);
        _reduceButton.onClick.AddListener(OnReduceButtonClicked);
        _restartbutton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString();
    }

    void OnUndoButtonClicked()
    {
        HistoryManager.Instance.Undo();
    }

    void OnReduceButtonClicked()
    {
        GameManager.Instance.ReduceBoard();
    }

    void OnRestartButtonClicked()
    {
        GameManager.Instance.Restart();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= OnScoreChanged;
        _undoButton.onClick.RemoveAllListeners();
        _reduceButton.onClick.RemoveAllListeners();
    }

    
}
