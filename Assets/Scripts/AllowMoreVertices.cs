using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class AllowMoreVertices : MonoBehaviour
{
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        _meshFilter.sharedMesh.indexFormat = IndexFormat.UInt32;
        _meshFilter.sharedMesh.UploadMeshData(true);
    }
    
}

