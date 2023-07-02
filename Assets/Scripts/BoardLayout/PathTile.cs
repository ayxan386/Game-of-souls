using System;
using System.Collections.Generic;
using System.Linq;
using BoardLayout;
using UnityEngine;

[Serializable]
public class PathTile : MonoBehaviour
{
    [Header("Tile discovery")] [SerializeField]
    private float maxDistance;

    [SerializeField] private LayerMask pathTileLayer;
    [SerializeField] private List<PathTile> connectedTiles;

    [Header("When player arrives")] [SerializeField]
    private Transform[] playerStandingPoints;

    [SerializeField] private MeshRenderer rend;
    [SerializeField] private TileType tileType;
    [SerializeField] private int value;

    [Header("Direction indicator")] [SerializeField]
    private DirectionIndicator arrowPrefab;

    [SerializeField] private float spawnOffset;

    private int currentPoint;
    private int currentHighlight;
    private bool waitingForChoice;
    private PathTile prevTile;
    private List<DirectionIndicator> arrows;
    public static event Action<PathTile> OnNextTileSelected;

    public TileType Type => tileType;
    public int Value => value;

    public List<PathTile> ConnectedTiles
    {
        get => connectedTiles;
        set => connectedTiles = value.ToHashSet().ToList();
    }

    private void Start()
    {
        OnNextTileSelected += OnOtherTileSelected;
        Player.OnPlayerChoiceChanged += OnPlayerChoiceChanged;
        Player.OnPlayerTileSelected += OnPlayerTileSelected;
    }

    private void OnDestroy()
    {
        OnNextTileSelected -= OnOtherTileSelected;
        Player.OnPlayerChoiceChanged -= OnPlayerChoiceChanged;
        Player.OnPlayerTileSelected -= OnPlayerTileSelected;
    }


    public Transform GetNextPoint()
    {
        var res = playerStandingPoints[currentPoint];
        currentPoint = (currentPoint + 1) % playerStandingPoints.Length;
        return res;
    }

    public void GetNextTile(Player player)
    {
        print("Asking for next tile");
        prevTile = player.PrevPosition;
        var numberOfOptions = connectedTiles.Count(tile => !ReferenceEquals(tile, player.PrevPosition));

        switch (numberOfOptions)
        {
            case 1:
                print("Only 1 tile found");
                OnNextTileSelected?.Invoke(connectedTiles.Find(tile => !ReferenceEquals(tile, player.PrevPosition)));
                break;
            case 0:
                print("No tile found, returning back");
                OnNextTileSelected?.Invoke(player.PrevPosition);
                break;
            default:
            {
                print("Many tile found");
                waitingForChoice = true;
                arrows = new List<DirectionIndicator>();
                foreach (var tile in connectedTiles.Where(tile => tile != player.PrevPosition))
                {
                    var dir = (tile.transform.position - transform.position).normalized;
                    var pos = player.ArrowBasePoint.position + dir * spawnOffset;
                    var arrow = Instantiate(arrowPrefab, pos, Quaternion.LookRotation(dir));
                    arrow.RelatedTile = tile;
                    arrow.UnSelect();
                    arrows.Add(arrow);
                }

                break;
            }
        }
    }

    public bool HasChoices(Player player)
    {
        var numberOfOptions = connectedTiles.Distinct().Count(tile => !ReferenceEquals(tile, player.Position));
        return numberOfOptions > 1;
    }

    private void OnOtherTileSelected(PathTile obj)
    {
        currentHighlight = -1;
        waitingForChoice = false;
    }

    private void OnPlayerChoiceChanged(int dir)
    {
        if (!waitingForChoice) return;
        print($"Changing choice from {currentHighlight}");
        var arrowsCount = arrows.Count;
        print("Number of arrows: " + arrowsCount);
        if (currentHighlight >= 0 && currentHighlight < arrowsCount) arrows[currentHighlight].UnSelect();
        while (true)
        {
            currentHighlight = (currentHighlight + dir + arrowsCount) % arrowsCount;
            arrows[currentHighlight].Select();
            print("in selection loop");
            if (prevTile == null || !ReferenceEquals(arrows[currentHighlight].RelatedTile, prevTile)) break;
        }
    }

    private void OnPlayerTileSelected(int obj)
    {
        if (!waitingForChoice) return;
        foreach (var arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }

        OnNextTileSelected?.Invoke(arrows[currentHighlight].RelatedTile);
    }

    private void OnDrawGizmosSelected()
    {
        if (connectedTiles == null) return;

        Gizmos.color = Color.green;
        foreach (var connectedTile in connectedTiles)
        {
            Gizmos.DrawSphere(connectedTile.transform.position, 0.5f);
        }
    }

    public void FindNearbyTiles()
    {
        connectedTiles = new List<PathTile>();
        if (!name.Contains("type"))
        {
            name += " type: " + rend.sharedMaterial.name;
        }

        var nearbyTiles = Physics.OverlapSphere(transform.position, maxDistance, pathTileLayer);
        foreach (var nearbyTile in nearbyTiles)
        {
            if (!nearbyTile.TryGetComponent(out PathTile otherTile)) continue;

            var dir = (nearbyTile.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, dir, maxDistance, pathTileLayer))
            {
                connectedTiles.Add(otherTile);
            }
        }
    }

    public override int GetHashCode()
    {
        return transform.position.GetHashCode();
    }
}


public enum TileType
{
    None,
    SoulAwarding,
    HealthDamaging,
    HealthHealing,
    MiniGameLoading,
    Teleporting,
    Shop
}