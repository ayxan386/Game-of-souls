using System.Collections;
using UnityEngine;

namespace ChaseGreen
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;

        public static bool PlayersReady { get; set; }

        IEnumerator Start()
        {
            PlayersReady = false;
            yield return new WaitForSeconds(1f);
            foreach (var player in PlayerSubManager.PlayerRoots)
            {
                player.LoadedChaseTheGreen();
                player.ChaseGreenPlayer.TeleportToPosition(spawnPosition.position);
            }

            yield return new WaitForSeconds(2f);
            PlayersReady = true;
        }
    }
}