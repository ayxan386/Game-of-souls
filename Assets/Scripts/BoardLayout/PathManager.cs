using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    private PathTile lastSelected;

    public bool IsSelected { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PathTile.OnNextTileSelected += PathTileOnSelected;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;
    }


    private void OnDestroy()
    {
        PathTile.OnNextTileSelected -= PathTileOnSelected;
        Player.OnPlayerPositionReached -= OnPlayerPositionReached;
    }

    private void PathTileOnSelected(PathTile obj)
    {
        lastSelected = obj;
        IsSelected = true;
    }

    public void SearchForNextTile(Player player)
    {
        Instance.IsSelected = false;
        var playerPosition = player.Position;
        playerPosition.GetNextTile(player);
    }

    public PathTile GetNextTileForPlayer()
    {
        IsSelected = false;
        return lastSelected;
    }

    public void PlayerReachedNextTile(Player player)
    {
        player.PrevPosition = player.Position;
        player.Position = lastSelected;
    }

    private void OnPlayerPositionReached(Player player)
    {
        var playersCurrentTile = player.Position;
        switch (playersCurrentTile.Type)
        {
            case TileType.SoulAwarding:
                player.UpdateSoulCount(playersCurrentTile.Value);
                PlayerManager.Instance.EndPlayerTurn();
                break;
            case TileType.HealthDamaging:
            case TileType.HealthHealing:
                player.UpdateHealth(playersCurrentTile.Value);
                PlayerManager.Instance.EndPlayerTurn();
                break;
            case TileType.MiniGameLoading:
                MiniGameManager.Instance.LoadMiniGame(playersCurrentTile.MiniGame);
                break;
            default:
                PlayerManager.Instance.EndPlayerTurn();
                break;
        }
    }
}