using System;
using UnityEngine;

public class TouchInputController : MonoBehaviour, IRacketInput
{
    public bool HasInput => _hasInput;

    public float Input => _input;

    private bool _hasInput = false;
    private float _input = 0;

    private void Update()
    {
        if (UnityEngine.Input.touchCount > 0)
        {
            var touch = UnityEngine.Input.GetTouch(0);

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
                var touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

                _input = touchPos.x;
            }
        }
    }
}
