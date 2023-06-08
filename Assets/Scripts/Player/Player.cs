using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private float playerMovementSpeed;
    
    private CharacterController cc;

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
            var pathTile = PathManager.Instance.GetNextTileForPlayer(name);
            var point = pathTile.GetNextPoint;
            print($"Moving player {name} to {point.position}");
            while (Vector3.Distance(transform.position, point.position) >= minDistance)
            {
                cc.Move(-(transform.position - point.position) * (playerMovementSpeed * Time.deltaTime));
                yield return null;
            }

            PathManager.Instance.PlayerReachedNextTile(name);
            print("Tile reached");
        }
        
        OnPlayerPositionReached?.Invoke(name);
        
    }
}