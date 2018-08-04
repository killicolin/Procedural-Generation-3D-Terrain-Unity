using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer texture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
    public void DrawTexture(Texture2D texture2D)
    {
        texture.sharedMaterial.mainTexture = texture2D;
        texture.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
    }
}
