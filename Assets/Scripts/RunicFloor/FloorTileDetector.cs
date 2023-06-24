using UnityEngine;

namespace RunicFloor
{
    public class FloorTileDetector : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask detectionLayer;

        void FixedUpdate()
        {
            if (!GameManager.Instance.GameRunning) return;
            
            var detectedColliders = Physics.OverlapSphere(transform.position, radius, detectionLayer);
            foreach (var detectedCollider in detectedColliders)
            {
                if (detectedCollider.CompareTag("Respawn"))
                {
                    GameManager.Instance.PlayerTouchedLava(
                        transform.parent.GetComponent<ThirdPersonController>());
                }

                if (detectedCollider.TryGetComponent(out FloorTile floorTile))
                {
                    floorTile.StartCounter();
                }
            }
        }
    }
}