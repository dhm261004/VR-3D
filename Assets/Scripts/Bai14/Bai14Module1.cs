using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Module 1 — Khái niệm phép chiếu song song
/// Khung cửa ABCD → tia sáng song song Δ → bóng A′B′C′D′ trên sàn α
/// Space = replay  |  R = reset
/// </summary>
public class Bai14Module1 : MonoBehaviour
{
    [Header("Màu")]
    public Color floorColor       = new Color(0.95f, 0.92f, 0.80f, 0.85f);
    public Color frameColor       = new Color(0.85f, 0.60f, 0.15f, 1f);
    public Color rayColor         = new Color(1f,    0.95f, 0.30f, 0.55f);
    public Color shadowColor      = new Color(0.25f, 0.25f, 0.25f, 0.85f);
    public Color labelPrimary     = new Color(0.3f,  0.85f, 1f,   1f);
    public Color labelShadow      = new Color(1f,    0.5f,  0.1f, 1f);

    // Phương chiếu Δ (dùng để tính t = -P.y / Delta.y)
    private static readonly Vector3 Delta = new Vector3(1f, -2f, 0.5f);

    // 4 đỉnh khung cửa ABCD tại y = 2
    private static readonly Vector3[] ABCDFrame =
    {
        new Vector3(-0.8f,  2f, -0.8f), // A
        new Vector3( 0.8f,  2f, -0.8f), // B
        new Vector3( 0.8f,  2f,  0.8f), // C
        new Vector3(-0.8f,  2f,  0.8f), // D
    };

    private bool _running;

    void Start()  => StartCoroutine(Play());
    void Update()
    {
        if (LessonInputBridge.NextPressed && !_running)
            StartCoroutine(Play());
        if (LessonInputBridge.ResetPressed)
        {
            StopAllCoroutines();
            ClearChildren();
            _running = false;
            StartCoroutine(Play());
        }
    }

    // Chiếu điểm P theo Delta xuống mặt y = 0
    static Vector3 ProjectToFloor(Vector3 p)
    {
        float t = -p.y / Delta.y;
        return p + t * Delta;
    }

