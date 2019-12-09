using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallView ball;

    [SerializeField] private Text countText;
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text rivalScoreText;

    [SerializeField] private TouchInputController touchInput;

    [SerializeField] private Racket playerRacket;
    [SerializeField] private Racket rivalRacket;

    private int _myScore, _rivalScore;
    private readonly WaitForSeconds _waitFor1Sec = new WaitForSeconds(1);

    private const string myScoreKey = "my_best_score";
    private const string rivalScoreKey = "rival_best_score";

    private Dictionary<InputType, IRacketInput> _inputs;

    private void Start()
    {
        if (ball != null)
        {
            ball.OnScored += OnScored;
            ball.transform.localPosition = new Vector2(10000, 10000);
        }

        _inputs = new Dictionary<InputType, IRacketInput>
        {
            {InputType.Touch, touchInput}
        };

        playerRacket.SetInput(touchInput);

        Restart(InputType.Touch);
    }

    private void Restart(InputType inputType)
    {
        _myScore = 0;
        _rivalScore = 0;

        myScoreText.text = _myScore.ToString();
        rivalScoreText.text = _rivalScore.ToString();

        rivalRacket.SetInput(_inputs[inputType]);

        StartCoroutine(LaunchBall());
    }

    private IEnumerator LaunchBall()
    {
        for (int i = 3; i > 0; i--)
        {
            countText.text = i.ToString();
            yield return _waitFor1Sec;
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

        int myBest = PlayerPrefs.GetInt(myScoreKey, 0);
        int rivalBest = PlayerPrefs.GetInt(rivalScoreKey, 0);

        PlayerPrefs.SetInt(myScoreKey, Mathf.Max(myBest, _myScore));
        PlayerPrefs.SetInt(rivalScoreKey, Mathf.Max(rivalBest, _rivalScore));
    }
}

internal enum InputType
{
    None = 0,
    Touch,
    Photon,
}
