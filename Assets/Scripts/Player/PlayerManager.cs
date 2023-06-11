using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player[] players;
    [SerializeField] private PathTile startingTile;
    private int currentPlayer;

    private void Start()
    {
        foreach (var player in players)
        {
            PathManager.Instance.StartPlayerAtPosition(player.name, startingTile);
            print("Player positioned");
        }

        DiceRotationManager.OnDiceRolled += OnDiceRolled;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;
        players[currentPlayer].PlayerView.Priority = 15;
    }

    private void OnPlayerPositionReached(string obj)
    {
        players[currentPlayer].PlayerView.Priority = 5;
        currentPlayer = (currentPlayer + 1) % players.Length;
        players[currentPlayer].PlayerView.Priority = 15;
    }

    private void OnDiceRolled(int diceRoll)
    {
        players[currentPlayer].MoveToTile(diceRoll);
    }
}