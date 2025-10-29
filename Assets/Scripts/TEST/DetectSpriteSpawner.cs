using UnityEngine;

public enum DetectShapeType {
    Rectangle,
    Circle
}

public class DetectSpriteSpawner : MonoBehaviour {
    [Header("基本設定")]
    public DetectShapeType ShapeType = DetectShapeType.Rectangle;

    [Header("共同對象")]
    [SerializeField, HideInInspector] private Transform targetTransform;
    private Color color = new Color(81f / 255f, 163f / 255f, 255f / 255f,   89f / 255f);
    private int referencePPU = 100;       // 與相機一致
    private float alphaEdge = 0f;
    private int sortingOrder = -10;

    [Header("矩形設定")]
    [SerializeField, HideInInspector] private Sprite rectangleSprite;
    [SerializeField, HideInInspector] private float rangeX = 3f;
    [SerializeField, HideInInspector] private float rangeY = 2f;

    [Header("圓形設定")]
    [SerializeField, HideInInspector] private Sprite circleSprite;
    [SerializeField, HideInInspector] private int radius = 3;


    void Start() {
        //var obj = CreateCircleObject(radius, targetTransform);
        var obj = CreateRectangleObject(rangeX, rangeY, targetTransform);
    }

    public GameObject CreateCircleObject(int radius,Transform transform) {
        // 建立物件
        GameObject go = new GameObject("GeneratedCircle");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;

        // 加入 SpriteRenderer
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateCircleSprite(radius);
        sr.color = color;
        sr.sortingOrder = sortingOrder;

        return go;
    }
    public Sprite GenerateCircleSprite(int radius) {
        // 依 PPU 換算成像素半徑
        int radiusPixels = Mathf.RoundToInt(radius * referencePPU);
        int diameter = radiusPixels * 2;

        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Vector2 center = new Vector2(radiusPixels, radiusPixels);
        float softEdge = radiusPixels * alphaEdge;

        for (int y = 0; y < diameter; y++) {
            for (int x = 0; x < diameter; x++) {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f;

                if (dist > radiusPixels - softEdge)
                    alpha = Mathf.Clamp01((radiusPixels - dist) / softEdge);

                if (dist > radiusPixels)
                    tex.SetPixel(x, y, Color.clear);
                else
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        tex.Apply();

        // 用 referencePPU 建立 sprite，確保世界大小正確
        return Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), referencePPU);
    }

    public GameObject CreateRectangleObject(float rangeX, float rangeY, Transform parent) {
        // 建立物件
        GameObject go = new GameObject("GeneratedRectangle");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;

        // 加入 SpriteRenderer
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateRectangleSprite(rangeX, rangeY);
        sr.color = color;
        sr.sortingOrder = sortingOrder;

        return go;
    }
    private Sprite GenerateRectangleSprite(float rangeX, float rangeY) {
        // 轉成像素尺寸（以 PPU 為基準）
        int widthPixels = Mathf.RoundToInt(rangeX * 2 * referencePPU);
        int heightPixels = Mathf.RoundToInt(rangeY * 2 * referencePPU);

        Texture2D tex = new Texture2D(widthPixels, heightPixels, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        // 填滿白色區域
        for (int y = 0; y < heightPixels; y++) {
            for (int x = 0; x < widthPixels; x++) {
                tex.SetPixel(x, y, Color.white);
            }
        }

        tex.Apply();

        // pivot 設在中心，確保在角色正中顯示
        return Sprite.Create(tex, new Rect(0, 0, widthPixels, heightPixels), new Vector2(0.5f, 0.5f), referencePPU);
    }
}
