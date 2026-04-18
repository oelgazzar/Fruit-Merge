using System;
using UnityEngine;

public class FruitPreview : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateData(FruitModel fruitModel)
    {
        spriteRenderer.sprite = fruitModel.Sprite;
    }
}
