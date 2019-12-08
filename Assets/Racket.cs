using System;
using UnityEngine;

public class Racket : MonoBehaviour
{
    [SerializeField] private bool isMine;
    [SerializeField] private float speed = 1f;

    internal bool IsMine => isMine;

    [HideInInspector] internal bool CanMove = false;

    private Transform _cachedTransform;
    private bool _touched = false;

    private const float HorizontalConstraint = 11.22f;
    private Vector3 _lastTouchedPosition;

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
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                //NOTE Check touch on Racket;
//                if (touch.phase == TouchPhase.Began)
//                {
//                    var ray = Camera.main.ScreenPointToRay(touch.position);
//                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.Equals(this.gameObject))
//                    {
//                        _touched = true;
//                    }
//                    else
//                    {
//                        _touched = false;
//                    }
//                }
//
//                if (!_touched || touch.phase == TouchPhase.Ended)
//                    return;

                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    Vector2 newPosition = _cachedTransform.position;

                    var touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

                    float xPos = Mathf.Lerp(newPosition.x, touchPos.x, Time.deltaTime * speed);
                    float clampedPos = Mathf.Clamp(xPos, -HorizontalConstraint, HorizontalConstraint);
                    newPosition = new Vector2(clampedPos, newPosition.y);

                    _cachedTransform.position = newPosition;
                    _lastTouchedPosition = newPosition;
                }
            }
        }
    }
}
