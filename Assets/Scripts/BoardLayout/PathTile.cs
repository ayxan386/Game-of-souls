using UnityEngine;

public class PathTile : MonoBehaviour
{
    [SerializeField] private Transform[] playerStandingPoints;
    private int currentPoint = 0;

    public Transform GetNextPoint => playerStandingPoints[(currentPoint + 1) % playerStandingPoints.Length];
}