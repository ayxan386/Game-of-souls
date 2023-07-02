using UnityEngine;

public class SoulDropCollectible : MonoBehaviour
{
    [SerializeField] private Vector3 animationDirection;
    [SerializeField] private float animationSpeed;
    [SerializeField] private GameObject lightBeam;
    [SerializeField] private float beamDuration;
    [SerializeField] private AudioClip collectionSound;

    [Header("Player discovery")] [SerializeField]
    private float radius;

    [SerializeField] private LayerMask playerLayer;

    private float t;
    public int SoulAmount { get; set; }

    void Update()
    {
        t += animationSpeed * Time.deltaTime;
        transform.Translate(animationDirection * (Time.deltaTime * Mathf.Sin(t)));

        if (t > beamDuration)
        {
            lightBeam.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        var players = Physics.OverlapSphere(transform.position, radius, playerLayer);
        if (players is not { Length: > 0 }) return;

        foreach (var playerCollider in players)
        {
            if (!playerCollider.TryGetComponent(out Player player)) continue;
            PlayerManager.Instance.SfxAudioSource.PlayOneShot(collectionSound);
            player.UpdateSoulCount(SoulAmount);
            Destroy(gameObject);
            break;
        }
    }

    public void SetPosition(PathTile position)
    {
        transform.position = position.GetNextPoint().position;
    }
}