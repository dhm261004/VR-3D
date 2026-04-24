using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Module 2 — Điều kiện nhận biết d // (P)
/// 4-bước animation: (P) → b trong P → a bên trên → a // b → kết luận a // (P)
/// Space = next step, R = reset
/// </summary>
public class Bai12Module2 : MonoBehaviour
{
    [Header("Màu")]
    public Color planeColor  = new Color(0.2f, 0.5f, 1f, 0.45f);
    public Color lineAColor  = new Color(1f, 0.55f, 0f, 1f);
    public Color lineBColor  = new Color(1f, 1f, 1f, 1f);
    public Color arrowColor  = new Color(1f, 0.9f, 0f, 1f);
    public Color resultColor = new Color(0f, 1f, 0.4f, 1f);

    private int _step = 0;
    private bool _busy;

    private GameObject _plane, _lineB, _lineA, _arrowGroup, _resultLabel;

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
        }
    }

    void ResetAll()
    {
        StopAllCoroutines();
        foreach (Transform c in transform) Destroy(c.gameObject);
        _step = 0; _busy = false;
        _plane = _lineB = _lineA = _arrowGroup = _resultLabel = null;
        StartCoroutine(Step1());
    }

    IEnumerator Step1()
    {
        _busy = true;
        _step = 1;
        // Mặt phẳng (P)
        _plane = CreateQuad(Vector3.zero, new Vector3(3f, 1f, 2f), planeColor);
        _plane.transform.localScale = Vector3.zero;
        _plane.transform.DOScale(new Vector3(3f, 1f, 2f), 0.7f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.8f);

        // Nhãn (P)
        SpawnLabel(new Vector3(1.6f, 0.05f, 1.1f), "(P)", new Color(0.6f, 0.8f, 1f));
        SpawnLabel(new Vector3(0f, -0.6f, 0f), "[Space] → Bước tiếp", Color.gray, 1.5f);
        _busy = false;
    }

    IEnumerator Step2()
    {
        _busy = true;
        // Đường thẳng b nằm trong (P)
        _lineB = CreateLine(new Vector3(-1.2f, 0.02f, 0f), new Vector3(1.2f, 0.02f, 0f), lineBColor);
        yield return new WaitForSeconds(0.3f);
        SpawnLabel(new Vector3(1.4f, 0.15f, 0f), "b ⊂ (P)", lineBColor);
        yield return new WaitForSeconds(0.5f);
        _busy = false;
    }

    IEnumerator Step3()
    {
        _busy = true;
        // Đường thẳng a bên trên, song song (P)
        _lineA = CreateLine(new Vector3(-1.2f, 0.7f, 0f), new Vector3(1.2f, 0.7f, 0f), lineAColor);
        yield return new WaitForSeconds(0.3f);
        SpawnLabel(new Vector3(1.4f, 0.75f, 0f), "a", lineAColor);
        SpawnLabel(new Vector3(-1.6f, 0.75f, 0f), "a ∉ (P)", lineAColor, 1.8f);
        yield return new WaitForSeconds(0.5f);
        _busy = false;
    }

    IEnumerator Step4()
    {
        _busy = true;
        // Mũi tên song song giữa a và b
        SpawnLabel(new Vector3(0f, 0.35f, 0.5f), "a // b", arrowColor, 2.5f);
        yield return new WaitForSeconds(0.6f);

        // Kết quả
        _resultLabel = new GameObject("Result");
        _resultLabel.transform.SetParent(transform);
        _resultLabel.transform.localPosition = new Vector3(0f, 1.3f, 0f);
        _resultLabel.AddComponent<Billboard>();
        var tm = _resultLabel.AddComponent<TextMeshPro>();
        tm.text = "∴ a // (P)";
        tm.color = resultColor;
        tm.fontSize = 4f;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        _resultLabel.transform.localScale = Vector3.zero;
        _resultLabel.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack);

        _busy = false;
    }

    GameObject CreateQuad(Vector3 pos, Vector3 size, Color c)
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Quad);
        p.transform.SetParent(transform);
        p.transform.localPosition = pos;
        p.transform.localScale = size;
        p.transform.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(p.GetComponent<Collider>());
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        mat.SetFloat("_Surface", 1);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        p.GetComponent<Renderer>().material = mat;
        return p;
    }

    GameObject CreateLine(Vector3 from, Vector3 to, Color c)
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.startWidth = lr.endWidth = 0.05f;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        lr.material = mat;
        return go;
    }

    void SpawnLabel(Vector3 pos, string text, Color c, float size = 2f)
    {
        GameObject go = new GameObject("Lbl_" + text);
        go.transform.SetParent(transform);
        go.transform.localPosition = pos;
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = c; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
    }
}
