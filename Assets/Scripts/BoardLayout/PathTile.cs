using UnityEngine;

public class PathTile : MonoBehaviour
{
    [SerializeField] private Transform[] playerStandingPoints;
    private int currentPoint = 0;

    public Transform GetNextPoint()
    {
        var res = playerStandingPoints[currentPoint];
        currentPoint = (currentPoint + 1) % playerStandingPoints.Length;
        return res;
    }
}