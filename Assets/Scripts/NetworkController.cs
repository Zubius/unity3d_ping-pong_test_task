using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;

public class NetworkController : Bolt.GlobalEventListener
{
    public ConnectState  State => _state;
    public event Action OnConnected;
    public event Action OnDisonnected;
    public event Action<BoltEntity> OnEntityReceived;

    private ConnectState _state = ConnectState.Disconnected;
    private bool _waitingInitialRoomLiat = false;
    private float _waitingRoomListTimer = 5f;
    private UdpSession _session;
    private BoltConnection _connection;

    public void ConnectNetwork()
    {
        Connect(false);
    }

    public void ConnectLocal()
    {
       Connect(true);
    }

    private void Connect(bool local)
    {
        _state = ConnectState.ClientConnecting;

        if (local)
            BoltLauncher.StartSinglePlayer();
        else
            BoltLauncher.StartClient();
    }

    private void Update()
    {
        if (_waitingInitialRoomLiat)
        {
            _waitingRoomListTimer -= BoltNetwork.FrameDeltaTime;
            if (_waitingRoomListTimer <= 0)
            {
                _state = ConnectState.ReconnectingFromClientToServer;
                _waitingInitialRoomLiat = false;
                BoltLauncher.Shutdown();
            }
        }
    }

    #region Callbacks
    public override void BoltStartDone()
    {
        Debug.LogError($"{nameof(BoltNetwork.IsServer)}: {BoltNetwork.IsServer}, " +
                       $"{nameof(BoltNetwork.IsClient)}: {BoltNetwork.IsClient}, " +
                       $"{nameof(BoltNetwork.IsSinglePlayer)}: {BoltNetwork.IsSinglePlayer}");

        if (BoltNetwork.IsClient)
        {
            _waitingInitialRoomLiat = true;
            _waitingRoomListTimer = 5f;
            _state = ConnectState.ConnectedAsClient;
        }

        if (BoltNetwork.IsServer && !BoltNetwork.IsSinglePlayer)
        {
            string matchName = Guid.NewGuid().ToString();
            _state = ConnectState.ConnectedAsServer;

            BoltMatchmaking.CreateSession(matchName);
        }

        if (BoltNetwork.IsSinglePlayer)
        {
            _state = ConnectState.ConnectedAsServer;
            OnConnected?.Invoke();
        }
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        registerDoneCallback(() =>
        {
            switch (_state)
            {
                case ConnectState.Disconnected:
                    break;
                case ConnectState.ClientConnecting:
                    break;
                case ConnectState.ReconnectingFromClientToServer:
                    _state = ConnectState.ServerConnecting;
                    BoltLauncher.StartServer();
                    break;
            }
        });
    }

    public override void Disconnected(BoltConnection connection)
    {
        Debug.LogError("Disconnected");
        _state = ConnectState.Disconnected;
        OnDisonnected?.Invoke();
    }

    public override void Connected(BoltConnection connection)
    {
        Debug.LogError("Connected");
        _connection = connection;

        if (BoltNetwork.IsServer)
            OnConnected?.Invoke();
    }

    public override void EntityReceived(BoltEntity entity)
    {
        Debug.LogError("EntityReceived");
        OnEntityReceived?.Invoke(entity);
    }

    public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
    {
        Debug.LogError("ConnectAttempt");
    }

    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        Debug.LogError("ConnectFailed");
    }

    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
    {
        Debug.LogError("ConnectRefused");
    }

    public override void SessionCreated(UdpSession session)
    {
        Debug.LogError("SessionCreated");
        _session = session;
    }

    public override void SessionConnected(UdpSession session, IProtocolToken token)
    {
        Debug.LogError("SessionConnected");
    }

    public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
    {
        Debug.LogError("SessionConnectFailed");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        _waitingInitialRoomLiat = false;
        Debug.LogError("SessionListUpdated");
        BoltMatchmaking.JoinRandomSession();
    }
    #endregion
}

public enum ConnectState
{
    Disconnected,
    ClientConnecting,
    ServerConnecting,
    ConnectedAsClient,
    ConnectedAsServer,
    ReconnectingFromClientToServer,
}
