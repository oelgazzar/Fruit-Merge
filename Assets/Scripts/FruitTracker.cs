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
    private void OnFruitDrop(Fruit fruit)
    {
        _activeFruits.Add(fruit);
        _currentFruit = null;
    }

    private void OnFruitSpawn(Fruit fruit)
    {
        // Useful in case of spawning after restoring state
        if (_currentFruit != null)
            Destroy(_currentFruit.gameObject);
        _currentFruit = fruit;
    }

    private void OnFruitMerge(Fruit fruit1, Fruit fruit2, Fruit newFruit)
    {
        _activeFruits.Remove(fruit1);
        _activeFruits.Remove(fruit2);
        _activeFruits.Add(newFruit);
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
        FruitController.OnFruitDrop += OnFruitDrop;
        MergeManager.OnFruitMerge += OnFruitMerge;
        FruitSpawner.OnFruitSpawn += OnFruitSpawn;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDrop -= OnFruitDrop;
        MergeManager.OnFruitMerge -= OnFruitMerge;
        FruitSpawner.OnFruitSpawn -= OnFruitSpawn;
    }
}
