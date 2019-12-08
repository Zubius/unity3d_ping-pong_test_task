using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallView ball;
    [SerializeField] private IRacketInput playerRacket;
    [SerializeField] private IRacketInput rivalRacket;

    [SerializeField] private Text countText;
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text rivalScoreText;

    private int _myScore, _rivalScore;
    private WaitForSeconds _waitFor1sec = new WaitForSeconds(1);

    private void Start()
    {
        if (ball != null)
        {
            ball.OnScored += OnScored;
        }

        ball.transform.localPosition = new Vector2(10000, 10000);
        Restart();
    }

    private void Restart()
    {
        _myScore = 0;
        _rivalScore = 0;

        myScoreText.text = _myScore.ToString();
        rivalScoreText.text = _rivalScore.ToString();

        StartCoroutine(LaunchBall());
    }

    private IEnumerator LaunchBall()
    {
        for (int i = 3; i > 0; i--)
        {
            countText.text = i.ToString();
            yield return _waitFor1sec;
        }
        countText.text = string.Empty;

        ball.transform.localPosition = Vector3.zero;
        ball.SetSize(Random.Range(0.1f, 1f));
        ball.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        ball.Launch(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Random.Range(1f, 10f));
    }

    private void OnScored(Collision obj)
    {
        if (obj.transform.localPosition.y > 0)
        {
            _myScore++;
            myScoreText.text = _myScore.ToString();
        }

        if (obj.transform.localPosition.y < 0)
        {
            _rivalScore++;
            rivalScoreText.text = _rivalScore.ToString();
        }

        ball.transform.localPosition = new Vector2(10000, 10000);
        StartCoroutine(LaunchBall());
    }

    private void OnDestroy()
    {
        if (ball != null)
        {
            ball.OnScored = null;
        }
    }
}
