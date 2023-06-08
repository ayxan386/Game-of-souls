using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CharacterController[] players;
    [SerializeField] private float playerMovementSpeed;
    [SerializeField] private float minDistance;

    private int currentPlayer;

    private IEnumerator Start()
    {
        foreach (var player in players)
        {
            var pathTile = PathManager.Instance.SetPlayerToPosition(player.name, 0);
            var point = pathTile.GetNextPoint;
            while (Vector3.Distance(player.transform.position, point.position) >= minDistance)
            {
                player.Move(-(player.transform.position - point.position) * playerMovementSpeed * Time.deltaTime);
                yield return null;
            }

            print("Player positioned");
        }

        DiceRotationManager.OnDiceRolled += OnDiceRolled;
    }

    private void OnDiceRolled(int diceRoll)
    {
        StartCoroutine(MoveCurrentPlayerBy(diceRoll));
    }

    private IEnumerator MoveCurrentPlayerBy(int diceRoll)
    {
        var player = players[currentPlayer];

        for (int i = 0; i < diceRoll; i++)
        {
            var pathTile = PathManager.Instance.GetNextTileForPlayer(player.name);
            var point = pathTile.GetNextPoint;
            print($"Moving player {player.name} to {point.position}");
            while (Vector3.Distance(player.transform.position, point.position) >= minDistance)
            {
                player.Move(-(player.transform.position - point.position) * (playerMovementSpeed * Time.deltaTime));
                yield return null;
            }

            PathManager.Instance.PlayerReachedNextTile(player.name);

            print("Tile reached");
        }

        currentPlayer = (currentPlayer + 1) % players.Length;
    }
}