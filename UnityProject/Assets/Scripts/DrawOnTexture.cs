using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawOnTexture : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // public Texture2D baseTexture;
    public int brushSize = 1; // size in pixels - TODO respect max

    readonly int maxBrushSize = 4;
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(eventData, results);
        if (results.Any(r => r.gameObject == gameObject))
        {
            Vector2 localpoint;
            var rectTransform = GetComponent<RectTransform>();

            if (m_Camera == null)
                m_Camera = GetComponentInParent<Canvas>().worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, m_Camera, out localpoint);

            Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);

            Texture2D tex = m_TextureToDraw;
            Vector2Int pixelUV = new Vector2Int((int)(normalizedPoint.x * tex.width), (int)(normalizedPoint.y * tex.height));

            //for (int i = 0; i < tex.width; i++)
            //{
            //    for (int j = 0; j < tex.height; j++)
            //    {
            //        tex.SetPixel(i, j, Color.Lerp(Color.red, Color.Lerp(Color.blue, Color.green, (float) j / tex.height), (float) i / tex.height));
            //    }
            //}

            // TODO fix out of bounds

            var halfBrushSize = brushSize / 2;
            tex.SetPixels(pixelUV.x - halfBrushSize, pixelUV.y - halfBrushSize, brushSize, brushSize, blackColors);
            tex.Apply();
        }
    }
}
