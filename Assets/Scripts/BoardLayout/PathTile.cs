using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    [SerializeField] private Transform[] playerStandingPoints;
    [SerializeField] private List<PathTile> nextTiles;
    [SerializeField] private List<PathTile> prevTiles;
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private bool startingTile;

    [Header("When player arrives")] [SerializeField]
    private TileType tileType;

    [SerializeField] private int value;
    [SerializeField] private MiniGames miniGame;

    private int currentPoint;
    private int currentHighlight;
    private bool waitingForChoice;
    private Color initialColor;
    private List<PathTile> lastPathDirection;
    public static event Action<PathTile> OnNextTileSelected;

    public bool HasChoices => nextTiles.Count > 1 || prevTiles.Count > 1;

    public TileType Type => tileType;
    public int Value => value;
    public MiniGames MiniGame => miniGame;

    private void Start()
    {
        OnNextTileSelected += OnOtherTileSelected;
        Player.OnPlayerChoiceChanged += OnPlayerChoiceChanged;
        Player.OnPlayerTileSelected += OnPlayerTileSelected;
        initialColor = rend.material.color;

        if (startingTile) return;

        foreach (var nextTile in nextTiles)
        {
            nextTile.AddAsPrevTile(this);
        }
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

        if (lastPathDirection == null)
        {
            lastPathDirection = new List<PathTile>();
            lastPathDirection.AddRange(nextTiles);
            lastPathDirection.AddRange(prevTiles);
            lastPathDirection = lastPathDirection.DistinctBy(tile => tile).ToList();
        }

        if (player.PrevPosition)
            print("Player came from: " + player.PrevPosition.name);

        var numberOfOptions = lastPathDirection.Count(tile => tile != player.PrevPosition);
        switch (numberOfOptions)
        {
            case 1:
                print("Only 1 tile found");
                OnNextTileSelected?.Invoke(lastPathDirection.Find(tile => tile != player.PrevPosition));
                break;
            case 0:
                print("No tile found, returning back");
                OnNextTileSelected?.Invoke(player.PrevPosition);
                break;
            default:
            {
                print("Many tile found");
                waitingForChoice = true;
                foreach (var tile in lastPathDirection.Where(tile => tile != player.PrevPosition))
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
        if (currentHighlight >= 0) lastPathDirection[currentHighlight].SetSelectable();
        currentHighlight = (currentHighlight + obj + lastPathDirection.Count) % lastPathDirection.Count;
        lastPathDirection[currentHighlight].SetHighlight();
    }


    private void OnPlayerTileSelected(int obj)
    {
        if (!waitingForChoice) return;

        OnNextTileSelected?.Invoke(lastPathDirection[currentHighlight]);
    }

    private void AddAsPrevTile(PathTile pathTile)
    {
        prevTiles.Add(pathTile);
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