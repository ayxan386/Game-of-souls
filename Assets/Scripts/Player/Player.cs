using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private float playerMovementSpeed;
    [SerializeField] private Transform tileCheckPoint;
    [SerializeField] private CinemachineVirtualCamera vCamera;

    private CharacterController cc;
    private Transform targetPoint;

    public CinemachineVirtualCamera PlayerView => vCamera;

    public static event Action<string> OnPlayerPositionReached;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void MoveToTile(int roll)
    {
        StartCoroutine(MoveCurrentPlayerBy(roll));
    }

    private IEnumerator MoveCurrentPlayerBy(int diceRoll)
    {
        for (int i = 0; i < diceRoll; i++)
        {
            PathManager.Instance.SearchForNextTile(name);
            yield return new WaitUntil(() => PathManager.Instance.IsSelected);

            var pathTile = PathManager.Instance.GetNextTileForPlayer();
            targetPoint = pathTile.GetNextPoint();
            print($"Moving player {name} to {targetPoint.position}");

            while (!CheckIfReached())
            {
                var dir = -(transform.position - targetPoint.position);
                dir.Normalize();
                cc.Move(dir * (playerMovementSpeed * Time.deltaTime));
                dir.y = 0;
                transform.forward = dir;
                yield return null;
            }

            PathManager.Instance.PlayerReachedNextTile(name);
            print("Tile reached");
        }

        OnPlayerPositionReached?.Invoke(name);
    }

    private bool CheckIfReached()
    {
        return Vector3.Distance(tileCheckPoint.position, targetPoint.position) <= minDistance;
    }


    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPoint.position, 0.3f);
        }
    }
}