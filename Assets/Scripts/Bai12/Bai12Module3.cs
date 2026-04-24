using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Module 3 — Tính chất: a // (P), (Q) chứa a, (Q) ∩ (P) = b → a // b
/// 5-bước animation. Space = next step, R = reset.
/// </summary>
public class Bai12Module3 : MonoBehaviour
{
    [Header("Màu")]
    public Color planePColor  = new Color(0.2f, 0.5f, 1f,  0.45f);
    public Color planeQColor  = new Color(0.6f, 0.2f, 1f,  0.4f);
    public Color lineAColor   = new Color(1f,   0.55f, 0f, 1f);
    public Color lineBColor   = new Color(1f,   0.9f,  0f, 1f);
    public Color resultColor  = new Color(0f,   1f,    0.4f, 1f);

    private int _step = 0;
    private bool _busy;

    void Start() => StartCoroutine(Step1());

    void Update()
    {
        if (LessonInputBridge.NextPressed && !_busy) NextStep();
        if (LessonInputBridge.ResetPressed) ResetAll();
    }

    void NextStep()
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

    IEnumerator Step1()
    {
        _busy = true; _step = 1;
        // Mặt phẳng (P)
        var p = MakeQuad(new Vector3(0, 0, 0), new Vector3(3.5f, 1f, 2.5f), planePColor, 0f);
        p.transform.DOScale(new Vector3(3.5f, 1f, 2.5f), 0.7f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.8f);
        SpawnLabel(new Vector3(1.8f, 0.05f, 1.3f), "(P)", new Color(0.6f, 0.8f, 1f));

        // Đường thẳng a song song (P)
        var lineA = MakeLine(new Vector3(-1.5f, 0.8f, 0f), new Vector3(1.5f, 0.8f, 0f), lineAColor);
        SpawnLabel(new Vector3(1.7f, 0.85f, 0f), "a // (P)", lineAColor);
        SpawnLabel(new Vector3(0f, -0.8f, 0f), "[Space] → Bước tiếp", Color.gray, 1.5f);
        _busy = false;
    }

    IEnumerator Step2()
    {
        _busy = true;
        // Mặt phẳng (Q) nghiêng, chứa a
        var q = MakeQuad(new Vector3(0, 0.4f, 0), new Vector3(3.5f, 1f, 2f), planeQColor, 30f);
        q.transform.DOScale(new Vector3(3.5f, 1f, 2f), 0.6f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.7f);
        SpawnLabel(new Vector3(-1.8f, 1.1f, 0.8f), "(Q)", new Color(0.8f, 0.6f, 1f));
        SpawnLabel(new Vector3(0f, 0.5f, -1.2f), "a ⊂ (Q)", planeQColor, 1.8f);
        _busy = false;
    }

    IEnumerator Step3()
    {
        _busy = true;
        // Giao tuyến b = (Q) ∩ (P)
        var lineB = MakeLine(new Vector3(-1.5f, 0f, -0.6f), new Vector3(1.5f, 0f, -0.6f), lineBColor);
        yield return new WaitForSeconds(0.3f);
        SpawnLabel(new Vector3(1.7f, 0.1f, -0.6f), "b", lineBColor);
        SpawnLabel(new Vector3(0f, -0.4f, -0.6f), "b = (Q) ∩ (P)", lineBColor, 1.8f);
        _busy = false;
    }

    IEnumerator Step4()
    {
        _busy = true;
        // Mũi tên song song a và b
        SpawnLabel(new Vector3(0f, 0.45f, -0.3f), "a // b  ?", arrowColor: new Color(1f, 1f, 0.3f), 2.5f);
        yield return new WaitForSeconds(0.8f);
        _busy = false;
    }

    IEnumerator Step5()
    {
        _busy = true;
        var go = new GameObject("Result");
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0f, 1.8f, 0f);
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = "∴ a // b";
        tm.color = resultColor;
        tm.fontSize = 4.5f;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.6f);

        SpawnLabel(new Vector3(0f, 1.1f, 0f),
            "Tính chất xác nhận!\nBất kể (Q) nghiêng góc nào,\na luôn // giao tuyến b.",
            resultColor, 1.6f);
        _busy = false;
    }

    GameObject MakeQuad(Vector3 pos, Vector3 scale, Color c, float rotX)
    {
        var p = GameObject.CreatePrimitive(PrimitiveType.Quad);
        p.transform.SetParent(transform);
        p.transform.localPosition = pos;
        p.transform.localScale = scale;
        p.transform.rotation = Quaternion.Euler(90 - rotX, 0, 0);
        Destroy(p.GetComponent<Collider>());
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        mat.SetFloat("_Surface", 1);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        p.GetComponent<Renderer>().material = mat;
        p.transform.localScale = Vector3.zero;
        return p;
    }

    GameObject MakeLine(Vector3 from, Vector3 to, Color c)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, from); lr.SetPosition(1, to);
        lr.startWidth = lr.endWidth = 0.05f;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        lr.material = mat;
        return go;
    }

    void SpawnLabel(Vector3 pos, string text, Color arrowColor, float size = 2f)
    {
        var go = new GameObject("Lbl");
        go.transform.SetParent(transform);
        go.transform.localPosition = pos;
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = arrowColor; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
    }
}
