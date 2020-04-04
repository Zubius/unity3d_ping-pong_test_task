using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text countText;
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text rivalScoreText;

    [SerializeField] private GameObject menu;

    internal static UIController SharedController;
    private ScoreController _scoreController;

    private bool _menuIsActive = false;

    private void Awake()
    {
        SharedController = this;
        _scoreController = new ScoreController();
    }

    internal void ShowMenu()
    {
        menu.SetActive(true);
        _menuIsActive = true;
        Time.timeScale = 0;
    }

    internal void HideMenu()
    {
        menu.SetActive(false);
        _menuIsActive = false;
        Time.timeScale = 1;
    }

    internal void SetCounter(int count)
    {
        if (countText == null)
            return;

        countText.text = count > 0 ? count.ToString() : string.Empty;
    }

    internal void ScoreMe()
    {
        _scoreController.ScoreMe();
        myScoreText.text = _scoreController.GetMyScore();
    }

    internal void ScoreRival()
    {
        _scoreController.ScoreRival();
        rivalScoreText.text = _scoreController.GetRivalScore();
    }

    internal void ResetScores()
    {
        _scoreController.ResetScores();

        myScoreText.text = _scoreController.GetMyScore();
        rivalScoreText.text = _scoreController.GetRivalScore();
    }

    private void OnDestroy()
    {
        _scoreController.SaveBestScores();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            _scoreController.SaveBestScores();
    }

    public void OnMenuPressed()
    {
        if (!_menuIsActive)
            ShowMenu();
        else
            HideMenu();
    }
}
