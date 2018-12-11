using UnityEngine;

public class InitRandomMesh : MonoBehaviour
{
    public float brushWidth = .08f;
    public Color splineColor = Color.cyan;

    Renderer meshRenderer;
    Spline spline;

    int widthProperty, colorProperty, controlPointsProperty, numControlPointsProperty;

    void Start ()
    {
        meshRenderer = GetComponent<Renderer>();

        widthProperty = Shader.PropertyToID("_Width");
        colorProperty = Shader.PropertyToID("_Color");
        controlPointsProperty = Shader.PropertyToID("_ControlPoints");
        numControlPointsProperty = Shader.PropertyToID("_NumControlPoints");

        spline = Spline.CreateTest();
        meshRenderer.material.SetVectorArray(controlPointsProperty, spline.controlPoints);
        meshRenderer.material.SetFloat(numControlPointsProperty, 8f);
    }

    void Update()
    {
        meshRenderer.material.SetFloat(widthProperty, brushWidth);
        meshRenderer.material.SetColor(colorProperty, splineColor);
    }
}
