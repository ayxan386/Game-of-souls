using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChaseGreen_PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject playerPrefab;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        yield return new WaitForSeconds(1f);
        playerInputManager.playerPrefab = playerPrefab;
        foreach (var player in PlayerManager.Instance.Players)
        {
            var boardInput = player.GetComponent<PlayerInput>();
            var newPlayer = playerInputManager.JoinPlayer(boardInput.playerIndex, boardInput.splitScreenIndex,
                boardInput.currentControlScheme);

            newPlayer.transform.position = spawnPosition.position;
            newPlayer.name = player.DisplayName;
        }

        yield return new WaitForSeconds(2f);
        PlayersReady = true;
    }

    public static bool PlayersReady { get; set; }
}