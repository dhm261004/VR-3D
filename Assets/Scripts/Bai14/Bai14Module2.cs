using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Module 2 — Tính chất phép chiếu song song
/// Step 1 (auto) : Hình bình hành ABCD lơ lửng
/// Step 2 (Space): Tia sáng + bóng A′B′C′D′
/// Step 3 (Space): Nhãn A′B′∥C′D′, A′D′∥B′C′ → "luôn là hình bình hành"
/// Step 4 (Space): Đo |AB| và |A′B′| → AB ≠ A′B′ (không bảo toàn độ dài)
/// Step 5 (Space): Đo tỉ số AB/DC = A′B′/D′C′ = 1 (bảo toàn tỉ số)
/// R = reset
/// </summary>
public class Bai14Module2 : MonoBehaviour
{
    [Header("Màu")]
    public Color floorColor    = new Color(0.93f, 0.90f, 0.78f, 0.80f);
    public Color planeABCDColor= new Color(0.35f, 0.60f, 1f,   0.50f);
    public Color edgeABCDColor = new Color(0.5f,  0.8f,  1f,   1f);
    public Color rayColor      = new Color(1f,    0.95f, 0.30f, 0.45f);
    public Color shadowEdgeColor= new Color(0.25f, 0.25f, 0.25f, 1f);
    public Color highlightAB   = new Color(0.2f,  1f,    0.3f,  1f);
    public Color highlightABp  = new Color(1f,    0.25f, 0.25f, 1f);
    public Color ratioColor    = new Color(1f,    0.9f,  0.1f,  1f);
    public Color conclusionColor= new Color(0f,   1f,    0.5f,  1f);

    private static readonly Vector3 Delta = new Vector3(1f, -2f, 0.5f);

    // Hình bình hành ABCD nghiêng (các đỉnh ở độ cao khác nhau → AB ≠ A′B′)
    private static readonly Vector3 A = new Vector3(-1.2f, 1.5f, -0.5f);
    private static readonly Vector3 B = new Vector3( 0.8f, 1.0f, -0.5f);
    private static readonly Vector3 C = new Vector3( 1.2f, 1.0f,  0.5f);
    private static readonly Vector3 D = new Vector3(-0.8f, 1.5f,  0.5f);

    private int  _step;
    private bool _busy;

    // Cache hình chiếu để dùng lại ở các bước sau
    private Vector3 _Ap, _Bp, _Cp, _Dp;

