using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    [SerializeField] private PathTile startingTile;
    [SerializeField] private LevelPath path;
    [SerializeField] private List<PathTile> levelTiles;

    [Header("Teleport FX")] [SerializeField]
    private AudioClip teleportationSound;

    [SerializeField] private GameObject teleportParticles;

    private Dictionary<string, List<string>> buffer;
    public static PathManager Instance { get; private set; }

    private PathTile lastSelected;
    private Queue<PathTile> visualizeOrder;

    public bool IsSelected { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PathTile.OnNextTileSelected += PathTileOnSelected;
        Player.OnPlayerPositionReached += OnPlayerPositionReached;

        LoadPath();
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
                var values = Enum.GetValues(typeof(MiniGames)).Cast<MiniGames>().ToList();
                MiniGameManager.Instance.LoadMiniGame(values[Random.Range(0, values.Count)]);
                break;
            case TileType.Teleporting:
                StartCoroutine(TeleportationSequence(player));
                break;
            default:
                PlayerManager.Instance.EndPlayerTurn();
                break;
        }
    }

    private IEnumerator TeleportationSequence(Player player)
    {
        Instantiate(teleportParticles, player.FootPoint.position, Quaternion.identity, player.transform);
        yield return new WaitForSeconds(1.2f);
        var randomTile = levelTiles[Random.Range(0, levelTiles.Count)];
        player.TeleportToTile(randomTile);
        PlayerManager.Instance.SfxAudioSource.PlayOneShot(teleportationSound);
        yield return new WaitForSeconds(1);
    }

    [ContextMenu("Create path")]
    public void CreatePath()
    {
        var pathTiles = FindObjectsOfType<PathTile>();
        foreach (var pathTile in pathTiles)
        {
            pathTile.FindNearbyTiles();
        }
    }

    [ContextMenu("Load path")]
    public void LoadPath()
    {
        var pathTiles = FindObjectsOfType<PathTile>();
        var dic = new SerializedDictionary<string, PathTile>();
        foreach (var pathTile in pathTiles)
        {
            dic[ToKey(pathTile)] = pathTile;
        }

        foreach (var pathTile in pathTiles)
        {
            var pathData = path.Path.Find(data => data.key == ToKey(pathTile));
            if (pathData != null)
            {
                var connectedTiles = pathData.connectedTileKeys.Distinct().Select(hashes => dic[hashes]).ToList();
                pathTile.ConnectedTiles = connectedTiles;
            }
            else
            {
                print("Missing path for " + pathTile.name);
            }
        }

        levelTiles = pathTiles.ToList();
    }

    [ContextMenu("Save path")]
    public void SavePath()
    {
        buffer = new SerializedDictionary<string, List<string>>();
        var pathTiles = FindObjectsOfType<PathTile>();
        foreach (var pathTile in pathTiles)
        {
            if (pathTile.ConnectedTiles.Count > 2)
            {
                print($"Path tile {pathTile.transform.parent.name} has {pathTile.ConnectedTiles.Count} connections");
            }

            buffer[ToKey(pathTile)] = pathTile.ConnectedTiles.ConvertAll(ToKey).Distinct().ToList();
        }

        path.Path = new List<PathData>(buffer.Count);

        foreach (var bufferPair in buffer)
        {
            path.Path.Add(new PathData
            {
                key = bufferPair.Key,
                connectedTileKeys = bufferPair.Value
            });
        }
    }

    [ContextMenu("Visualize path")]
    public void VisualizePath()
    {
        StartCoroutine(SlowlyVisualizePath());
    }

    private IEnumerator SlowlyVisualizePath()
    {
        visualizeOrder = new Queue<PathTile>();
        var visitedTiles = new HashSet<string>();
        visitedTiles.Add(ToKey(startingTile));
        visualizeOrder.Enqueue(startingTile);
        PathTile top;
        while (visualizeOrder.TryDequeue(out top))
        {
            foreach (var connectedTile in top.ConnectedTiles.Where(connectedTile =>
                         !visitedTiles.Contains(ToKey(connectedTile))))
            {
                print("Visited tile at: " + connectedTile.transform.position);
                visualizeOrder.Enqueue(connectedTile);
                visitedTiles.Add(ToKey(connectedTile));
            }

            yield return new WaitForSeconds(1.0f);
        }

        print("Number of visited tiles: " + visitedTiles.Count);
    }

    private void OnDrawGizmosSelected()
    {
        if (visualizeOrder == null) return;

        Gizmos.color = Color.yellow;
        foreach (var pathTile in visualizeOrder)
        {
            Gizmos.DrawSphere(pathTile.transform.position, 0.5f);
        }
    }

    private static string ToKey(PathTile pathTile)
    {
        return pathTile.transform.position.GetHashCode().ToString();
    }
}