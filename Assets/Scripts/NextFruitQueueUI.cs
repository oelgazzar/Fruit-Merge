using System.Collections.Generic;
using UnityEngine;

public class NextFruitQueueUI : MonoBehaviour
{
    [SerializeField] Transform[] _placementPoints;
    [SerializeField] FruitPreview _fruitPreviewPrefab;

    FruitPreview[] _previews;

    private void Start()
    {
        _previews = new FruitPreview[_placementPoints.Length];
    }

    void UpdateQueue(FruitModel[] queue)
    {
        for (var i = 0;  i < _previews.Length; i++)
        {
            var preview = _previews[i];
            if (preview == null)
            {
                preview = Instantiate(_fruitPreviewPrefab, _placementPoints[i].position, Quaternion.identity);
                _previews[i] = preview;
            }
            preview.UpdateData(queue[i]);
        }
    }

    private void OnEnable()
    {
        FruitSpawner.OnNextQueueUpdated += UpdateQueue;
    }

    private void OnDisable()
    {
        FruitSpawner.OnNextQueueUpdated -= UpdateQueue;
    }
}
