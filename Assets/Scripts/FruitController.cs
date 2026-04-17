using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitController : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _lineOriginOffset;
    [SerializeField] float _xRange;


    public static event Action OnFruitDropped;
    
    Fruit _activeFruit;

    float _indicatorStartY;

    private void Update()
    {
        if (_activeFruit == null) return;

        if (Mouse.current.leftButton.isPressed)
        {
            ToggleIndicator(true);
            var mousePos = Mouse.current.position.ReadValue();
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            var x = Mathf.Clamp(worldPos.x, -_xRange, _xRange);
            _activeFruit.transform.position = new Vector3(
                x, _activeFruit.transform.position.y, _activeFruit.transform.position.z);

            UpdateIndicator();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            DropFruit();
        }
        else
        {
            ToggleIndicator(false);
        }        
    }

    void DropFruit()
    {
        _activeFruit.GetComponent<Rigidbody2D>().simulated = true;
        _activeFruit = null;
        OnFruitDropped?.Invoke();
    }

    private void UpdateIndicator()
    {
        var origin = new Vector2(_activeFruit.transform.position.x, _indicatorStartY);
        var hit = Physics2D.Raycast(origin, Vector2.down);
        if (hit)
        {
            var lineEnd = hit.point;
            var midPoint = Vector2.Lerp(_activeFruit.transform.position, lineEnd, .5f);
            _lineRenderer.SetPositions(new Vector3[] { origin, midPoint, lineEnd });
        }
    }

    private void ToggleIndicator(bool value)
    {
        _lineRenderer.gameObject.SetActive(value);
    }

    private void OnEnable()
    {
        FruitSpawner.OnFruitSpawn += FruitSpawner_OnFruitSpawn;
    }

    private void FruitSpawner_OnFruitSpawn(Fruit fruit)
    {
        _activeFruit = fruit;
        var collider = _activeFruit.GetComponent<Collider2D>();
        _indicatorStartY = collider.bounds.min.y;
        var gradient = new Gradient();
        gradient.SetKeys(
            // COLOR keys (RGB)
            new GradientColorKey[]
            {
                new (fruit.Color, 0.5f),
            },

            // ALPHA keys (transparency)
            new GradientAlphaKey[]
            {
                new (0f, 0.0f),  // start transparent
                new (1f, 0.5f),  // middle visible
                new (0f, 1.0f),  // end transparent
            });
        _lineRenderer.colorGradient = gradient;
    }
}
