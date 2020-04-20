using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoneDrawLibrary
{
    private static Material defaultMaterial;

    public static Mesh GetBone(float radius, float zlen)
    {
        const float mix = 0.1f;
        const int polyn = 4;
        
        Mesh m = new Mesh();

        Vector3 origin = Vector3.zero;
        Vector3 target = new Vector3(0, 0, zlen);

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        
        for (int i = 0; i < polyn; i++)
        {
            int vertInd = tris.Count;
            
            float theta1 = (i + 0) * 2 * Mathf.PI / polyn;
            float theta2 = (i + 1) * 2 * Mathf.PI / polyn;
            Vector3 vi = new Vector3(Mathf.Cos(theta1) * radius, Mathf.Sin(theta1) * radius, zlen * mix);
            Vector3 vp = new Vector3(Mathf.Cos(theta2) * radius, Mathf.Sin(theta2) * radius, zlen * mix);

            verts.AddRange(new Vector3[] {Vector3.zero, vp, vi, vi, vp, target});
            tris.AddRange(new int[] {vertInd + 0, vertInd + 1, vertInd + 2, vertInd + 3, vertInd + 4, vertInd + 5});
        }

        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();

        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
        
        return m;
    }

    public static Material GetDefaultMaterial()
    {
        if (defaultMaterial == null)
        {
            defaultMaterial = Resources.Load("BoneMaterial") as Material;
        }
        return (defaultMaterial);
    }
}
