using System;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public static event Action<Fruit, Fruit> OnFruitContact;

    public FruitModel Model => _model;
    public string ID => _id;
    public bool IsMerged => _isMerged;

    FruitModel _model;
    string _id;
    bool _isMerged;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _id = Guid.NewGuid().ToString();
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

    public void MarkAsMerged() => _isMerged = true;
}
