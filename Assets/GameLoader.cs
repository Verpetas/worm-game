using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField] List<WormController> players;
    [SerializeField] List<TMP_Text> playerScores;
    [SerializeField] GameObject gameOverScreen;

    private void Start()
    {
        GameManager.Instance.players = players;
        GameManager.Instance.playerScores = playerScores;
        GameManager.Instance.gameOverScreen = gameOverScreen;
        GameManager.Instance.SetupGame();
    }

    public void RestartGame()
    {
        GameManager.Instance.StartGame();
        GameManager.Instance.SetupGame();
    }
}
