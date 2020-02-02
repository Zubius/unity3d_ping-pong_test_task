using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : Bolt.EntityBehaviour<IPingBallState>
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

    public override void Attached()
    {
        state.SetTransforms(state.BallTransform, _cachedTransform);

        state.AddCallback("BallSize", SizeChanged);
        state.AddCallback("BallColor", ColorChanged);
    }

    internal void Launch(Vector2 direction, float speed)
    {
        if (entity.IsOwner)
        {
            _speed = speed;
            _velocity = direction;
            _isLaunched = true;
        }
    }

    internal void Stop()
    {
        _speed = 0;
        _velocity = Vector2.zero;
        _isLaunched = false;
    }

    internal void SetColor(Color color)
    {
        if (entity.IsOwner && _cachedRenderer != null)
        {
            state.BallColor = color;
        }
    }

    internal void SetSize(float size)
    {
        if (entity.IsOwner && _cachedTransform != null)
        {
            state.BallSize = new Vector3(size, size, size);
        }
    }

    private void ColorChanged()
    {
        _cachedRenderer.sharedMaterial.color = state.BallColor;
    }

    private void SizeChanged()
    {
        _cachedTransform.localScale = state.BallSize;
    }

    public override void SimulateOwner()
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
        if (other.transform.tag.Equals(ScoreTag))
        {
            OnScored?.Invoke(other);
            if (entity.IsOwner)
            {
                _isLaunched = false;
                rigidbody.velocity = Vector3.zero;
            }
        }

        if (entity.IsOwner && other.transform.tag.Equals(BounceTag))
        {
            Vector3 d, n, r;

            foreach (var contact in other.contacts)
            {
                d = _velocity;
                n = contact.normal;
                r = d - (2 * Vector3.Dot(d, n) * n);

                _velocity = r;
            }
        }
    }
}
