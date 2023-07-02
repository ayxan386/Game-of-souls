using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClimbMinigame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Transform[] spawnPoints;
        [Header("On win")] [SerializeField] private GameObject awardScreen;
        [SerializeField] private List<PlayerAwardUI> playerAwardUis;
        [SerializeField] private int[] soulAwards;

        private List<PlayerRoundData> roundDatas;
        private int numberOfWins;

        public static GameManager Instance { get; private set; }

        private IEnumerator Start()
        {
            Instance = this;
            loadingScreen.SetActive(true);
            yield return new WaitForSeconds(5);
            loadingScreen.SetActive(false);
            InitiatePlayers();
        }

        private void InitiatePlayers()
        {
            var i = 0;
            roundDatas = new List<PlayerRoundData>(PlayerSubManager.PlayerRoots.Count);
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.SwitchToClimb();
                playerRoot.ClimbingPlayer.transform.position = spawnPoints[i++].position;
                var playerRoundData = new PlayerRoundData
                {
                    playerName = playerRoot.PlayerId,
                    score = 0,
                    place = -1,
                };
                roundDatas.Insert(playerRoot.PlayerIndex, playerRoundData);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var spawnPoint in spawnPoints)
            {
                Gizmos.DrawSphere(spawnPoint.position, 0.5f);
            }
        }

        private void AwardPlayers()
        {
            awardScreen.SetActive(true);

            foreach (var t in playerAwardUis)
            {
                t.gameObject.SetActive(false);
            }

            var places = new List<PlayerRoundData>();
            //Determine player places
            DeterminePlayerPlaces(places, roundDatas);
            places.Sort((a, b) => a.place - b.place);
            //Calculating soul awards
            CalculatePlayerScores(places);

            //Display awards and give souls to players
            for (var index = 0; index < places.Count; index++)
            {
                var data = places[index];
                var place = data.place;
                var soulAward = data.finalAward;
                playerAwardUis[index].gameObject.SetActive(true);
                playerAwardUis[index].UpdateUI(place, data.playerName, soulAward);
                PlayerSubManager.PlayerRoots.Find(player => player.PlayerId == data.playerName)
                    .BoardPlayer.UpdateSoulCount(soulAward);
            }
        }

        private void CalculatePlayerScores(List<PlayerRoundData> places)
        {
            for (var index = 0; index < places.Count; index++)
            {
                var self = places[index];
                var award = soulAwards[index];
                var count = 1;
                for (var i = 0; i < places.Count; i++)
                {
                    var other = places[i];
                    //Skipping same player
                    if (index == i) continue;
                    if (other.place != self.place) continue;

                    count++;
                    award += soulAwards[i];
                }

                self.finalAward = award / count;

                print($"Player {self.playerName} is {self.place} and awarded with {self.finalAward}");
            }
        }

        private void DeterminePlayerPlaces(List<PlayerRoundData> places, List<PlayerRoundData> roundData)
        {
            foreach (var self in roundData)
            {
                self.place = PlayerSubManager.PlayerRoots.Count;
                foreach (var other in roundData)
                {
                    if (self == other) continue;
                    if (self.score >= other.score)
                    {
                        self.place--;
                    }
                }

                places.Add(self);
            }
        }

        private void UnloadMiniGamePlayer()
        {
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.SwitchFromClimb();
            }
            
            MiniGameManager.Instance.MinigameFinished();
        }

        public void OnWin(PlayerSubManager playerSubManager)
        {
            roundDatas[playerSubManager.PlayerIndex].score = PlayerSubManager.PlayerRoots.Count - numberOfWins;
            numberOfWins++;
            if (numberOfWins >= PlayerSubManager.PlayerRoots.Count - 1)
            {
                AwardPlayers();
                Invoke(nameof(UnloadMiniGamePlayer), 5f);
            }
        }
    }
}