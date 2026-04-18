using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FruitController : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _lineOriginOffset;
    [SerializeField] float _xRange;


    public static event Action<Fruit> OnFruitDropStarted;
    public static event Action<Fruit> OnFruitDropCompleted;
    
    Fruit _activeFruit;

    float _indicatorStartY;

    private void Update()
    {
        if (_activeFruit == null) return;

        var mousePos = Mouse.current.position.ReadValue();
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        if (worldPos.y > 2) return;
        if (Mouse.current.leftButton.isPressed)
        {
            ToggleIndicator(true);
            var x = Mathf.Clamp(worldPos.x, -_xRange, _xRange);
            _activeFruit.transform.position = new Vector3(
                x, _activeFruit.transform.position.y, _activeFruit.transform.position.z);

            UpdateIndicator();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && worldPos.y < 2)
        {
            if (worldPos.y < 2)
                DropFruit();
        }
        else
        {
            ToggleIndicator(false);
        }        
    }

    void DropFruit()
    {
        var fruit = _activeFruit;
        OnFruitDropStarted?.Invoke(fruit);

        _activeFruit.GetComponent<Rigidbody2D>().simulated = true;
        _activeFruit = null;
        OnFruitDropCompleted?.Invoke(fruit);
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
        //if (_activeFruit != null) Destroy(_activeFruit.gameObject);

        _activeFruit = fruit;
        var collider = _activeFruit.GetComponent<Collider2D>();
        _indicatorStartY = collider.bounds.min.y;
        var gradient = new Gradient();
        gradient.SetKeys(
            // COLOR keys (RGB)
            new GradientColorKey[]
            {
                new (fruit.Model.Color, 0.5f),
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
