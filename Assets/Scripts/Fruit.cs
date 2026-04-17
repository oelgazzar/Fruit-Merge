using System;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public static event Action<Fruit, Fruit> OnFruitContact;

    public int Type => _type;
    public Color Color => _color;
    public bool IsMerged => _isMerged;

    int _type;
    Color _color;
    bool _isMerged;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(FruitModel model)
    {
        _type = model.Type;
        _color = model.Color;
        _spriteRenderer.sprite = model.Sprite;
        transform.localScale *= model.Size;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Fruit>(out var otherFruit))
        {
            if (otherFruit.Type == Type)
            {
                OnFruitContact?.Invoke(this, otherFruit);
                _isMerged = true;
            }
        }
    }
}
