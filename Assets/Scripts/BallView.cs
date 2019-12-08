using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : MonoBehaviour
{
    [SerializeField] private Rigidbody rigibody;

    internal Action<Collision> OnScored;

    private Transform _cachedTransform;
    private MeshRenderer _cachedRenderer;

    private const string ScoreTag = "Finish";
    private const string BounceTag = "Bounce";

    private void Start()
    {
        _cachedRenderer = GetComponent<MeshRenderer>();
        _cachedTransform = transform;

        Launch(new Vector3(1000, 0));
    }

    internal void Launch(Vector2 direction)
    {
        rigibody.AddForce(direction);
    }

    internal void SetColor(Color color)
    {
        if (_cachedRenderer != null)
        {
            _cachedRenderer.sharedMaterial.color = color;
        }
    }

    internal void SetSize(int size)
    {
        if (_cachedTransform != null)
        {
            _cachedTransform.localScale = new Vector3(size, size, size);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"{name} {nameof(OnCollisionEnter)} {other.gameObject.name} {other.transform.tag}");

        if (other.transform.tag.Equals(ScoreTag))
        {
            OnScored?.Invoke(other);
        }

        if (other.transform.tag.Equals(BounceTag))
        {
            Launch(new Vector2());
        }
    }
}
