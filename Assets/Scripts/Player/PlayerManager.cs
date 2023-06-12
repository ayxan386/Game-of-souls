using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player[] players;
    [SerializeField] private PathTile startingTile;
    [SerializeField] private Transform playerUIParent;
    private int currentPlayer;

    public static PlayerManager Instance { get; private set; }

    public Transform PlayerUIParent => playerUIParent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var player in players)
        {
            PathManager.Instance.StartPlayerAtPosition(player.name, startingTile);
            print("Player positioned");
        }

        DiceRotationManager.OnDiceRolled += OnDiceRolled;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;
        ActivatePlayer();
    }

    private void OnPlayerPositionReached(string obj)
    {
        players[currentPlayer].PlayerView.Priority = 5;
        players[currentPlayer].UpdateStateOfPlayer(false);
        currentPlayer = (currentPlayer + 1) % players.Length;
        ActivatePlayer();
    }

    private void ActivatePlayer()
    {
        players[currentPlayer].PlayerView.Priority = 15;
        players[currentPlayer].UpdateStateOfPlayer(true);
    }

    private void OnDiceRolled(int diceRoll)
    {
        players[currentPlayer].MoveToTile(diceRoll);
    }
}