using System;

[Serializable]
public class StateSnapshot
{
    public FruitState[] FruitsState;
    public int ScoreState;
    public FruitModel[] FruitQueueState;
    public UnityEngine.Random.State RandomState;
}
