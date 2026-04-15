using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private Texture2D baseTexture;
    private Texture2D _cloneTexture;
    private SpriteRenderer _sr;

    private float _widthWorld, _heightWorld;
    private int _widthPixel, _heightPixel;

    public float WidthWorld
    {
        get
        {
            if (_widthWorld == 0)
                _widthWorld = _sr.bounds.size.x;
            return _widthWorld;
        }

    }
    public float HeightWorld
    {
        get
        {
            if (_heightWorld == 0)
                _heightWorld = _sr.bounds.size.y;
            return _heightWorld;
        }

    }
    public int WidthPixel
    {
        get
        {
            if (_widthPixel == 0)
                _widthPixel = _sr.sprite.texture.width;

            return _widthPixel;
        }
    }
    public int HeightPixel
    {
        get
        {
            if (_heightPixel == 0)
                _heightPixel = _sr.sprite.texture.height;

            return _heightPixel;
        }
    }


    // Use this for initialization
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cloneTexture = Instantiate(baseTexture);
        _cloneTexture.alphaIsTransparency = true;

        if (_cloneTexture.format != TextureFormat.ARGB32)
            Debug.LogWarning("Texture must be ARGB32");
        if (_cloneTexture.wrapMode != TextureWrapMode.Clamp)
            Debug.LogWarning("wrapMode must be Clamp");

        UpdateTexture();
        gameObject.AddComponent<PolygonCollider2D>();

    }

    void MakeAHole(CircleCollider2D col)
    {
        print(string.Format("{0},{1},{2},{3}", WidthPixel, HeightPixel, WidthWorld, _heightWorld));

        Vector2Int c = World2Pixel(col.bounds.center);
        int r = Mathf.RoundToInt(col.bounds.size.x * WidthPixel / WidthWorld);

        int px, nx, py, ny, d;
        for (int i = 0; i <= r; i++)
        {
            d = Mathf.RoundToInt(Mathf.Sqrt(r * r - i * i));
            for (int j = 0; j <= d; j++)
            {
                px = c.x + i;
                nx = c.x - i;
                py = c.y + j;
                ny = c.y - j;

                _cloneTexture.SetPixel(px, py, Color.clear);
                _cloneTexture.SetPixel(nx, py, Color.clear);
                _cloneTexture.SetPixel(px, ny, Color.clear);
                _cloneTexture.SetPixel(nx, ny, Color.clear);
            }
        }
        _cloneTexture.Apply();
        UpdateTexture();

        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    void UpdateTexture()
    {
        _sr.sprite = Sprite.Create(_cloneTexture,
                            new Rect(0, 0, _cloneTexture.width, _cloneTexture.height),
                            new Vector2(0.5f, 0.5f),
                            50f
                            );
    }

    Vector2Int World2Pixel(Vector2 pos)
    {
        Vector2Int v = Vector2Int.zero;

        var dx = (pos.x - transform.position.x);
        var dy = (pos.y - transform.position.y);

        v.x = Mathf.RoundToInt(0.5f * WidthPixel + dx * (WidthPixel / WidthWorld));
        v.y = Mathf.RoundToInt(0.5f * HeightPixel + dy * (HeightPixel / HeightWorld));

        return v;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("bullet"))
            return;
        if (!collision.GetComponent<CircleCollider2D>())
            return;

        MakeAHole(collision.GetComponent<CircleCollider2D>());
        Destroy(collision.gameObject, 0.1f);
    }

}
