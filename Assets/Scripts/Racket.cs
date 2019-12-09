using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Racket : MonoBehaviour
{
    [SerializeField] private bool isMine;
    [SerializeField] private float speed = 1f;

    internal bool IsMine => isMine;

    [HideInInspector] internal bool CanMove = false;

    private Transform _cachedTransform;

    private IRacketInput _inputManager;

    private const float HorizontalConstraint = 11.22f;
    private Vector3 _lastTouchedPosition;

    internal void SetInput(IRacketInput inputManager)
    {
        _inputManager = inputManager;
    }

    private void OnEnable()
    {
        _cachedTransform = transform;
        CanMove = true;
        _lastTouchedPosition = _cachedTransform.position;
    }

    private void Update()
    {
        if (CanMove)
        {
            if (_inputManager?.HasInput ?? false)
            {
                Vector2 newPosition = _cachedTransform.position;

                float xPos = Mathf.Lerp(newPosition.x, _inputManager.Input, Time.deltaTime * speed);
                float clampedPos = Mathf.Clamp(xPos, -HorizontalConstraint, HorizontalConstraint);
                newPosition = new Vector2(clampedPos, newPosition.y);

                _cachedTransform.position = newPosition;
                _lastTouchedPosition = newPosition;
            }
        }
    }
}
