using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;












public class Bai14Module3 : MonoBehaviour
{
    [Header("Màu")]
    public Color floorColor    = new Color(0.93f, 0.90f, 0.78f, 0.80f);
    public Color circleColor   = new Color(0.3f,  0.7f,  1f,    1f);
    public Color ellipseColor  = new Color(0.2f,  0.2f,  0.2f,  0.85f);
    public Color cubeColor     = new Color(0.8f,  0.55f, 0.2f,  1f);
    public Color shadowColor   = new Color(0.25f, 0.25f, 0.25f, 0.9f);
    public Color gridColor     = new Color(0.6f,  0.6f,  0.6f,  0.5f);
    public Color eFrontColor   = new Color(0.2f,  0.9f,  0.5f,  1f);
    public Color eBackColor    = new Color(0.15f, 0.65f, 0.35f, 0.8f);
    public Color eDashColor    = new Color(0.55f, 0.55f, 0.55f, 0.9f);
    public Color rayColor      = new Color(1f,    0.95f, 0.30f, 0.45f);

    private static readonly Vector3 Delta = new Vector3(1f, -2f, 0.5f);

    
    private static readonly Vector3 ExtrudeDir = new Vector3(0.35f, 0f, -0.5f);
    private const float ExtrudeLen = 1f;

    private int  _step;
    private bool _busy;

    
    private readonly List<GameObject> _phase1Objects = new List<GameObject>();

