using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float score;
    public float timer;
    private bool gameOver;
    private bool isPaused;

    public GameObject panelMain;
    public TMP_Text timeAndScore;

    public List<ScoreData> topScores = new List<ScoreData>();

    [System.Serializable]
    public class ScoreData
    {
        public float time;
        public float score;

        public ScoreData(float t, float s)
        {
            time = t;
            score = s;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        instance = this;
        LoadTopScores();
    }

    private void Update()
    {
        if (!gameOver && !isPaused)
        {
            timer += Time.deltaTime;
            int roundedTimer = Mathf.RoundToInt(timer);
            timeAndScore.text = $"Timer: {roundedTimer}s \n Score: {score}";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void AddPoint(int points)
    {
        score += points;
    }

    public void GameOver()
    {
        gameOver = true;
        panelMain.SetActive(true);
        Destroy(FindObjectOfType<BoatController>().gameObject);
        SaveScore();
        Pause();
    }

    public void Reset()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Pause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void SaveScore()
    {
        ScoreData currentScore = new ScoreData(timer, score);
        if (topScores.Count < 10)
        {
            topScores.Add(currentScore);
        }
        else
        {
            int worstScoreIndex = 0;
            for (int i = 0; i < topScores.Count; i++)
            {
                if (topScores[i].score < topScores[worstScoreIndex].score)
                {
                    worstScoreIndex = i;
                }
            }

            if (currentScore.score > topScores[worstScoreIndex].score)
            {
                topScores[worstScoreIndex] = currentScore;
            }
        }

        topScores.Sort((x, y) => y.score.CompareTo(x.score));
        SaveTopScores();
    }

    private void SaveTopScores()
    {
        for (int i = 0; i < topScores.Count; i++)
        {
            PlayerPrefs.SetFloat("Score " + i, topScores[i].score);
            PlayerPrefs.SetFloat("Time " + i, topScores[i].time);
        }
        PlayerPrefs.Save();
    }

    private void LoadTopScores()
    {
        for (int i = 0; i < 10; i++)
        {
            float score = PlayerPrefs.GetFloat("Score " + i, 0);
            float time = PlayerPrefs.GetFloat("Time " + i, 0);
            if (score != 0)
            {
                topScores.Add(new ScoreData(time, score));
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
