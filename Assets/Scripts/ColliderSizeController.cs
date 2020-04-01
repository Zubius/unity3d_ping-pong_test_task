using System;
using UnityEngine;

public class ColliderSizeController : MonoBehaviour
{
    public Transform[] horizontalColliders;
    public Transform[] verticalColliders;

    private void Awake()
    {
        var height = Camera.main.orthographicSize * 2;
        var width = height * Screen.width/ Screen.height;

        var horizontalScale = new Vector3(width, 1, 1);
        var verticalScale = new Vector3(1, height, 1);
        float topY = height / 2 + 0.5f;
        float leftX = width / 2 + 0.5f;

        horizontalColliders[0].position = Vector3.up * topY;
        horizontalColliders[1].position = Vector3.down * topY;
        verticalColliders[0].position = Vector3.left * leftX;
        verticalColliders[1].position = Vector3.right * leftX;

        for (int i = 0; i < horizontalColliders.Length; i++)
        {
            horizontalColliders[i].localScale = horizontalScale;
        }

        for (int i = 0; i < verticalColliders.Length; i++)
        {
            verticalColliders[i].localScale = verticalScale;
        }
    }
}