    void Start() => StartCoroutine(Step1());
    void Update()
    {
        if (LessonInputBridge.NextPressed && !_busy) Advance();
        if (LessonInputBridge.ResetPressed) ResetAll();
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
            case 6: StartCoroutine(Step6()); break;
            case 7: StartCoroutine(Step7()); break;
        }
    }

    void ResetAll()
    {
        StopAllCoroutines();
        foreach (Transform c in transform) Destroy(c.gameObject);
        _phase1Objects.Clear();
        _step = 0; _busy = false;
        StartCoroutine(Step1());
    }

    
    IEnumerator Step1()
    {
        _busy = true; _step = 1;

        var floorScale = new Vector3(8f, 1f, 7f);
        var floor = MakeQuad(new Vector3(0.5f, 0f, 0.2f), floorColor);
        floor.transform.DOScale(floorScale, 0.6f).SetEase(Ease.OutBack);
        _phase1Objects.Add(floor);
        yield return new WaitForSeconds(0.7f);

        
        var circle = MakeCircleXZ(new Vector3(-1.2f, 1.5f, 0f), 0.9f, 64, circleColor, 0.05f);
        _phase1Objects.Add(circle);
        yield return new WaitForSeconds(0.3f);

        SpawnLabel(new Vector3(-1.2f, 1.85f, 0f), "Hình tròn", circleColor, 2.3f);
        SpawnLabel(new Vector3(0f, -0.5f, -2f), "[Space] → chiếu xuống sàn", Color.gray, 1.6f);
        _busy = false;
    }

    
    IEnumerator Step2()
    {
        _busy = true;

        
        Vector3 center = new Vector3(-1.2f, 1.5f, 0f);
        const int rayCount = 8;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * Mathf.PI * 2f / rayCount;
            Vector3 pt  = center + new Vector3(Mathf.Cos(angle) * 0.9f, 0f, Mathf.Sin(angle) * 0.9f);
            AnimateRay(pt, ProjectToFloor(pt), rayColor);
        }
        yield return new WaitForSeconds(0.6f);

        
        var ellipseGO = MakeProjectedCircleXZ(center, 0.9f, 64, ellipseColor, 0.06f);
        _phase1Objects.Add(ellipseGO);
        yield return new WaitForSeconds(0.4f);

        
        var lbl = MakeLabelGO(new Vector3(-1.2f, 0.6f, 0f),
            "Hình biểu diễn\ncủa hình tròn là Elip", Color.white, 2.2f);
        lbl.transform.localScale = Vector3.zero;
        lbl.transform.DOScale(Vector3.one * 1.05f, 0.4f).SetEase(Ease.OutBack);
        _phase1Objects.Add(lbl);

        SpawnLabel(new Vector3(0f, -0.5f, -2f), "[Space] → khối lập phương", Color.gray, 1.6f);
        _busy = false;
    }

    
    IEnumerator Step3()
    {
        _busy = true;

        Vector3 cubeOrigin = new Vector3(1.5f, 0.8f, -0.4f);
        float   side       = 0.8f;
        var cube = MakeCubeWireframe(cubeOrigin, side, cubeColor, 0.04f);
        foreach (var g in cube) _phase1Objects.Add(g);
        yield return new WaitForSeconds(0.4f);

        SpawnLabel(cubeOrigin + new Vector3(0.5f, 0.6f, 0f), "Khối lập phương", cubeColor, 2.2f);
        SpawnLabel(new Vector3(0f, -0.5f, -2f), "[Space] → chiếu xuống sàn", Color.gray, 1.6f);
        _busy = false;
    }

    
    IEnumerator Step4()
    {
        _busy = true;

        Vector3 o = new Vector3(1.5f, 0.8f, -0.4f);
        float   s = 0.8f;
        
        Vector3[] verts =
        {
            o,                         o + new Vector3(s,0,0),
            o + new Vector3(s,0,s),    o + new Vector3(0,0,s),
            o + new Vector3(0,s,0),    o + new Vector3(s,s,0),
            o + new Vector3(s,s,s),    o + new Vector3(0,s,s),
        };

        
        var proj = new Vector3[8];
        for (int i = 0; i < 8; i++) proj[i] = ProjectToFloor(verts[i]);

        
        var topShadow = new Vector3[] { proj[4], proj[5], proj[6], proj[7] };
        DrawPolyLoop(topShadow, shadowColor, 0.05f);
        yield return new WaitForSeconds(0.4f);

        var lbl2 = MakeLabelGO(new Vector3(1.5f, 0.3f, -0.1f),
            "Hình vuông → Hình bình hành\n(góc vuông không bảo toàn)",
            Color.white, 2.1f);
        lbl2.transform.localScale = Vector3.zero;
        lbl2.transform.DOScale(Vector3.one * 1.02f, 0.4f).SetEase(Ease.OutBack);
        _phase1Objects.Add(lbl2);

        SpawnLabel(new Vector3(0f, -0.5f, -2f), "[Space] → mini-game Extrude chữ E", Color.gray, 1.6f);
        _busy = false;
    }

    
    IEnumerator Step5()
    {
        _busy = true;

        
        foreach (var g in _phase1Objects)
            if (g != null) g.transform.DOScale(Vector3.zero, 0.3f);
        yield return new WaitForSeconds(0.4f);

        
        SpawnGrid(new Vector3(-2f, 0.01f, -2f), 6, 6, 0.5f);
        yield return new WaitForSeconds(0.3f);

        
        DrawFlatE(Vector3.up * 0.02f, eFrontColor, 0.05f);
        yield return new WaitForSeconds(0.4f);

        SpawnLabel(new Vector3(0f, 0.8f, -2f),
            "Chữ E phẳng (2D)\ntrên mặt phẳng kẻ ô vuông", eFrontColor, 2.2f);
        SpawnLabel(new Vector3(0f, -0.5f, -2.3f),
            "[Space] → Extrude (kéo giãn 3D)", Color.yellow, 1.8f);
        _busy = false;
    }

    
    IEnumerator Step6()
    {
        _busy = true;

        
        Vector3 offset = ExtrudeDir.normalized * ExtrudeLen;
        DrawFlatE(offset + Vector3.up * 0.02f, eBackColor, 0.04f);
        yield return new WaitForSeconds(0.2f);

        
        var frontPts = GetEOutlineXZ();
        for (int i = 0; i < frontPts.Length; i++)
        {
            Vector3 f = new Vector3(frontPts[i].x, 0.02f, frontPts[i].y);
            Vector3 b = f + offset;
            bool hidden = IsHiddenEdge(i, frontPts.Length);
            if (hidden)
                DrawDashedLine(f, b, eDashColor, 0.02f, 0.07f, 0.04f);
            else
                DrawLine(f, b, eFrontColor, 0.04f);
        }
        yield return new WaitForSeconds(0.4f);

        
        var result = MakeLabelGO(new Vector3(0f, 2f, -0.5f),
            "Khối chữ E 3D hoàn chỉnh!\nDùng phép chiếu song song\nđể vẽ hình không gian.",
            new Color(0.2f, 1f, 0.5f), 2.5f);
        result.transform.localScale = Vector3.zero;
        result.transform.DOScale(Vector3.one * 1.1f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.8f);

        SpawnLabel(new Vector3(0f, -0.5f, -2.3f), "[Space] → Hoàn thành! 🎉", Color.yellow, 1.8f);
        _busy = false;
    }

    
    IEnumerator Step7()
    {
        _busy = true;

        var banner = MakeLabelGO(new Vector3(0f, 2.8f, 0f),
            "🎉  Chúc mừng!\nBạn đã hoàn thành\nChương IV — Quan hệ song song!",
            new Color(1f, 0.9f, 0.1f), 3.5f);
        banner.transform.localScale = Vector3.zero;
        banner.transform.DOScale(Vector3.one * 1.15f, 0.55f).SetEase(Ease.OutBack);

        
        Color[] fireworkColors = {
            Color.red, Color.cyan, Color.yellow, Color.green,
            new Color(1f,0.5f,0f), Color.magenta, Color.white
        };
        for (int burst = 0; burst < 3; burst++)
        {
            yield return new WaitForSeconds(0.35f);
            Vector3 center = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-0.5f, 0.5f));
            for (int i = 0; i < 12; i++)
            {
                var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.transform.SetParent(transform);
                ball.transform.localPosition = center;
                ball.transform.localScale = Vector3.one * 0.12f;
                Destroy(ball.GetComponent<Collider>());
                Color col = fireworkColors[i % fireworkColors.Length];
                ball.GetComponent<Renderer>().material = MakeMat(col);

                Vector3 dir = new Vector3(
                    Mathf.Cos(i * Mathf.PI * 2f / 12f),
                    Random.Range(0.3f, 0.8f),
                    Mathf.Sin(i * Mathf.PI * 2f / 12f));
                ball.transform.DOLocalMove(center + dir * 1.4f, 0.7f).SetEase(Ease.OutQuad);
                ball.transform.DOScale(Vector3.zero, 0.7f).SetDelay(0.3f);
                Destroy(ball, 1.2f);
            }
        }

        yield return new WaitForSeconds(1.5f);
        _busy = false;
    }

    

    
    Vector2[] GetEOutlineXZ()
    {
        return new Vector2[]
        {
            new Vector2(0f,    0f),
            new Vector2(1.2f,  0f),
            new Vector2(1.2f,  0.35f),
            new Vector2(0.3f,  0.35f),
            new Vector2(0.3f,  0.65f),
            new Vector2(1.0f,  0.65f),
            new Vector2(1.0f,  0.95f),
            new Vector2(0.3f,  0.95f),
            new Vector2(0.3f,  1.25f),
            new Vector2(1.2f,  1.25f),
            new Vector2(1.2f,  1.6f),
            new Vector2(0f,    1.6f),
        };
    }

    void DrawFlatE(Vector3 localOffset, Color c, float w)
    {
        var pts2D = GetEOutlineXZ();
        var pts3D = new Vector3[pts2D.Length + 1];
        for (int i = 0; i < pts2D.Length; i++)
            pts3D[i] = new Vector3(pts2D[i].x - 0.6f, 0f, pts2D[i].y - 0.8f) + localOffset;
        pts3D[pts2D.Length] = pts3D[0];

        var go = new GameObject("FlatE");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = pts3D.Length;
        lr.SetPositions(pts3D);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
    }

    
    
    bool IsHiddenEdge(int i, int total)
    {
        
        var pts = GetEOutlineXZ();
        float x = pts[i % pts.Length].x;
        return x < 0.05f;
    }

    
    GameObject MakeCircleXZ(Vector3 center, float r, int n, Color c, float w)
    {
        var go = new GameObject("Circle");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = n + 1;
        for (int i = 0; i <= n; i++)
        {
            float a = i * Mathf.PI * 2f / n;
            lr.SetPosition(i, center + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r));
        }
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
        return go;
    }

    
    GameObject MakeProjectedCircleXZ(Vector3 center, float r, int n, Color c, float w)
    {
        var go = new GameObject("Ellipse");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = n + 1;
        for (int i = 0; i <= n; i++)
        {
            float   a  = i * Mathf.PI * 2f / n;
            Vector3 pt = center + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
            lr.SetPosition(i, ProjectToFloor(pt));
        }
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
        return go;
    }

    
    List<GameObject> MakeCubeWireframe(Vector3 o, float s, Color c, float w)
    {
        var list = new List<GameObject>();
        Vector3[] v =
        {
            o,                         o+new Vector3(s,0,0),
            o+new Vector3(s,0,s),      o+new Vector3(0,0,s),
            o+new Vector3(0,s,0),      o+new Vector3(s,s,0),
            o+new Vector3(s,s,s),      o+new Vector3(0,s,s),
        };
        int[,] edges =
        {
            {0,1},{1,2},{2,3},{3,0}, 
            {4,5},{5,6},{6,7},{7,4}, 
            {0,4},{1,5},{2,6},{3,7}  
        };
        for (int e = 0; e < edges.GetLength(0); e++)
            list.Add(DrawLineGO(v[edges[e,0]], v[edges[e,1]], c, w));
        return list;
    }

    void SpawnGrid(Vector3 origin, int cols, int rows, float cellSize)
    {
        float w = cols * cellSize, h = rows * cellSize;
        for (int i = 0; i <= cols; i++)
        {
            float x = origin.x + i * cellSize;
            DrawLine(new Vector3(x, origin.y, origin.z),
                     new Vector3(x, origin.y, origin.z + h), gridColor, 0.015f);
        }
        for (int j = 0; j <= rows; j++)
        {
            float z = origin.z + j * cellSize;
            DrawLine(new Vector3(origin.x, origin.y, z),
                     new Vector3(origin.x + w, origin.y, z), gridColor, 0.015f);
        }
    }

    void AnimateRay(Vector3 from, Vector3 to, Color c)
    {
        var go = new GameObject("Ray");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, from); lr.SetPosition(1, from);
        lr.startWidth = lr.endWidth = 0.02f;
        lr.material = MakeMat(c);
        float p = 0f;
        DOTween.To(() => p, v => { p = v; if (lr) lr.SetPosition(1, Vector3.Lerp(from, to, v)); },
                   1f, 0.4f).SetEase(Ease.Linear);
    }

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
        DrawLineGO(a, b, c, w);
    }

    GameObject DrawLineGO(Vector3 a, Vector3 b, Color c, float w)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, a); lr.SetPosition(1, b);
        lr.startWidth = lr.endWidth = w;
        lr.material = MakeMat(c);
        return go;
    }

    
    void DrawDashedLine(Vector3 from, Vector3 to, Color c, float w, float dashLen, float gapLen)
    {
        float totalLen = Vector3.Distance(from, to);
        Vector3 dir    = (to - from).normalized;
        float   dist   = 0f;
        while (dist < totalLen)
        {
            Vector3 a = from + dir * dist;
            float   b = Mathf.Min(dist + dashLen, totalLen);
            DrawLine(a, from + dir * b, c, w);
            dist += dashLen + gapLen;
        }
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
        go.transform.localPosition = pos;
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
        p.transform.localPosition = pos;
        p.transform.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(p.GetComponent<Collider>());
        p.GetComponent<Renderer>().material = MakeMat(c, transparent: true);
        p.transform.localScale = Vector3.zero;
        return p;
    }

    static Vector3 ProjectToFloor(Vector3 p)
    {
        float t = -p.y / Delta.y;
        return p + t * Delta;
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