    void Start() => StartCoroutine(Step1());
    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !_busy) Advance();
        if (Keyboard.current.rKey.wasPressedThisFrame) ResetAll();
    }

    void Advance()
    {
        _step++;
        switch (_step)
        {
            case 2: StartCoroutine(Step2()); break;
            case 3: StartCoroutine(Step3()); break;
            case 4: StartCoroutine(Step4()); break;
            case 5: StartCoroutine(Step5()); break;
        }
    }

    void ResetAll()
    {
        StopAllCoroutines();
        foreach (Transform c in transform) Destroy(c.gameObject);
        _step = 0; _busy = false;
        StartCoroutine(Step1());
    }

    // ── Step 1: Sàn + hình bình hành ABCD ────────────────────────────────
    IEnumerator Step1()
    {
        _busy = true; _step = 1;

        var floorScale = new Vector3(7f, 1f, 6f);
        var floor = MakeQuad(new Vector3(0.2f, 0f, 0.1f), floorColor);
        floor.transform.DOScale(floorScale, 0.6f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.7f);

        // Mặt bình hành (Quad giả làm mặt, chỉ đẹp hơn đường viền)
        DrawPoly(new[] { A, B, C, D }, edgeABCDColor, 0.06f);
        yield return new WaitForSeconds(0.4f);

        string[] names = { "A", "B", "C", "D" };
        Vector3[] ABCD = { A, B, C, D };
        Vector3[] off  = { new Vector3(-0.2f,0.15f,0), new Vector3(0.2f,0.15f,0),
                           new Vector3(0.2f,0.15f,0),  new Vector3(-0.2f,0.15f,0) };
        for (int i = 0; i < 4; i++) SpawnLabel(ABCD[i] + off[i], names[i], edgeABCDColor, 2.5f);

        SpawnLabel(new Vector3(0f, -0.5f, -1.5f), "[Space] → tia sáng", Color.gray, 1.6f);
        _busy = false;
    }

    // ── Step 2: Tia sáng + bóng A′B′C′D′ ────────────────────────────────
    IEnumerator Step2()
    {
        _busy = true;

        _Ap = Project(A); _Bp = Project(B); _Cp = Project(C); _Dp = Project(D);

        Vector3[] ABCD = { A, B, C, D };
        Vector3[] prj  = { _Ap, _Bp, _Cp, _Dp };

        for (int i = 0; i < 4; i++)
        {
            AnimateRay(ABCD[i], prj[i], rayColor);
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(0.5f);

        DrawPoly(prj, shadowEdgeColor, 0.07f);
        yield return new WaitForSeconds(0.3f);

        string[] sn = { "A′", "B′", "C′", "D′" };
        for (int i = 0; i < 4; i++)
            SpawnLabel(prj[i] + Vector3.up * 0.08f, sn[i], new Color(1f,0.5f,0.1f), 2.2f);

        SpawnLabel(new Vector3(0f, -0.5f, -1.5f), "[Space] → tính chất song song", Color.gray, 1.6f);
        _busy = false;
    }

    // ── Step 3: A′B′∥C′D′, A′D′∥B′C′ → kết luận ─────────────────────────
    IEnumerator Step3()
    {
        _busy = true;

        SpawnLabel((_Ap + _Bp) * 0.5f + Vector3.up * 0.15f,
                   "A′B′ ∥ C′D′", conclusionColor, 2f);
        yield return new WaitForSeconds(0.5f);
        SpawnLabel((_Ap + _Dp) * 0.5f + Vector3.up * 0.15f,
                   "A′D′ ∥ B′C′", conclusionColor, 2f);
        yield return new WaitForSeconds(0.6f);

        // Kết luận nổi bật
        var go = MakeLabelGO(new Vector3(0f, 1.8f, -0.3f),
            "Bóng của hình bình hành\nluôn là một hình bình hành!",
            conclusionColor, 2.8f);
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one * 1.1f, 0.5f).SetEase(Ease.OutBack);

        SpawnLabel(new Vector3(0f, -0.5f, -1.5f), "[Space] → đo độ dài", Color.gray, 1.6f);
        _busy = false;
    }

    // ── Step 4: Đo |AB| và |A′B′| → không bảo toàn ───────────────────────
    IEnumerator Step4()
    {
        _busy = true;

        float lenAB  = Vector3.Distance(A, B);
        float lenApBp = Vector3.Distance(_Ap, _Bp);

        // Highlight cạnh AB (màu xanh lá)
        DrawLine(A, B, highlightAB, 0.1f);
        yield return new WaitForSeconds(0.3f);
        SpawnLabel((A + B) * 0.5f + new Vector3(0f, 0.2f, 0f),
                   $"|AB| = {lenAB:F2}", highlightAB, 2.2f);
        yield return new WaitForSeconds(0.7f);

        // Highlight cạnh A′B′ (màu đỏ)
        DrawLine(_Ap, _Bp, highlightABp, 0.1f);
        yield return new WaitForSeconds(0.3f);
        SpawnLabel((_Ap + _Bp) * 0.5f + new Vector3(0f, 0.15f, 0f),
                   $"|A′B′| = {lenApBp:F2}", highlightABp, 2.2f);
        yield return new WaitForSeconds(0.7f);

        // Cảnh báo
        var warn = MakeLabelGO(new Vector3(0f, 1.5f, -0.5f),
            "⚠  AB ≠ A′B′\nPhép chiếu KHÔNG bảo toàn độ dài!",
            new Color(1f, 0.4f, 0.1f), 2.5f);
        warn.transform.localScale = Vector3.zero;
        warn.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

        SpawnLabel(new Vector3(0f, -0.5f, -1.5f), "[Space] → đo tỉ số", Color.gray, 1.6f);
        _busy = false;
    }

    // ── Step 5: Tỉ số AB/DC = A′B′/D′C′ = 1 → bảo toàn ─────────────────
    IEnumerator Step5()
    {
        _busy = true;

        float lenAB   = Vector3.Distance(A,  B);
        float lenDC   = Vector3.Distance(D,  C);
        float lenApBp = Vector3.Distance(_Ap, _Bp);
        float lenDpCp = Vector3.Distance(_Dp, _Cp);

        float ratioTop = lenAB  / lenDC;
        float ratioBtm = lenApBp / lenDpCp;

        SpawnLabel((A + B) * 0.5f + new Vector3(0f, 0.35f, 0f),
                   $"AB / DC = {ratioTop:F2}", ratioColor, 2f);
        yield return new WaitForSeconds(0.6f);
        SpawnLabel((_Ap + _Bp) * 0.5f + new Vector3(0f, 0.2f, 0f),
                   $"A′B′ / D′C′ = {ratioBtm:F2}", ratioColor, 2f);
        yield return new WaitForSeconds(0.8f);

        var go = MakeLabelGO(new Vector3(0f, 2.2f, -0.3f),
            "✔  Phép chiếu LUÔN bảo toàn\ntỉ số độ dài trên hai đường song song!",
            conclusionColor, 2.6f);
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one * 1.05f, 0.5f).SetEase(Ease.OutBack);

        _busy = false;
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    static Vector3 Project(Vector3 p)
    {
        float t = -p.y / Delta.y;
        return p + t * Delta;
    }

    void DrawPoly(Vector3[] pts, Color c, float w)
    {
        var go = new GameObject("Poly");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = pts.Length + 1;
        for (int i = 0; i < pts.Length; i++) lr.SetPosition(i, pts[i]);
        lr.SetPosition(pts.Length, pts[0]);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
        lr.useWorldSpace = true;
    }

    void DrawLine(Vector3 a, Vector3 b, Color c, float w)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, a); lr.SetPosition(1, b);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
        lr.useWorldSpace = true;
    }

    void AnimateRay(Vector3 from, Vector3 to, Color c)
    {
        var go = new GameObject("Ray");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, from); lr.SetPosition(1, from);
        lr.startWidth = lr.endWidth = 0.025f;
        lr.material = MakeMat(c);
        lr.useWorldSpace = true;
        float p = 0f;
        DOTween.To(() => p, v => { p = v; if (lr) lr.SetPosition(1, Vector3.Lerp(from, to, v)); },
                   1f, 0.4f).SetEase(Ease.Linear);
    }

    void SpawnLabel(Vector3 pos, string text, Color c, float size = 2f)
    {
        var go = MakeLabelGO(pos, text, c, size);
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
    }

    GameObject MakeLabelGO(Vector3 pos, string text, Color c, float size)
    {
        var go = new GameObject("Lbl");
        go.transform.SetParent(transform);
        go.transform.position = pos;
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = c; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        return go;
    }

    GameObject MakeQuad(Vector3 pos, Color c)
    {
        var p = GameObject.CreatePrimitive(PrimitiveType.Quad);
        p.transform.SetParent(transform);
        p.transform.position = pos;
        p.transform.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(p.GetComponent<Collider>());
        p.GetComponent<Renderer>().material = MakeMat(c, transparent: true);
        p.transform.localScale = Vector3.zero;
        return p;
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
}
