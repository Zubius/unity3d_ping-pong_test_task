using UnityEngine;

public class ScoreController
{
    private int _myScore, _rivalScore, _myBestScore, _rivalBestScore;

    private const string myScoreKey = "my_best_score";
    private const string rivalScoreKey = "rival_best_score";
    private const string bestScoreFormat = "{0} ({1})";

    internal ScoreController()
    {
        _myBestScore = PlayerPrefs.GetInt(myScoreKey, 0);
        _rivalBestScore = PlayerPrefs.GetInt(rivalScoreKey, 0);
    }

    internal void ResetScores()
    {
        _myScore = 0;
        _rivalScore = 0;
    }

    internal void SaveBestScores()
    {
        PlayerPrefs.SetInt(myScoreKey, Mathf.Max(_myBestScore, _myScore));
        PlayerPrefs.SetInt(rivalScoreKey, Mathf.Max(_rivalBestScore, _rivalScore));
        PlayerPrefs.Save();
    }

    internal void ScoreMe()
    {
        _myScore++;
        _myBestScore = Mathf.Max(_myScore, _myBestScore);
    }

    internal void ScoreRival()
    {
        _rivalScore++;
        _rivalBestScore = Mathf.Max(_rivalScore, _rivalBestScore);
    }

    internal string GetMyScore()
    {
        return GetScoreString(_myScore, _myBestScore);
    }

    internal string GetRivalScore()
    {
        return GetScoreString(_rivalScore, _rivalBestScore);
    }

    private string GetScoreString(int score, int bestScore)
    {
        return string.Format(bestScoreFormat, score.ToString(), bestScore.ToString());
    }
}
