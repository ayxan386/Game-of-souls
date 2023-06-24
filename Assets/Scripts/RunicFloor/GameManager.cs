using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunicFloor
{
    public class GameManager : MonoBehaviour
    {
        [Header("Loading")] [SerializeField] private GameObject loadingScreen;

        [Header("Grid generation")] [SerializeField]
        private GameObject gridBlock;

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 blockSize;
        [SerializeField] private Vector2 blockSpacing;
        [SerializeField] private Transform[] floorOrigins;

        [Header("Player")] [SerializeField] private PlayerMiniGameUI playerMiniGameUIPrefab;

        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform playerUiHolder;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private Color eliminationColor;

        [Header("Post game phase")] [SerializeField]
        private int[] soulAwards;

        [SerializeField] private float waitDuration;
        [SerializeField] private GameObject awardScreen;
        [SerializeField] private PlayerAwardUI[] playerAwardUis;

        private Dictionary<string, PlayerRoundData> roundData;
        public static GameManager Instance;
        private int playerRoundScore;
        public AudioSource AudioSource => sfxAudioSource;

        public bool GameRunning { get; private set; }

        private IEnumerator Start()
        {
            Instance = this;
            GameRunning = false;
            loadingScreen.SetActive(true);
            GenerateGrid();
            yield return new WaitUntil(() => PlayerSubManager.PlayerRoots != null);
            InitialPlayerSetup();
            yield return new WaitForSeconds(0.5f);
            loadingScreen.SetActive(false);
            GameRunning = true;
        }

        private void UnloadMiniGamePlayer()
        {
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.SwitchFrom3rdPerson();
            }
        }

        private void InitialPlayerSetup()
        {
            roundData = new Dictionary<string, PlayerRoundData>();
            playerRoundScore = PlayerSubManager.PlayerRoots.Count;
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.SwitchTo3rdPerson();
                playerRoot.ThirdPersonController.TeleportToPosition(spawnPoint.position);
                var playerMiniGameUI = Instantiate(playerMiniGameUIPrefab, playerUiHolder);
                playerMiniGameUI.UpdateUI(playerRoot.PlayerId, 0, Color.clear);

                var playerRoundData = new PlayerRoundData
                {
                    playerName = playerRoot.PlayerId,
                    score = 0,
                    place = -1,
                    gameUi = playerMiniGameUI
                };
                roundData[playerRoot.PlayerId] = playerRoundData;
                playerRoot.ThirdPersonController.RoundData = playerRoundData;
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
                PlayerManager.Instance.AwardPlayerWithSouls(data.playerName, soulAward);
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
            foreach (var self in roundData.Values)
            {
                self.place = PlayerSubManager.PlayerRoots.Count;
                foreach (var other in roundData.Values)
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

        private void GenerateGrid()
        {
            foreach (var floorOrigin in floorOrigins)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    for (var y = 0; y < gridSize.y; y++)
                    {
                        var position = floorOrigin.position
                                       + new Vector3(blockSize.x * (x - gridSize.x / 2), 0,
                                           blockSize.y * (y - gridSize.y / 2))
                                       + new Vector3(blockSpacing.x * x, 0, blockSpacing.y * y);
                        var gm = Instantiate(gridBlock, position, Quaternion.identity, floorOrigin);
                        gm.name = $"grid block ({x}, {y})";
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var floorOrigin in floorOrigins)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    for (var y = 0; y < gridSize.y; y++)
                    {
                        var position = floorOrigin.position
                                       + new Vector3(blockSize.x * (x - gridSize.x / 2), 0,
                                           blockSize.y * (y - gridSize.y / 2))
                                       + new Vector3(blockSpacing.x * x, 0, blockSpacing.y * y);
                        Gizmos.DrawSphere(position, 0.5f);
                    }
                }
            }
        }

        private IEnumerator GameFinished()
        {
            AwardPlayers();
            yield return new WaitForSeconds(waitDuration);
            UnloadMiniGamePlayer();
            MiniGameManager.Instance.MinigameFinished();
        }

        public void PlayerTouchedLava(ThirdPersonController player)
        {
            var playerRoundData = roundData[player.RoundData.playerName];
            playerRoundData.place = playerRoundScore--;
            playerRoundData.gameUi.UpdateUI(playerRoundData.playerName, playerRoundData.place,
                eliminationColor, "Eliminated");

            if (playerRoundScore <= 1)
            {
                GameRunning = false;
                StartCoroutine(GameFinished());
            }
        }
    }
}