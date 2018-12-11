using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BakeSplineMesh
{
    static Func<float, float> easing = Easing.GetEasing(Easing.Style.OutQuint);

    const string defaultPath = "Assets/Binaries/Mesh/SplineMeshes/";

    enum SplineMeshSize
    { Small, Medium, Large };

    static readonly Dictionary<SplineMeshSize, int> bonesPerSplineSize = new Dictionary<SplineMeshSize, int>
    {
        { SplineMeshSize.Small, 50 },
        { SplineMeshSize.Medium, 200 },
        { SplineMeshSize.Large, 600 },
    };

    [MenuItem("Assets/Spline Mesh/Create Small...")]
    static void CreateSplineMeshSmall()
    {
        CreateAndSaveSplineMesh(SplineMeshSize.Small);
    }

    [MenuItem("Assets/Spline Mesh/Create Medium...")]
    static void CreateSplineMeshMedium()
    {
        CreateAndSaveSplineMesh(SplineMeshSize.Medium);
    }

    [MenuItem("Assets/Spline Mesh/Create Large...")]
    static void CreateSplineMeshLarge()
    {
        CreateAndSaveSplineMesh(SplineMeshSize.Large);
    }

    static void CreateAndSaveSplineMesh(SplineMeshSize size)
    {
        string name = "SplineMesh_" + Enum.GetName(typeof(SplineMeshSize), size);
        string path = EditorUtility.SaveFilePanel("Save Spline Mesh", defaultPath, name, "asset");

        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path);
            Mesh mesh = CreateMeshForSplines(bonesPerSplineSize[size], easing);
            MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }

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
    static Mesh CreateMeshForSplines(int numBones, Func<float, float> easingFunction)
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
