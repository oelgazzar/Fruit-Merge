using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ReduceBoard()
    {
        HistoryManager.Instance.SaveSnapshot();
        FruitTracker.Instance.Reduce();
    }

    public void Restart()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;

        HistoryManager.Instance.ClearHistory();
        FruitTracker.Instance.ClearBoard();
        FruitSpawner.Instance.ClearQueue();
        ScoreManager.Instance.RestoreScore(0);
        
        FruitSpawner.Instance.Init();

        Physics2D.Simulate(Time.fixedDeltaTime);
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }
}
