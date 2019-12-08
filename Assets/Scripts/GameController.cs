using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        Restart();
    }

    private void Restart()
    {
        _myScore = 0;
        _rivalScore = 0;

        LaunchBall();
    }

    private void LaunchBall()
    {
        ball.transform.localPosition = Vector3.zero;
        ball.SetSize(Random.Range(0.1f, 1f));
        ball.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        ball.Launch(new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)), Random.Range(1f, 10f));
    }

    private void OnScored(Collision obj)
    {
        if (obj.transform.localPosition.y > 0)
            _myScore++;

        if (obj.transform.localPosition.y < 0)
            _rivalScore++;

        ball.transform.localPosition = new Vector2(10000, 10000);
    }

    private void OnDestroy()
    {
        if (ball != null)
        {
            ball.OnScored = null;
        }
    }
}
