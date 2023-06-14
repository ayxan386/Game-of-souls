using System;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    [SerializeField] private Transform[] playerStandingPoints;
    [SerializeField] private PathTile[] nextTiles;
    [SerializeField] private MeshRenderer rend;

    [Header("When player arrives")] [SerializeField]
    private TileType tileType;

    [SerializeField] private int value;
    [SerializeField] private MiniGames miniGame;

    private int currentPoint = 0;
    private int currentHighlight = 0;
    private bool isSelectable;
    private bool waitingForChoice;
    public static event Action<PathTile> OnNextTileSelected;

    public bool HasChoices => nextTiles.Length > 1;

    public TileType Type => tileType;
    public int Value => value;
    public MiniGames MiniGame => miniGame;

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

    public void GetNextTile()
    {
        print("Asking for next tile");
        if (nextTiles.Length == 1)
        {
            print("Only 1 tile found");
            OnNextTileSelected?.Invoke(nextTiles[0]);
        }
        else
        {
            print("Many tile found");
            waitingForChoice = true;
            foreach (var tile in nextTiles)
            {
                tile.SetSelectable();
            }
        }
    }

    private void SetSelectable()
    {
        isSelectable = true;
        rend.material.color = Color.yellow;
    }

    private void SetHighlight()
    {
        rend.material.color = Color.green;
    }

    private void OnOtherTileSelected(PathTile obj)
    {
        isSelectable = false;
        rend.material.color = Color.black;
        currentHighlight = -1;
        waitingForChoice = false;
    }

    private void OnDrawGizmos()
    {
        if (isSelectable)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }


    private void OnPlayerChoiceChanged(int obj)
    {
        if (!waitingForChoice) return;
        if (currentHighlight >= 0) nextTiles[currentHighlight].SetSelectable();
        currentHighlight = (currentHighlight + obj + nextTiles.Length) % nextTiles.Length;
        nextTiles[currentHighlight].SetHighlight();
    }


    private void OnPlayerTileSelected(int obj)
    {
        if (!waitingForChoice) return;

        OnNextTileSelected?.Invoke(nextTiles[currentHighlight]);
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