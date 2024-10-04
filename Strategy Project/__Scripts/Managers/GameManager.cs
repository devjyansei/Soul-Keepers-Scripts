using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] Camera cam;
    Core core;
    [SerializeField] private bool isGameStarted;
    private bool isGameEnded;
    private bool isPaused;
    public bool GetIsPaused()
    {
        return isPaused;
    }
    public bool GetIsGameStarted()
    {
        return isGameStarted;
    }
    public InGameStates currentGameState;
    public enum InGameStates
    {
        flow,
        paused,
    }
    public void SwitchGameState(InGameStates state)
    {
        currentGameState = state;
        switch (currentGameState)
        {
            case InGameStates.flow:
                ResumeGame();
                break;

            case InGameStates.paused:
                PauseGame();
                break;

            default:
                break;
        }
    }

    private void Awake()
    {
        if (FindObjectOfType<Core>() != null)
        {
            core = FindObjectOfType<Core>();
        }
        isGameEnded = false;
    }
    public void StartNewGame()
    {
        isGameStarted = true;
        SceneManager.LoadScene(1);

    }
    public void ExitGameApplication()
    {
        Application.Quit();
        Debug.Log("quit");
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    public bool IsGameEnded()
    {
        return isGameEnded;
    }
    public void GameOver()
    {
        isGameEnded = true;
        CameraController.Instance.DisablePan();
        UiManager.Instance.CloseInfoCanvas();
        UiManager.Instance.CloseGeneralCanvas();
        UiManager.Instance.OpenMinimapCanvas();
        UiManager.Instance.CloseSoulPanel();
        UiManager.Instance.CloseSoulShop();

        WaveManager.Instance.enabled = false;
        PlayerInputManager.Instance.enabled = false;
        CameraController.Instance.TranslateCamera(Camera.main.transform.position, new Vector3(0, Camera.main.transform.position.y, -15f), .5f);
        core.PlayDestroySequence();      
    }
    public void Victory()
    {
        isGameEnded = true;
        CameraController.Instance.DisablePan();
        UiManager.Instance.CloseInfoCanvas();
        UiManager.Instance.CloseGeneralCanvas();
        UiManager.Instance.OpenMinimapCanvas();
        UiManager.Instance.CloseSoulPanel();
        UiManager.Instance.CloseSoulShop();

        WaveManager.Instance.enabled = false;
        PlayerInputManager.Instance.enabled = false;
        CameraController.Instance.TranslateCamera(Camera.main.transform.position, new Vector3(0, Camera.main.transform.position.y, -15f), .5f);

        core.PlayVictorySequence();

    }
    public void GoToMainMenu()
    {
        isGameStarted = false;
        SceneManager.LoadScene(0);
    }
    public void SetIsGameStarted(bool state)
    {
        isGameStarted = state;
    }
}
