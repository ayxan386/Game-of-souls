using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player[] players;
    private int currentPlayer;

    private void Start()
    {
        foreach (var player in players)
        {
            PathManager.Instance.StartPlayerAtPosition(player.name, -1);
            player.MoveToTile(1);
            print("Player positioned");
        }

        DiceRotationManager.OnDiceRolled += OnDiceRolled;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;
    }

    private void OnPlayerPositionReached(string obj)
    {
        currentPlayer = (currentPlayer + 1) % players.Length;
    }

    private void OnDiceRolled(int diceRoll)
    {
        players[currentPlayer].MoveToTile(diceRoll);
    }
}