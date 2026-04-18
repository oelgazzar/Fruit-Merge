using System;
using UnityEngine;

/* 
 * Visual fruit component
 * Detect collisions and publish OnFruitContact event
 */

public class Fruit : MonoBehaviour
{
    public static event Action<Fruit, Fruit> OnFruitContact;

    FruitModel _model;
    public FruitModel Model => _model;

    bool _isMerged;
    public bool IsMerged => _isMerged;

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(FruitModel model)
    {
        _isMerged = false;
        _model = model;
        _spriteRenderer.sprite = model.Sprite;
        transform.localScale *= model.Size;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Fruit>(out var otherFruit))
        {
            if (otherFruit.Model.Tier == _model.Tier)
            {
                OnFruitContact?.Invoke(this, otherFruit);
            }
        }
    }

    public void MarkMerged() => _isMerged = true;
}
