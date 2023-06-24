using UnityEngine;

namespace RunicFloor
{
    public class FloorTile : MonoBehaviour
    {
        [SerializeField] private float stayDuration;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] possiblesRunes;
        [SerializeField] private MeshRenderer backgroundRenderer;
        [SerializeField] private Color colorStart;
        [SerializeField] private Color colorEnd;
        [SerializeField] private AudioClip crackingSound;

        private bool counterStarted;
        private float counter;
        private Color materialColor;

        private void Start()
        {
            materialColor = backgroundRenderer.material.color;
            spriteRenderer.sprite = possiblesRunes[Random.Range(0, possiblesRunes.Length)];
        }

        private void Update()
        {
            if (!counterStarted) return;

            counter += Time.deltaTime;
            backgroundRenderer.material.color = Color.Lerp(colorStart, colorEnd, counter / stayDuration);

            if (counter >= stayDuration)
            {
                Destroy(gameObject);
            }
        }

        public void StartCounter()
        {
            if(!GameManager.Instance.GameRunning) return;
            if (!counterStarted)
                GameManager.Instance.AudioSource.PlayOneShot(crackingSound);
            counterStarted = true;
        }
    }
}