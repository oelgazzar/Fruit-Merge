using System;
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
        ClearBoard();

        foreach (var fruit in fruits)
        {
            _activeFruits.Add(fruit);
        }
    }    

    public void Reduce()
    {
        var fruitCount = _activeFruits.Count;
        var fruitsList = _activeFruits.ToList();
        foreach (var i in GetRandomIndices(fruitCount, fruitCount / 2))
        {
            var fruit = fruitsList[i];
            Destroy(fruit.gameObject);
            _activeFruits.Remove(fruit);
        }
    }

    IEnumerable<int> GetRandomIndices(int n, int k)
    {
        var indices = Enumerable.Range(0, n).ToArray();
        for (var i = 0; i < k; i++)
        {
            var r = UnityEngine.Random.Range(i, n);
            (indices[i], indices[r]) = (indices[r], indices[i]);
            yield return indices[i];
        }
    }

    public void ClearBoard()
    {
        foreach (var fruit in _activeFruits)
        {
            Destroy(fruit.gameObject);
        }

        _activeFruits.Clear();
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
