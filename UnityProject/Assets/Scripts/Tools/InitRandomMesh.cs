using System;
using System.Collections.Generic;
using UnityEngine;

public class InitRandomMesh : MonoBehaviour
{
    public int bones = 50;
    public float brushWidth = .08f;
    public Color splineColor = Color.cyan;
    public Easing.Style easing = Easing.Style.OutQuart;

    MeshFilter meshFilter;
    Renderer meshRenderer;
    Spline spline;

    int widthProperty, colorProperty, controlPointsProperty, numControlPointsProperty;

    void Start ()
    {
        meshRenderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();

        widthProperty = Shader.PropertyToID("_Width");
        colorProperty = Shader.PropertyToID("_Color");
        controlPointsProperty = Shader.PropertyToID("_ControlPoints");
        numControlPointsProperty = Shader.PropertyToID("_NumControlPoints");

        spline = Spline.CreateTest();
        var easingFunction = Easing.GetEasing(easing);
        meshFilter.mesh = CreateMeshForSplines(bones, easingFunction);
        meshRenderer.material.SetVectorArray(controlPointsProperty, spline.controlPoints);
        meshRenderer.material.SetFloat(numControlPointsProperty, 8f);
    }

    void Update()
    {
        meshRenderer.material.SetFloat(widthProperty, brushWidth);
        meshRenderer.material.SetColor(colorProperty, splineColor);
    }

    /*
     * Create a Mesh to match our Spline Shader
     * We create a rectangular "triangle strip" matching the number of bones
     * so for 2 bones we create |\|\|
     * each vertex will actually get placed from the control points passed to the shader
     * so we don't need to pass positions. Instead we pass other data to each vertex:
     * X = position in the spline (0 - 1)
     * Y = normalized distance from center of the spline (eg. left = -0.5 | right = 0.5)
     *   that way the spline can have a different width on the edges and in the middle
     * Z = (unused)
     */
    Mesh CreateMeshForSplines(int numBones, Func<float, float> easingFunction)
    {
        List<Vector3> verts = new List<Vector3>(numBones * 2);

        float maxBoneIndex = numBones - 1;
        for (int i = 0; i < numBones; i++)
        {
            float interval = i / maxBoneIndex; // 0 -> 1

            float linearWidth = 1f - Mathf.Abs(2f * interval - 1f); // curves from 0 -> 1 -> 0
            float halfWidth = easingFunction(linearWidth) * .5f;

            verts.Add(new Vector3(interval, .5f - halfWidth, 0));
            verts.Add(new Vector3(interval, .5f + halfWidth, 0));
        }

        int totalNumTri = (numBones - 1) * 2;
        int totalNumTriIdx = totalNumTri * 3;
        List<int> tris = new List<int>(totalNumTriIdx);
        for (int i = 0; i < numBones - 1; i++)
        {
            int vertIndex = i * 2;

            tris.Add(vertIndex + 1);
            tris.Add(vertIndex + 2);
            tris.Add(vertIndex + 3);

            tris.Add(vertIndex + 0);
            tris.Add(vertIndex + 2);
            tris.Add(vertIndex + 1);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors32 = new Color32[] { };
        mesh.UploadMeshData(false);
        return mesh;
    }
}
