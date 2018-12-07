using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawOnTexture : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // public Texture2D baseTexture;
    public int brushSize = 4; // size in pixels - TODO respect max

    readonly int maxBrushSize = 25;
    Color DefaultColor => Color.black;

    Camera m_Camera;
    GraphicRaycaster m_Raycaster;
    Image m_Image;
    Texture2D m_TextureToDraw;
    private bool res;

    public Color[] blackColors { get; private set; }

    void Start () {
        m_Image = GetComponent<Image>();
        m_Raycaster = GetComponent<GraphicRaycaster>();

        Assert.IsNotNull(m_Image);
        Assert.IsNotNull(m_Raycaster);

        m_Image.material = Instantiate(m_Image.material);
        m_TextureToDraw = Instantiate(m_Image.material.mainTexture) as Texture2D;
        m_Image.material.mainTexture = m_TextureToDraw;

        int maxBrushPixels = maxBrushSize * maxBrushSize;
        blackColors = new Color[maxBrushPixels];
        for (int i = 0; i < maxBrushPixels; i++)
        {
            blackColors[i] = DefaultColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(eventData, results);
        if (results.Any(r => r.gameObject == gameObject))
        {
            ApplyBrushAtPosition(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(eventData, results);
        if (results.Any(r => r.gameObject == gameObject))
        {
            ApplyBrushAtPosition(eventData.position);
        }
    }

    private void ApplyBrushAtPosition(Vector2 worldPosition)
    {
        Vector2 localpoint;
        var rectTransform = GetComponent<RectTransform>();

        if (m_Camera == null)
            m_Camera = GetComponentInParent<Canvas>().worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, worldPosition, m_Camera, out localpoint);

        Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);

        Texture2D tex = m_TextureToDraw;
        Vector2Int pixelUV = new Vector2Int((int)(normalizedPoint.x * tex.width), (int)(normalizedPoint.y * tex.height));

        int halfBrush = brushSize / 2;
        RectInt brushRect = new RectInt(pixelUV.x - halfBrush, pixelUV.y - halfBrush, brushSize, brushSize);

        ResizeRectToFit(ref brushRect, new RectInt(0, 0, tex.width, tex.height));
        tex.SetPixels(brushRect.xMin, brushRect.yMin, brushRect.width, brushRect.height, blackColors);
        tex.Apply();
    }

    private void DebugFillTexWithGradient(Texture2D tex)
    {
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                tex.SetPixel(i, j, Color.Lerp(Color.red, Color.Lerp(Color.blue, Color.green, (float)j / tex.height), (float)i / tex.height));
            }
        }
        tex.Apply();
    }

    private static void ResizeRectToFit(ref RectInt rect, RectInt constraint)
    {
        int leftOverflow = constraint.xMin - rect.xMin;
        if (leftOverflow > 0)
        {
            rect.width -= leftOverflow;
            rect.xMin = constraint.xMin;
        }
        int topOverflow = constraint.yMin - rect.yMin;
        if (topOverflow > 0)
        {
            rect.height -= topOverflow;
            rect.yMin = constraint.yMin;
        }
        int rightOverflow = rect.xMax - constraint.xMax;
        if (rightOverflow > 0)
        {
            rect.width -= rightOverflow;
            rect.xMax = constraint.xMax;
        }
        int bottomOverflow = rect.yMax - constraint.yMax;
        if (bottomOverflow > 0)
        {
            rect.height -= bottomOverflow;
            rect.yMax = constraint.yMax;
        }
    }
}
