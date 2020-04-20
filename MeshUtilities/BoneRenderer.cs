using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BoneRenderer : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {

    }

    [ContextMenu("Rebuild mesh")]
    public void RebuildMesh()
    {
        if (mf == null)
        {
            mf = transform.GetComponent<MeshFilter>();
        }
        if (mr == null)
        {
            mr = transform.GetComponent<MeshRenderer>();
        }

        mf.sharedMesh = BoneDrawLibrary.GetBone(0.3f, 2.0f);
        mr.sharedMaterial = BoneDrawLibrary.GetDefaultMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
