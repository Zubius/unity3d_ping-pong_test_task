using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Bolt;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : Bolt.GlobalEventListener
{
    [SerializeField] private BallView ball;

    [SerializeField] private TouchInputController touchInput;
    [SerializeField] private PhotonInputController photonInput;

    [SerializeField] private NetworkController network;

    private UIController _uiController;
    private GameFieldController _gameFieldController;

    private readonly WaitForSeconds _waitFor1Sec = new WaitForSeconds(1);
    private Action _onGameSceneLoadedAction;

    private InputType _inputType = InputType.None;

    private Dictionary<InputType, IRacketInput> _inputs;

    private Coroutine _launchCoroutine = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (ball != null)
        {
            ball.OnScored = OnScored;
        }

        _inputs = new Dictionary<InputType, IRacketInput>
        {
            {InputType.Touch, touchInput},
            {InputType.Photon, photonInput}
        };

        network.OnConnected += OnConnected;
        network.OnDisonnected += OnDisonnected;
        network.OnEntityReceived += OnEntityReceived;

        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (scene.name.Equals("GameScene"))
        {
            _uiController = UIController.SharedController;
            _gameFieldController = GameFieldController.SharedController;
            _onGameSceneLoadedAction?.Invoke();

            _gameFieldController.PlayerRacket.SetInput(touchInput);
            _uiController.ResetScores();

            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
        }
    }

    public void U_StartSinglePlayer()
    {
        _onGameSceneLoadedAction = () =>
        {
            _inputType = InputType.Touch;
            network.ConnectLocal();
        };
        LoadGameScene();
    }

    public void U_StartMultiPlayer()
    {
        _onGameSceneLoadedAction = () =>
        {
            _inputType = InputType.Photon;
            network.ConnectNetwork();
        };
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnDisonnected()
    {
        if (network.State == ConnectState.ConnectedAsServer)
        {
            ball.Stop();
        }

        Application.Quit();
    }

    public void OnRestartPressed()
    {
        Restart(_inputType);
    }

    private void Restart(InputType inputType)
    {
        _uiController.HideMenu();
        _uiController.ResetScores();

        _gameFieldController.RivalRacket.SetInput(_inputs[inputType]);

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
        ball.transform.SetParent(_gameFieldController.GameField);

        StartGame(ball);
    }

    private void OnEntityReceived(BoltEntity entity)
    {
        if (entity.StateIs<IPingBallState>())
        {
            _gameFieldController.SetGameFieldRotation(Quaternion.Euler(0, 0, 180));
            ball = entity.GetComponent<BallView>();
            ball.transform.SetParent(_gameFieldController.GameField);
            StartGame(ball);
        }
    }

    private void StartGame(BallView ball)
    {
        ball.OnScored = OnScored;
        Restart(_inputType);
    }

    private IEnumerator LaunchBall(bool asServer)
    {
        for (int i = 3; i > 0; i--)
        {
            _uiController.SetCounter(i);
            yield return _waitFor1Sec;
        }
        _uiController.SetCounter(0);

        if (asServer)
        {
            var angle = GetAngle(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            var speed = Random.Range(3f, 10f);

            var d = angle % 90;
            if (d <= 10)
            {
                angle += 10;
            }
            else if (d >= 80)
            {
                angle -= 10;
            }

            var rad = angle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            ball.transform.localPosition = Vector3.zero;
            ball.Launch(direction, speed);
        }
    }

    private float GetAngle(Vector3 vector)
    {
        var ang = Mathf.Asin(vector.x) * Mathf.Rad2Deg;

        if (vector.y < 0)
        {
            ang = 180 - ang;
        }
        else if (vector.x < 0)
        {
            ang = 360 + ang;
        }

        return ang;
    }

    private void OnScored(Collision obj)
    {
        if (obj.transform.localPosition.y > 0)
        {
            _uiController.ScoreMe();
        }

        if (obj.transform.localPosition.y < 0)
        {
           _uiController.ScoreRival();
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
    }
}

internal enum InputType
{
    None = 0,
    Touch,
    Photon,
}
