using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private float roundDuration;
        [SerializeField] private GameObject decidePopUp;
        [SerializeField] private GameObject nextRoundPopUp;
        [SerializeField] private DecisionComparision[] decisions;
        [SerializeField] private Sprite[] decisionSprites;

        private List<PlayerRoundData> roundData;

        private IEnumerator Start()
        {
            loadingScreen.SetActive(true);
            yield return new WaitForSeconds(5f);
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

                playerRoundData.UpdateSprite(null);

                roundData.Add(playerRoundData);
            }
        }


        private IEnumerator GamePhase()
        {
            for (var i = 0; i < numberOfRounds; i++)
            {
                decidePopUp.SetActive(true);
                yield return new WaitForSeconds(roundDuration);
                decidePopUp.SetActive(false);
                EvaluatePlayers();
                yield return new WaitForSeconds(2f);
                nextRoundPopUp.SetActive(true);
                yield return new WaitForSeconds(2f);
                nextRoundPopUp.SetActive(false);
            }
        }

        private void EvaluatePlayers()
        {
            foreach (var subManager in PlayerSubManager.PlayerRoots)
            {
                var playerController = subManager.RpsPlayerController;
                roundData[subManager.PlayerIndex].UpdateSprite(decisionSprites[playerController.LastDecision]);
                foreach (var otherManager in PlayerSubManager.PlayerRoots)
                {
                    if (decisions[playerController.LastDecision].stronger
                        .Contains(otherManager.RpsPlayerController.LastDecision))
                    {
                        roundData[subManager.PlayerIndex].score++;
                        roundData[subManager.PlayerIndex].UpdateSprite(decisionSprites[playerController.LastDecision]);
                    }
                }
            }
        }
    }
}

[Serializable]
public class DecisionComparision
{
    public List<int> stronger;
}