using System;
using UnityEngine;

public class PathTile : MonoBehaviour
{
    [SerializeField] private Transform[] playerStandingPoints;
    [SerializeField] private PathTile[] nextTiles;
    [SerializeField] private MeshRenderer rend;

    private int currentPoint = 0;
    private int currentHighlight = 0;
    private bool isSelectable;
    private bool waitingForChoice;
    public static event Action<PathTile> OnNextTileSelected;

    private void Start()
    {
        OnNextTileSelected += OnOtherTileSelected;
    }

    private void OnDestroy()
    {
        OnNextTileSelected -= OnOtherTileSelected;
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

    private void Update()
    {
        if (waitingForChoice)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (currentHighlight >= 0) nextTiles[currentHighlight].SetSelectable();
                currentHighlight = (currentHighlight + 1) % nextTiles.Length;
                nextTiles[currentHighlight].SetHighlight();
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                OnNextTileSelected?.Invoke(nextTiles[currentHighlight]);
            }
        }
    }
}