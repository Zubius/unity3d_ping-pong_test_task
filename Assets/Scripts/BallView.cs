using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<MeshRenderer>();
    }

    internal void SetColor(Color color)
    {
        if (renderer != null)
        {
            renderer.sharedMaterial.color = color;
        }
    }
}
