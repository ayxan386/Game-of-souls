using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject boardScene;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private List<MiniGameLoadingData> miniGameLoadingData;

    public static MiniGameManager Instance { get; set; }

    private string CurrentMiniGameName { get; set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMiniGame(MiniGames miniGame)
    {
        var gameLoadingData = miniGameLoadingData.Find(data => data.miniGame == miniGame);
        inputManager.splitScreen = gameLoadingData.isSplitScreen;
        CurrentMiniGameName = gameLoadingData.sceneName;
        boardScene.SetActive(false);
        SceneManager.LoadScene(CurrentMiniGameName, LoadSceneMode.Additive);
    }

    public void MinigameFinished()
    {
        var asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(CurrentMiniGameName));
        StartCoroutine(WaitForUnLoad(asyncOperation));
    }

    private IEnumerator WaitForUnLoad(AsyncOperation asyncOperation)
    {
        yield return new WaitUntil(() => asyncOperation.isDone);
        boardScene.SetActive(true);
        PlayerManager.Instance.EndPlayerTurn();
        print("Unload finished: allowing player switch");
    }
}

public enum MiniGames
{
    ChaseGreen,
    RunicFloor
}

[Serializable]
public class MiniGameLoadingData
{
    public MiniGames miniGame;
    public string sceneName;
    public bool isSplitScreen;
}