using UnityEngine;

public class AimIndictor : MonoBehaviour
{
    LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetColor(Color color)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            // COLOR keys (RGB)
            new GradientColorKey[]
            {
                new (color, 0.5f),
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

    public void SetPoints(Vector3 start, Vector3 end)
    {
        // Necessary for shader to work properly
        var midPoint = Vector2.Lerp(start, end, .5f);

        _lineRenderer.SetPositions(new Vector3[] { start, midPoint, end });
    }
}