    IEnumerator Play()
    {
        _running = true;

        // ── Bước 1: Mặt phẳng sàn α ──────────────────────────────────────
        var floorScale = new Vector3(8f, 1f, 7f);
        var floor = MakeQuad(new Vector3(0.6f, 0f, 0.4f), floorColor);
        floor.transform.DOScale(floorScale, 0.7f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.9f);
        SpawnLabel(new Vector3(3.2f, 0.06f, 2.8f), "α  (mặt phẳng chiếu)",
                   new Color(0.55f, 0.55f, 0.55f), 2f);
        yield return new WaitForSeconds(0.5f);

        // ── Bước 2: Khung cửa ABCD ───────────────────────────────────────
        DrawPolyLoop(ABCDFrame, frameColor, 0.06f);
        // Thanh chấn song ngang + dọc bên trong (giống Hình 4.56)
        DrawLine(Midpoint(ABCDFrame[0], ABCDFrame[3]),
                 Midpoint(ABCDFrame[1], ABCDFrame[2]), frameColor, 0.04f); // ngang giữa
        DrawLine(Midpoint(ABCDFrame[0], ABCDFrame[1]),
                 Midpoint(ABCDFrame[3], ABCDFrame[2]), frameColor, 0.04f); // dọc giữa

        yield return new WaitForSeconds(0.4f);

        string[] vertNames = { "A", "B", "C", "D" };
        Vector3[] labelOff  = {
            new Vector3(-0.22f, 0.15f, 0),
            new Vector3( 0.22f, 0.15f, 0),
            new Vector3( 0.22f, 0.15f, 0),
            new Vector3(-0.22f, 0.15f, 0),
        };
        for (int i = 0; i < 4; i++)
            SpawnLabel(ABCDFrame[i] + labelOff[i], vertNames[i], frameColor, 2.5f);
        yield return new WaitForSeconds(0.7f);

        // ── Bước 3: Tia sáng song song Δ (animate từng tia) ──────────────
        var shadows = new Vector3[4];
        for (int i = 0; i < 4; i++) shadows[i] = ProjectToFloor(ABCDFrame[i]);

        for (int i = 0; i < 4; i++)
        {
            AnimateRay(ABCDFrame[i], shadows[i], rayColor);
            yield return new WaitForSeconds(0.18f);
        }
        yield return new WaitForSeconds(0.55f);

        // Nhãn phương chiếu Δ tại giữa một tia
        Vector3 midFrame  = (ABCDFrame[1] + ABCDFrame[2]) * 0.5f;
        Vector3 midShadow = (shadows[1]   + shadows[2])   * 0.5f;
        SpawnLabel(Vector3.Lerp(midFrame, midShadow, 0.45f) + new Vector3(0.55f, 0, 0),
                   "Δ  (phương chiếu)", rayColor, 2f);
        yield return new WaitForSeconds(0.55f);

        // ── Bước 4: Bóng A′B′C′D′ trên sàn ──────────────────────────────
        DrawPolyLoop(shadows, shadowColor, 0.07f);
        yield return new WaitForSeconds(0.35f);

        string[] shadowNames = { "A′", "B′", "C′", "D′" };
        for (int i = 0; i < 4; i++)
            SpawnLabel(shadows[i] + new Vector3(0f, 0.07f, 0f), shadowNames[i], labelShadow, 2.2f);
        yield return new WaitForSeconds(0.6f);

        // ── Bước 5: Kết luận ─────────────────────────────────────────────
        SpawnLabel(new Vector3(0.3f, 1.6f, -1.8f),
            "A′B′C′D′ là hình chiếu\nson song của ABCD theo Δ",
            Color.white, 2.3f);

        _running = false;
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    void DrawPolyLoop(Vector3[] pts, Color c, float w)
    {
        var go = new GameObject("Poly");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = pts.Length + 1;
        for (int i = 0; i < pts.Length; i++) lr.SetPosition(i, pts[i]);
        lr.SetPosition(pts.Length, pts[0]);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
    }

    void DrawLine(Vector3 a, Vector3 b, Color c, float w)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, a); lr.SetPosition(1, b);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
    }

    void AnimateRay(Vector3 from, Vector3 to, Color c)
    {
        var go = new GameObject("Ray");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, from);
        lr.startWidth = lr.endWidth = 0.03f;
        lr.material = MakeMat(c);
        float progress = 0f;
        DOTween.To(() => progress, v => {
            progress = v;
            if (lr != null) lr.SetPosition(1, Vector3.Lerp(from, to, v));
        }, 1f, 0.45f).SetEase(Ease.Linear);
    }

    GameObject MakeQuad(Vector3 pos, Color c)
    {
        var p = GameObject.CreatePrimitive(PrimitiveType.Quad);
        p.transform.SetParent(transform);
        p.transform.localPosition = pos;
        p.transform.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(p.GetComponent<Collider>());
        p.GetComponent<Renderer>().material = MakeMat(c, transparent: true);
        p.transform.localScale = Vector3.zero;
        return p;
    }

    void SpawnLabel(Vector3 pos, string text, Color c, float size = 2f)
    {
        var go = new GameObject("Lbl");
        go.transform.SetParent(transform);
        go.transform.localPosition = pos;
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = c; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
    }

    Material MakeMat(Color c, bool transparent = false)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        if (transparent || c.a < 1f)
        {
            mat.SetFloat("_Surface", 1);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        mat.renderQueue = 3000;
        return mat;
    }

    void ClearChildren()
    {
        foreach (Transform c in transform) Destroy(c.gameObject);
    }

    static Vector3 Midpoint(Vector3 a, Vector3 b) => (a + b) * 0.5f;
}
