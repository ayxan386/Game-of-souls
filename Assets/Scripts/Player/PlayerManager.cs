using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private PathTile startingTile;
    [SerializeField] private Transform playerUIParent;
    [SerializeField] private PlayerInputManager playerInputManager;
    [Header("Joining")] [SerializeField] private Transform joiningIndicator;
    [SerializeField] private PlayerJoinedIndicator joiningPrefab;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject connectionMenu;
    public bool isLastPlayer;

    public int turns;
    public int MaxTurns;

    public bool GameStarted { get; private set; }

    public static PlayerManager Instance { get; private set; }

    public Transform PlayerUIParent => playerUIParent;

    private int currentPlayer;

    private void Awake()
    {
        Instance = this;
        isLastPlayer = false;
        turns = 1;
        MaxTurns = 5;
    }

    private void Start()
    {
        DiceRotationManager.OnDiceRolled += OnDiceRolled;
    }

    private void OnDestroy()
    {
        DiceRotationManager.OnDiceRolled -= OnDiceRolled;
    }

    private IEnumerator WaitThenSetUp(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(0.2f);
        var playerSubManager = playerInput.GetComponent<PlayerSubManager>();
        var player = playerSubManager.BoardPlayer;
        player.DisplayName = playerSubManager.PlayerId;
        players.Add(player);
        Instantiate(joiningPrefab, joiningIndicator).Display(
            playerSubManager.PlayerId,
            playerSubManager.ColorIndicator);
        player.Position = startingTile;
        player.TeleportToPosition(startingTile.GetNextPoint().position);
        player.UpdateSoulCount(0);
    }

    public void EndAllTurnsController()
    {
        if (currentPlayer == players.Count-1)
        {
            Debug.Log("El current player es" + currentPlayer);
            isLastPlayer = true;
        }
    }

    public void SetFalseIsLastTurn()
    {
        isLastPlayer = false;
    }

    public bool GetIsLastTurn()
    {
        return isLastPlayer;
    }

    public void EndPlayerTurn()
    {
        Debug.Log("El comienzo de EndPlayerTurn es el personaje");
        players[currentPlayer].PlayerView.Priority = 5;
        players[currentPlayer].UpdateStateOfPlayer(false);
        print("Ended player turn");
        EndAllTurnsController();
        currentPlayer = (currentPlayer + 1) % players.Count;
        print("Current player " + currentPlayer);
        ActivatePlayer();
    }

    public void SetFirstPlayerTurn()
    {
        currentPlayer = (currentPlayer + 1) % players.Count;
        print("Current player " + currentPlayer);
        turns++;
        ActivatePlayer();
    }

    private void ActivatePlayer()
    {
        players[currentPlayer].PlayerView.Priority = 15;
        players[currentPlayer].UpdateStateOfPlayer(true);
        DiceRotationManager.Instance.CanRoll = true;
    }

    private void OnDiceRolled(int diceRoll)
    {
        playerInputManager.DisableJoining();
        players[currentPlayer].MoveToTile(diceRoll);
        DiceRotationManager.Instance.CanRoll = false;
    }

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        startButton.interactable = true;

        StartCoroutine(WaitThenSetUp(newPlayer));
    }

    public void StartGame()
    {
        connectionMenu.SetActive(false);
        StartCoroutine(CustomizeCharacter());
    }

    private IEnumerator CustomizeCharacter()
    {
        foreach (var player in players)
        {
            player.UpdateStateOfPlayer(false);
            yield return new WaitUntil(() => player.IsCustomized);
        }

        GameStarted = true;
        ActivatePlayer();
    }

    public void AwardPlayerWithSouls(string playerName, int soulCount)
    {
        players.Find(player => player.DisplayName == playerName).UpdateSoulCount(soulCount);
    }

    public void NextSelectable()
    {
        foreach (var selectable in Selectable.allSelectablesArray)
        {
            if (!selectable.IsInteractable()) continue;

            selectable.Select();
            break;
        }
    }
}