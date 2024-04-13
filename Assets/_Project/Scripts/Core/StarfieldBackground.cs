using UnityEngine;

/// <summary>
/// Procedurally generates a static field of small white circles
/// behind the gameplay layer to give a sense of depth and motion.
/// Runs once on Start, no per-frame cost.
/// </summary>
public class StarfieldBackground : MonoBehaviour
{
    [Header("Starfield Settings")]
    [SerializeField] private int starCount = 120;
    [SerializeField] private float fieldWidth = 30f;
    [SerializeField] private float fieldHeight = 20f;
    [SerializeField] private float minStarSize = 0.02f;
    [SerializeField] private float maxStarSize = 0.08f;

    private void Start()
    {
        GenerateStars();
    }

    private void GenerateStars()
    {
        Sprite circleSprite = CreateCircleSprite();

        for (int i = 0; i < starCount; i++)
        {
            GameObject star = new GameObject($"Star_{i}");
            star.transform.parent = transform;

            float x = Random.Range(-fieldWidth / 2f, fieldWidth / 2f);
            float y = Random.Range(-fieldHeight / 2f, fieldHeight / 2f);
            star.transform.position = new Vector3(x, y, 1f);

            SpriteRenderer sr = star.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;

            float size = Random.Range(minStarSize, maxStarSize);
            star.transform.localScale = new Vector3(size, size, 1f);

            float brightness = Random.Range(0.4f, 1f);
            sr.color = new Color(brightness, brightness, brightness, 1f);

            sr.sortingOrder = -10; // always behind gameplay
        }
    }

    private Sprite CreateCircleSprite()
    {
        const int resolution = 16;
        Texture2D tex = new Texture2D(resolution, resolution);
        Color[] pixels = new Color[resolution * resolution];

        float center = resolution / 2f;
        float radius = resolution / 2f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dist = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(center, center));
                pixels[y * resolution + x] =
                    dist <= radius ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(
            tex,
            new Rect(0, 0, resolution, resolution),
            new Vector2(0.5f, 0.5f));
    }
}