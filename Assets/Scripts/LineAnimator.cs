using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    Material _material;

    private void Awake()
    {
        _material = GetComponent<LineRenderer>().material;
    }

    private void Update()
    {
        _material.mainTextureOffset -= new Vector2(2 * Time.deltaTime, 0);
    }
}
