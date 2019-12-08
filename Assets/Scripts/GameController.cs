using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallView ball;
    [SerializeField] private IRacketInput playerRacket;
    [SerializeField] private IRacketInput rivalRacket;

    private int _myScore, _rivalScore;

    private void Start()
    {
        if (ball != null)
        {
            ball.OnScored += OnScored;
        }

        _myScore = 0;
        _rivalScore = 0;
    }

    private void OnScored(Collision obj)
    {
        if (obj.transform.localPosition.y > 0)
            _rivalScore++;

        if (obj.transform.localPosition.y < 0)
            _myScore++;
    }

    private void OnDestroy()
    {
        if (ball != null)
        {
            ball.OnScored = null;
        }
    }
}
