using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NeonVisuals : MonoBehaviour
{
    public enum NeonType { Player, Obstacle, Guide }

    [SerializeField] private NeonType type;
    [SerializeField] private bool generateSpriteIfMissing = true;

    private void Start()
    {
        InitializeVisuals();
    }

    private void InitializeVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Set Color
        Color neonColor = Color.white;
        switch (type)
        {
            case NeonType.Player:
                neonColor = Color.cyan;
                break;
            case NeonType.Obstacle:
                neonColor = Color.magenta; // Pinkish
                break;
            case NeonType.Guide:
                neonColor = Color.white;
                break;
        }
        
        // Ensure bright neon look (HDR intensity emulation for standard sprite)
        // If URP 2D is used, this color interacts with Post Processing Bloom.
        sr.color = neonColor;

        // Generate Sprite if missing
        if (generateSpriteIfMissing && sr.sprite == null)
        {
            sr.sprite = CreateCircleSprite();
        }
    }

    private Sprite CreateCircleSprite()
    {
        int res = 64;
        Texture2D texture = new Texture2D(res, res);
        texture.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[res * res];
        Vector2 center = new Vector2(res / 2f, res / 2f);
        float radius = res / 2f - 2;

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius)
                {
                    // Antialiasing / Soft Edge
                    float t = Mathf.Clamp01(radius - dist);
                    pixels[y * res + x] = Color.white * t; 
                }
                else
                {
                    pixels[y * res + x] = Color.clear;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
    }
}
