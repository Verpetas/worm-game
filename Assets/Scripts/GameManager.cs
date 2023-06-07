using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<WormController> players;
    public List<TMP_Text> playerScores;
    public GameObject gameOverScreen;

    int deaths = 0;
    bool started = false;

    public int maxScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetupGame()
    {
        if (!started) return;

        foreach (WormController controller in players)
        {
            controller.DeathEvent += HandlePlayerDeath;
            controller.ScoreChangeEvent += UpdateScore;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    void HandlePlayerDeath(WormController controller)
    {
        deaths += 1;

        if (deaths == players.Count)
        {
            gameOverScreen.SetActive(true);
        }
    }

    void UpdateScore(WormController controller, int playerNumber)
    {
        maxScore = Mathf.Max(maxScore, controller.score);
        playerScores[playerNumber].text = controller.score.ToString();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        started = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
