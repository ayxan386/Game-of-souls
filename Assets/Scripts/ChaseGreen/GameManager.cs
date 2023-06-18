using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChaseGreen
{
    public class GameManager : MonoBehaviour
    {
        [Header("Loading")] [SerializeField] private GameObject loadingScreen;

        [Header("Grid generation")] [SerializeField]
        private GameObject gridBlock;

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 blockSize;
        [SerializeField] private Vector2 blockSpacing;

        [Header("Player UI display")] [SerializeField]
        private PlayerMiniGameUI playerMiniGameUIPrefab;

        [SerializeField] private Transform playerUiHolder;
        [SerializeField] private Color eliminationColor;

        [Header("Game phase")] [SerializeField]
        private Vector2Int safeBlockCountRange;

        [SerializeField] private Color safeBlockColor;
        [SerializeField] private Color otherBlocksColorStart;
        [SerializeField] private Color otherBlocksColorEnd;
        [SerializeField] private float duration;
        [SerializeField] [Range(0, 1f)] private float colorAnimationFactor;
        [SerializeField] private int survivalBonus;

        [Header("Post game phase")] [SerializeField]
        private int[] soulAwards;

        [SerializeField] private float waitDuration;
        [SerializeField] private GameObject awardScreen;
        [SerializeField] private PlayerAwardUI[] playerAwardUis;

        private MeshRenderer[,] gridMesh;
        private Vector2Int[] safeBlocks;
        private Dictionary<string, PlayerRoundData> roundData;

        public static GameManager Instance;

        private void Start()
        {
            Instance = this;
            PlayerManager.PlayersReady = false;
            loadingScreen.SetActive(true);
            GenerateGrid();
            InitialPlayerSetup();

            StartCoroutine(GamePhase());
        }


        private IEnumerator GamePhase()
        {
            yield return new WaitUntil(() => PlayerManager.PlayersReady);
            loadingScreen.SetActive(false);
            while (true)
            {
                RandomlySelectSafeBlocks();
                colorAnimationFactor = 0.1f;
                for (var t = 0f; t < duration; t += duration * colorAnimationFactor)
                {
                    ColorizeGrid(t / duration);
                    yield return new WaitForSeconds(duration * colorAnimationFactor);
                }

                CheckPlayerPositions();

                if (CheckForElimination())
                {
                    AwardPlayers();
                    yield return new WaitForSeconds(waitDuration);
                    UnloadMiniGamePlayer();
                    MiniGameManager.Instance.MinigameFinished();
                    yield break;
                }
            }
        }

        private bool CheckForElimination()
        {
            var eliminatedCount = roundData.Values.Count(data => data.isEliminated);
            return eliminatedCount >= roundData.Values.Count - 1;
        }

        private void UnloadMiniGamePlayer()
        {
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                playerRoot.UnLoadedChaseTheGreen();
            }
        }

        private void InitialPlayerSetup()
        {
            roundData = new Dictionary<string, PlayerRoundData>();
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
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
                playerRoot.ChaseGreenPlayer.RoundData = playerRoundData;
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
                global::PlayerManager.Instance.AwardPlayerWithSouls(data.playerName, soulAward);
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


        private void RandomlySelectSafeBlocks()
        {
            var safeBlockCount = Random.Range(safeBlockCountRange.x, safeBlockCountRange.y);
            safeBlocks = new Vector2Int[safeBlockCount];
            for (var i = 0; i < safeBlockCount; i++)
            {
                safeBlocks[i] = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            }
        }

        private void ColorizeGrid(float phase)
        {
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    gridMesh[x, y].material.color = IsSafeBlock(x, y)
                        ? safeBlockColor
                        : Color.Lerp(otherBlocksColorStart, otherBlocksColorEnd, phase);
                }
            }
        }

        private bool IsSafeBlock(int x, int y)
        {
            return safeBlocks.Any(safeBlock => x == safeBlock.x && y == safeBlock.y);
        }

        private void GenerateGrid()
        {
            gridMesh = new MeshRenderer[gridSize.x, gridSize.y];
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var centerTransform = transform;
                    var position = centerTransform.position
                                   + new Vector3(blockSize.x * (x - gridSize.x / 2), 0,
                                       blockSize.y * (y - gridSize.y / 2))
                                   + new Vector3(blockSpacing.x * x, 0, blockSpacing.y * y);
                    var gm = Instantiate(gridBlock, position, Quaternion.identity, centerTransform);
                    gm.name = $"grid block ({x}, {y})";
                    gridMesh[x, y] = gm
                        .GetComponent<MeshRenderer>();
                }
            }
        }


        private void CheckPlayerPositions()
        {
            foreach (var playerRoot in PlayerSubManager.PlayerRoots)
            {
                var player = playerRoot.ChaseGreenPlayer;
                var playerRoundData = roundData[playerRoot.PlayerId];
                if (playerRoundData.isEliminated) continue;

                var playerPos = player.transform.position;
                var x = Mathf.RoundToInt((playerPos.x + blockSize.x * gridSize.x / 2) /
                                         (blockSize.x + blockSpacing.x));
                var y = Mathf.RoundToInt((playerPos.z + blockSize.y * gridSize.y / 2) /
                                         (blockSize.y + blockSpacing.y));

                if (IsSafeBlock(x, y))
                {
                    playerRoundData.UpdateScore(survivalBonus);
                }
                else
                {
                    playerRoundData.isEliminated = true;
                    playerRoundData.eliminationColor = eliminationColor;
                    playerRoundData.UpdateUI();
                    print($"Player {player.name} eliminated {playerRoundData.place}");
                    player.gameObject.SetActive(false);
                }
            }
        }

        public Vector3[] GetGridCornerPositions()
        {
            return new Vector3[4]
            {
                gridMesh[0, 0].transform.position,
                gridMesh[0, gridSize.y - 1].transform.position,
                gridMesh[gridSize.x - 1, 0].transform.position,
                gridMesh[gridSize.x - 1, gridSize.y - 1].transform.position,
            };
        }
    }
}