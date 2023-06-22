using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChaseGreen
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController cc;
        [SerializeField] private TextMeshPro inWorldName;
        [SerializeField] [Range(0, 1f)] private float rotationFactor;

        [Header("Collectibles")] [SerializeField]
        private float radius;

        [SerializeField] private LayerMask collectibleLayer;

        private Vector3 movementVector;

        public TextMeshPro InWorldName => inWorldName;
        public PlayerRoundData RoundData { get; set; }

        void Update()
        {
            if (!PlayerManager.PlayersReady) return;

            animator.SetBool("running", movementVector.sqrMagnitude > 0);
            cc.SimpleMove(movementVector);

            transform.forward = Vector3.Lerp(transform.forward, movementVector.normalized, rotationFactor);


            CheckForCollectibles();
        }

        private void CheckForCollectibles()
        {
            var collectibles = Physics.OverlapSphere(transform.position, radius, collectibleLayer);
            if (collectibles.Length > 0)
            {
                foreach (var collectible in collectibles)
                {
                    if (collectible.TryGetComponent(out Collectible coll))
                    {
                        coll.PickedUp();
                        RoundData.UpdateScore(coll.AwardAmount);
                        Destroy(coll.gameObject);
                    }
                }
            }
        }

        private void OnDisable()
        {
            movementVector = Vector3.zero;
        }

        private void OnMove(InputValue inputValue)
        {
            var vec = inputValue.Get<Vector2>();
            vec.Normalize();
            movementVector = new Vector3(vec.x, 0, vec.y) * speed;
        }

        public void TeleportToPosition(Vector3 pos)
        {
            cc.enabled = false;
            transform.position = pos;
            cc.enabled = true;
        }
    }
}