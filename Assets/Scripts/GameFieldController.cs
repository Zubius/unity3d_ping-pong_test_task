using System;
using UnityEngine;

public class GameFieldController : MonoBehaviour
{
    [SerializeField] private Racket playerRacket;
    [SerializeField] private Racket rivalRacket;

    [SerializeField] private Transform gameField;

    internal static GameFieldController SharedController;

    internal Racket PlayerRacket => playerRacket;
    internal Racket RivalRacket => rivalRacket;
    internal Transform GameField => gameField;

    private void Awake()
    {
        SharedController = this;
    }

    internal void SetGameFieldRotation(Quaternion rotation)
    {
        if (gameField != null)
            gameField.localRotation = rotation;
    }
}
