using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitTracker : MonoBehaviour
{
    public static FruitTracker Instance { get; private set; }
    public Fruit[] ActiveFruits => _activeFruits.ToArray();
    
    readonly HashSet<Fruit> _activeFruits = new();
    Fruit _currentFruit;

    private void Awake()
    {
        Instance = this;
    }
    private void MergeManager_OnFruitMerge(Fruit fruit1, Fruit fruit2, Fruit newFruit)
    {
        _activeFruits.Remove(fruit1);
        _activeFruits.Remove(fruit2);
        _activeFruits.Add(newFruit);
        Debug.Log(_activeFruits.Count);
    }
    private void FruitController_OnFruitDropCompleted(Fruit fruit)
    {
        _activeFruits.Add(fruit);
        _currentFruit = null;
    }

    public void RestoreActiveFruits(Fruit[] fruits)
    {
        foreach (var fruit in _activeFruits)
        {
            Destroy(fruit.gameObject);
        }

        _activeFruits.Clear();

        foreach (var fruit in fruits)
        {
            _activeFruits.Add(fruit);
        }
    }

    private void OnEnable()
    {
        FruitController.OnFruitDropCompleted += FruitController_OnFruitDropCompleted;
        MergeManager.OnFruitMerge += MergeManager_OnFruitMerge;
        FruitSpawner.OnFruitSpawn += FruitSpawner_OnFruitSpawn;
    }

    private void FruitSpawner_OnFruitSpawn(Fruit fruit)
    {
        if (_currentFruit != null)
            Destroy(_currentFruit.gameObject);
        _currentFruit = fruit;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDropCompleted -= FruitController_OnFruitDropCompleted;
        MergeManager.OnFruitMerge -= MergeManager_OnFruitMerge;
        FruitSpawner.OnFruitSpawn -= FruitSpawner_OnFruitSpawn;
    }
}
