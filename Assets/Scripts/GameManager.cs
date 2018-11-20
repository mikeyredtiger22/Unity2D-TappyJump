﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = false;

    public bool GameOver { get { return gameOver; } }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CountdownText.OnCountDownFinished += OnCountDownFinished; // subscribe / listener
        TapController.OnPlayerDied += OnPlayerDied; // subscribe / listener
        TapController.OnPlayerScored += OnPlayerScored; // subscribe / listener
    }

    private void OnDisable()
    {
        CountdownText.OnCountDownFinished -= OnCountDownFinished; // remove listener / unsubscribe
        TapController.OnPlayerDied -= OnPlayerDied; // subscribe / listener
        TapController.OnPlayerScored -= OnPlayerScored; // subscribe / listener
    }

    void OnCountDownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted(); // event sent to Tap Controller
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HIGHSCORE");
        if (score > savedScore) {
            PlayerPrefs.SetInt("HIGHSCORE", score);
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void ConfirmGameOver() // RESTART
    {
        OnGameOverConfirmed(); // event sent to Tap Controller
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame() // START
    {
        SetPageState(PageState.Countdown);
    }

    void SetPageState(PageState pageState)
    {
        switch(pageState)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;

            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:

                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
         }
    }
}
