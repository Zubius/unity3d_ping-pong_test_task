using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : Bolt.GlobalEventListener
{
    [SerializeField] private BallView ball;

    [SerializeField] private Text countText;
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text rivalScoreText;

    [SerializeField] private TouchInputController touchInput;
    [SerializeField] private NetworkController network;

    [SerializeField] private Racket playerRacket;
    [SerializeField] private Racket rivalRacket;

    [SerializeField] private GameObject menu;



    private int _myScore, _rivalScore, _myBestScore, _rivalBestScore;
    private readonly WaitForSeconds _waitFor1Sec = new WaitForSeconds(1);
    private bool _menuIsActive = false;

    private const string myScoreKey = "my_best_score";
    private const string rivalScoreKey = "rival_best_score";
    private const string bestScoreFormat = "{0} ({1})";

    private InputType _inputType = InputType.None;

    private Dictionary<InputType, IRacketInput> _inputs;

    private Coroutine _launchCoroutine = null;

    private void Start()
    {
        if (ball != null)
        {
            ball.OnScored = OnScored;
        }

        _inputs = new Dictionary<InputType, IRacketInput>
        {
            {InputType.Touch, touchInput}
        };

        playerRacket.SetInput(touchInput);

        _myBestScore = PlayerPrefs.GetInt(myScoreKey, 0);
        _rivalBestScore = PlayerPrefs.GetInt(rivalScoreKey, 0);

        myScoreText.text = string.Format(bestScoreFormat, _myScore.ToString(), _myBestScore.ToString());
        rivalScoreText.text = string.Format(bestScoreFormat, _rivalScore.ToString(), _rivalBestScore.ToString());

        _inputType = InputType.Touch;

        network.OnConnected += OnConnected;
        network.OnDisonnected += OnDisonnected;
        network.ConnectNetwork();
        // network.ConnectLocal();
        network.OnEntityReceived += OnEntityReceived;
    }

    private void OnDisonnected()
    {
        if (network.State == ConnectState.ConnectedAsServer)
        {
            ball.Stop();
        }

        Application.Quit();
    }

    public void OnMenuPressed()
    {
        _menuIsActive = !_menuIsActive;
        menu.SetActive(_menuIsActive);

        Time.timeScale = _menuIsActive ? 0 : 1;
    }

    public void OnRestartPressed()
    {
        Restart(_inputType);
    }

    private void Restart(InputType inputType)
    {
        _menuIsActive = false;
        menu.SetActive(_menuIsActive);
        Time.timeScale = 1;

        _myScore = 0;
        _rivalScore = 0;

        myScoreText.text = string.Format(bestScoreFormat, _myScore.ToString(), _myBestScore.ToString());
        rivalScoreText.text = string.Format(bestScoreFormat, _rivalScore.ToString(), _rivalBestScore.ToString());

        rivalRacket.SetInput(_inputs[inputType]);

        bool asServer = network.State == ConnectState.ConnectedAsServer;
        if (asServer)
        {
            ball.transform.localPosition = new Vector2(10000, 10000);
            ball.SetSize(Random.Range(0.1f, 1f));
            ball.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

        }

        if (_launchCoroutine != null)
            StopCoroutine(_launchCoroutine);
        _launchCoroutine = StartCoroutine(LaunchBall(asServer));
    }

    private void OnConnected()
    {
        ball = BoltNetwork.Instantiate(BoltPrefabs.Ball, Vector3.zero, Quaternion.identity).GetComponent<BallView>();

        StartGame(ball);
    }

    private void OnEntityReceived(BoltEntity entity)
    {
        if (entity.StateIs<IPingBallState>())
        {
            Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 180);
            ball = entity.GetComponent<BallView>();
            StartGame(ball);
        }
    }

    private void StartGame(BallView ballб )
    {
        ball.OnScored = OnScored;
        Restart(_inputType);
    }

    private IEnumerator LaunchBall(bool asServer)
    {
        for (int i = 3; i > 0; i--)
        {
            countText.text = i.ToString();
            yield return _waitFor1Sec;
        }
        countText.text = string.Empty;

        if (asServer)
        {
            ball.transform.localPosition = Vector3.zero;
            ball.Launch(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Random.Range(1f, 10f));
        }
    }

    private void OnScored(Collision obj)
    {
        if (obj.transform.localPosition.y > 0)
        {
            _myScore++;
            _myBestScore = Mathf.Max(_myScore, _myBestScore);
            myScoreText.text = string.Format(bestScoreFormat, _myScore.ToString(), _myBestScore.ToString());
        }

        if (obj.transform.localPosition.y < 0)
        {
            _rivalScore++;
            _rivalBestScore = Mathf.Max(_rivalScore, _rivalBestScore);
            rivalScoreText.text = string.Format(bestScoreFormat, _rivalScore.ToString(), _rivalBestScore.ToString());
        }

        ball.transform.localPosition = new Vector2(10000, 10000);

        if (_launchCoroutine != null)
            StopCoroutine(_launchCoroutine);
        StartCoroutine(LaunchBall(network.State == ConnectState.ConnectedAsServer));
    }

    private void OnDestroy()
    {
        if (ball != null)
        {
            ball.OnScored = null;
        }

        SaveBestScores();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveBestScores();
    }

    private void SaveBestScores()
    {
        PlayerPrefs.SetInt(myScoreKey, Mathf.Max(_myBestScore, _myScore));
        PlayerPrefs.SetInt(rivalScoreKey, Mathf.Max(_rivalBestScore, _rivalScore));
        PlayerPrefs.Save();
    }
}

internal enum InputType
{
    None = 0,
    Touch,
    Photon,
}
