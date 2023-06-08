using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private PathTile[] path;
    private Dictionary<string, int> playerPositions;

    public static PathManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerPositions = new Dictionary<string, int>();
    }

    public PathTile GetNextTileForPlayer(string playerId)
    {
        return path[playerPositions[playerId] + 1];
    }

    public void PlayerReachedNextTile(string playerId)
    {
        playerPositions[playerId]++;
    }

    public void StartPlayerAtPosition(string playerId, int pos)
    {
        playerPositions[playerId] = pos;
    }
}