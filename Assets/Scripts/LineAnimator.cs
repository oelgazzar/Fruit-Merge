using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    [SerializeField] float _speed = 2f;

    Material _material;

    private void Awake()
    {
        _material = GetComponent<LineRenderer>().material;
    }

    private void Update()
    {
        _material.mainTextureOffset -= new Vector2(_speed * Time.deltaTime, 0);
    }
}
