using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PodiumPlayerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> podiumFinal;
    [SerializeField] private GameObject WinningMenu;
    [SerializeField] private CinemachineVirtualCamera podiumCamera;
    [SerializeField] private Transform lookingPoint;

    public Player temp;

    public void WinCondition()
    {
        Invoke(nameof(DelayedPodium), 0.5f);
    }

    [ContextMenu("Make player look")]
    public void MakePlayerLook()
    {
        var dir = lookingPoint.position - temp.transform.position;
        dir.Normalize();
        print("Look rotation: " + dir);
        temp.TeleportToPosition(podiumFinal[0].transform.position, Quaternion.LookRotation(dir));
        podiumCamera.Priority = 25;
    }

    private void DelayedPodium()
    {
        podiumCamera.enabled = true;
        var maxSouls = PlayerSubManager.PlayerRoots[0];
        var winningPlayers = new List<Player>();
        foreach (var playerSubManager in PlayerSubManager.PlayerRoots)
        {
            if (playerSubManager.BoardPlayer.SoulCount > maxSouls.BoardPlayer.SoulCount)
            {
                maxSouls = playerSubManager;
            }
        }

        foreach (var playerSubManager in PlayerSubManager.PlayerRoots)
        {
            var boardPlayer = playerSubManager.BoardPlayer;
            if (maxSouls.BoardPlayer.SoulCount == boardPlayer.SoulCount)
            {
                boardPlayer.PlayerView.Priority = 5;
                winningPlayers.Add(boardPlayer);
            }
        }

        podiumCamera.Priority = 25;
        for (int i = 0; i < winningPlayers.Count; i++)
        {
            var dir = lookingPoint.position - winningPlayers[i].transform.position;
            dir.Normalize();
            print("Look rotation: " + dir);
            StartCoroutine(TryManyTimes(winningPlayers[i], podiumFinal[i].transform.position,
                Quaternion.LookRotation(dir), dir));
        }

        WinningMenu.SetActive(true);
    }

    private IEnumerator TryManyTimes(Player player, Vector3 position, Quaternion rotation, Vector3 dir)
    {
        var characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;
        for (int i = 0; i < 2; i++)
        {
            player.transform.forward = dir;
            player.transform.position = position;
            player.transform.rotation = rotation;
            yield return new WaitForSeconds(0.5f);
        }

        characterController.enabled = true;
    }
}