using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{

    public GameObject panelMain, panelOptions;
    public List<GameManager.ScoreData> topScores = new List<GameManager.ScoreData>();
    public TMP_Text scores;
    public TMP_InputField spawnRate, maxEnemyCount;

    #region Options
    public void SaveOptionsAndReturn()
    {

        panelOptions.SetActive(false);
        panelMain.SetActive(true);
        if (string.IsNullOrEmpty(spawnRate.text) || string.IsNullOrEmpty(maxEnemyCount.text)) return;
        PlayerPrefs.SetInt("spawnRate", int.Parse(spawnRate.text));
        PlayerPrefs.SetInt("maxEnemyCount", int.Parse(maxEnemyCount.text));

    }
    #endregion
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadTopScores()
    {
        topScores.Clear();
        for (int i = 0; i < 10; i++)
        {
            float score = PlayerPrefs.GetFloat("Score " + i, 0);
            float time = PlayerPrefs.GetFloat("Time " + i, 0);
            if (score != 0)
            {
                topScores.Add(new GameManager.ScoreData(time, score));
            }
        }
        if (topScores.Count == 0) scores.text = "No Data!";
        else
        {
            scores.text = string.Join("\n", topScores.Select((data, index) =>
            $"{index} - time: {data.time}, score: {data.score}."));
        }
    }
}
