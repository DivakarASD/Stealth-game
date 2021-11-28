using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    private void Start()
    {
        Guard.OnGuardSpottedPlayer += ShowGameLoseUI;
        Player.OnPlayerReachedEndPoint += ShowGameWinUI;
    }

    private void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameWinUI()
    {
        GameOver(gameWinUI);
    }
    void ShowGameLoseUI()
    {
        GameOver(gameLoseUI);
    }

    void GameOver(GameObject gameUI)
    {
        gameUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardSpottedPlayer -= ShowGameLoseUI;
        Player.OnPlayerReachedEndPoint -= ShowGameWinUI;
    }

}
