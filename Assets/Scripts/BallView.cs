using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    internal Action<Collision> OnScored;

    private Transform _cachedTransform;
    private MeshRenderer _cachedRenderer;

    private const string ScoreTag = "Finish";
    private const string BounceTag = "Bounce";

    private float _speed = 0;
    private Vector2 _velocity;
    private bool _isLaunched = false;

    private void OnEnable()
    {
        _cachedRenderer = GetComponent<MeshRenderer>();
        _cachedTransform = transform;
    }

    internal void Launch(Vector2 direction, float speed)
    {
        Debug.Log($"{direction}, {speed}");
        _speed = speed;
        _velocity = direction;
        _isLaunched = true;
    }

    internal void SetColor(Color color)
    {
        if (_cachedRenderer != null)
        {
            _cachedRenderer.sharedMaterial.color = color;
        }
    }

    internal void SetSize(float size)
    {
        if (_cachedTransform != null)
        {
            _cachedTransform.localScale = new Vector3(size, size, size);
        }
    }

    private void Update()
    {
        if (_isLaunched)
        {
            _velocity = _velocity.normalized * _speed;
            rigidbody.velocity = _velocity;

            Debug.DrawRay(transform.position, rigidbody.velocity, Color.green);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"{name} {nameof(OnCollisionEnter)} {other.gameObject.name} {other.transform.tag}");

        if (other.transform.tag.Equals(ScoreTag))
        {
            OnScored?.Invoke(other);
            _isLaunched = false;
            rigidbody.velocity = Vector3.zero;
        }

        if (other.transform.tag.Equals(BounceTag))
        {
            Vector3 d, n, r;

            foreach (var contact in other.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.red, 10);

                d = _velocity;
                n = contact.normal;
                r = d - (2 * Vector3.Dot(d, n) * n);

                _velocity = r;
            }
        }
    }
}
