using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Dictionary<string, PathTile> playerPositions;

    public static PathManager Instance { get; private set; }

    private PathTile lastSelected;

    public bool IsSelected { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerPositions = new Dictionary<string, PathTile>();
    }

    private void Start()
    {
        PathTile.OnNextTileSelected += PathTileOnSelected;
    }

    private void OnDestroy()
    {
        PathTile.OnNextTileSelected += PathTileOnSelected;
    }

    private void PathTileOnSelected(PathTile obj)
    {
        lastSelected = obj;
        IsSelected = true;
    }

    public void SearchForNextTile(string playerId)
    {
        var playerPosition = playerPositions[playerId];
        playerPosition.GetNextTile();
    }

    public PathTile GetNextTileForPlayer()
    {
        IsSelected = false;
        return lastSelected;
    }

    public void PlayerReachedNextTile(string playerId)
    {
        playerPositions[playerId] = lastSelected;
    }

    public void StartPlayerAtPosition(string playerId, PathTile tile)
    {
        playerPositions[playerId] = tile;
    }
}