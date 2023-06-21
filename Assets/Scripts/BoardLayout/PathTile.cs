using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    [Header("Tile discovery")] [SerializeField]
    private float maxDistance;

    [SerializeField] private LayerMask pathTileLayer;

    [SerializeField] private Transform[] playerStandingPoints;
    [SerializeField] private List<PathTile> connectedTiles;
    [SerializeField] private MeshRenderer rend;

    [Header("When player arrives")] [SerializeField]
    private TileType tileType;

    [SerializeField] private int value;
    [SerializeField] private MiniGames miniGame;

    private int currentPoint;
    private int currentHighlight;
    private bool waitingForChoice;
    private Color initialColor;
    public static event Action<PathTile> OnNextTileSelected;

    public bool HasChoices => connectedTiles.Count > 1;

    public TileType Type => tileType;
    public int Value => value;
    public MiniGames MiniGame => miniGame;

    private void Start()
    {
        OnNextTileSelected += OnOtherTileSelected;
        Player.OnPlayerChoiceChanged += OnPlayerChoiceChanged;
        Player.OnPlayerTileSelected += OnPlayerTileSelected;
        initialColor = rend.material.color;
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


        if (player.PrevPosition)
            print("Player came from: " + player.PrevPosition.name);

        var numberOfOptions = connectedTiles.Count(tile => tile != player.PrevPosition);
        switch (numberOfOptions)
        {
            case 1:
                print("Only 1 tile found");
                OnNextTileSelected?.Invoke(connectedTiles.Find(tile => tile != player.PrevPosition));
                break;
            case 0:
                print("No tile found, returning back");
                OnNextTileSelected?.Invoke(player.PrevPosition);
                break;
            default:
            {
                print("Many tile found");
                waitingForChoice = true;
                foreach (var tile in connectedTiles.Where(tile => tile != player.PrevPosition))
                {
                    tile.SetSelectable();
                }

                break;
            }
        }
    }

    private void SetSelectable()
    {
        rend.material.color = Color.yellow;
    }


    private void SetHighlight()
    {
        rend.material.color = Color.green;
    }

    private void OnOtherTileSelected(PathTile obj)
    {
        rend.material.color = initialColor;
        currentHighlight = -1;
        waitingForChoice = false;
    }

    private void OnPlayerChoiceChanged(int obj)
    {
        if (!waitingForChoice) return;
        if (currentHighlight >= 0) connectedTiles[currentHighlight].SetSelectable();
        currentHighlight = (currentHighlight + obj + connectedTiles.Count) % connectedTiles.Count;
        connectedTiles[currentHighlight].SetHighlight();
    }

    private void OnPlayerTileSelected(int obj)
    {
        if (!waitingForChoice) return;

        OnNextTileSelected?.Invoke(connectedTiles[currentHighlight]);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
       
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    public void FindNearbyTiles()
    {
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
}


public enum TileType
{
    None,
    SoulAwarding,
    HealthDamaging,
    HealthHealing,
    MiniGameLoading
}