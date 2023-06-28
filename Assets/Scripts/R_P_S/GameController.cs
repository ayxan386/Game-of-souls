using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace R_P_S
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerMiniGameUI[] playerUI;

        [Header("PreGame phase")] [SerializeField]
        private GameObject loadingScreen;

        [Header("Game phase")] [SerializeField]
        private int numberOfRounds;

        [SerializeField] private int roundDuration;
        [SerializeField] private GameObject decidePopUp;
        [SerializeField] private TextMeshProUGUI decisionTimer;
        [SerializeField] private AudioClip timerSound;
        [SerializeField] private GameObject nextRoundPopUp;
        [SerializeField] private DecisionComparision[] decisions;
        [SerializeField] private Sprite[] decisionSprites;
        [SerializeField] private TextMeshProUGUI roundText;

        [Header("Post game phase")] [SerializeField]
        private int[] soulAwards;

        [SerializeField] private float waitDuration;
        [SerializeField] private GameObject awardScreen;
        [SerializeField] private PlayerAwardUI[] playerAwardUis;

        [SerializeField] private AudioSource audioSource;

        private List<PlayerRoundData> roundData;

        private IEnumerator Start()
        {
            loadingScreen.SetActive(true);
            yield return new WaitForSeconds(2f);
            loadingScreen.SetActive(false);
            InitialPlayerSetup();
            StartCoroutine(GamePhase());
        }

        private void InitialPlayerSetup()
        {
            foreach (var gameUI in playerUI)
            {
                gameUI.gameObject.SetActive(false);
            }

            roundData = new List<PlayerRoundData>();
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.SwitchToRpsPlayer();
                var playerRoundData = new PlayerRoundData()
                {
                    playerName = playerRoot.PlayerId,
                    eliminationColor = playerRoot.ColorIndicator,
                    score = 0,
                    place = -1,
                    gameUi = playerUI[playerRoot.PlayerIndex]
                };
                playerUI[playerRoot.PlayerIndex].gameObject.SetActive(true);

                playerRoundData.UpdateSprite(null, 0);

                roundData.Add(playerRoundData);
            }
        }


        private IEnumerator GamePhase()
        {
            for (var i = 0; i < numberOfRounds; i++)
            {
                roundText.text = (i + 1).ToString();
                decidePopUp.SetActive(true);
                for (int j = 0; j < roundDuration; j++)
                {
                    audioSource.PlayOneShot(timerSound);
                    decisionTimer.text = (roundDuration - j).ToString();
                    yield return new WaitForSeconds(1);
                }

                decidePopUp.SetActive(false);
                RevealDecisions();
                nextRoundPopUp.SetActive(true);
                yield return new WaitForSeconds(2f);
                EvaluatePlayers();
                yield return new WaitForSeconds(2f);
                nextRoundPopUp.SetActive(false);
            }

            // Ending
            AwardPlayers();
            yield return new WaitForSeconds(waitDuration);
            UnloadMiniGamePlayer();
            MiniGameManager.Instance.MinigameFinished();
        }

        private void RevealDecisions()
        {
            foreach (var subManager in PlayerSubManager.PlayerRoots)
            {
                var playerController = subManager.RpsPlayerController;
                roundData[subManager.PlayerIndex].UpdateSprite(decisionSprites[playerController.LastDecision], 0);
            }
        }

        private void EvaluatePlayers()
        {
            foreach (var subManager in PlayerSubManager.PlayerRoots)
            {
                var playerController = subManager.RpsPlayerController;
                var startingScore = roundData[subManager.PlayerIndex].score;
                foreach (var otherManager in PlayerSubManager.PlayerRoots)
                {
                    if (decisions[playerController.LastDecision].stronger
                        .Contains(otherManager.RpsPlayerController.LastDecision))
                    {
                        roundData[subManager.PlayerIndex].score++;
                    }
                }

                var increment = roundData[subManager.PlayerIndex].score - startingScore;
                roundData[subManager.PlayerIndex].UpdateSprite(decisionSprites[playerController.LastDecision], increment);
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
            DeterminePlayerPlaces(places);
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

        private void DeterminePlayerPlaces(List<PlayerRoundData> places)
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
                playerRoot.SwitchFromRpsPlayer();
            }
        }
    }
}

[Serializable]
public class DecisionComparision
{
    public List<int> stronger;
}