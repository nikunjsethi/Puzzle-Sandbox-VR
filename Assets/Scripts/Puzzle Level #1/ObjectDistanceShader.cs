using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistanceShader : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Shader shader;

    private void Start()
    {
        Material material = new Material(shader);
        material.SetVector("_FinalPoint", lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        GetComponent<Renderer>().material = material;
    }
}
