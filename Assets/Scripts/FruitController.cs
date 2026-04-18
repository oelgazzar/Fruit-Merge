using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Control movement of active fruit
 * Show aim indicator
 * Publish Drop events
 */

public class FruitController : MonoBehaviour
{
    [SerializeField] Rect _dragArea;
    
    [Header("Indicator")]
    [SerializeField] AimIndictor _aimIndicator;
    [SerializeField] float _Indicatoroffset;

    public static event Action<Fruit> OnFruitDropStarted;
    public static event Action<Fruit> OnFruitDrop;
    
    Fruit _activeFruit;

    private void Update()
    {
        if (_activeFruit == null) return;

        HandleInput();
    }

    void HandleInput()
    {
        var mousePos = GetMousePosition();
        if (!_dragArea.Contains(mousePos)) return;

        if (Mouse.current.leftButton.isPressed)
        {
            ToggleIndicator(true);

            _activeFruit.transform.position = new Vector2(mousePos.x, _activeFruit.transform.position.y);

            RefreshIndicator();
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

    Vector2 GetMousePosition()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return worldPos;
    }

    void DropFruit()
    {
        var droppedFruit = _activeFruit;
        OnFruitDropStarted?.Invoke(droppedFruit);

        _activeFruit.GetComponent<Rigidbody2D>().simulated = true;
        _activeFruit = null;
        OnFruitDrop?.Invoke(droppedFruit);
    }

    private void RefreshIndicator()
    {
        var origin = new Vector2(
            _activeFruit.transform.position.x,
            _activeFruit.transform.position.y - _activeFruit.Model.Size/2);
        var hit = Physics2D.Raycast(origin, Vector2.down);
        if (hit)
        {
            _aimIndicator.SetPoints(origin, hit.point);
        }
    }

    private void ToggleIndicator(bool active)
    {
        _aimIndicator.gameObject.SetActive(active);
        if (active)
            _aimIndicator.SetColor(_activeFruit.Model.Color);
    }

    private void OnFruitSpawn(Fruit fruit)
    {
        _activeFruit = fruit;
    }

    private void OnEnable()
    {
        FruitSpawner.OnFruitSpawn += OnFruitSpawn;
    }    
}