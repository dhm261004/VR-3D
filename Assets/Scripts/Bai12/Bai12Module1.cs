using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Module 1 — Ba vị trí tương đối: Song song / Cắt nhau / Nằm trong
/// Space = play sequence, R = reset
/// </summary>
public class Bai12Module1 : MonoBehaviour
{
    [Header("Màu sắc")]
    public Color planeColor    = new Color(0.2f, 0.5f, 1f, 0.55f);
    public Color parallelColor = new Color(0f, 1f, 0.4f, 1f);
    public Color intersectColor= new Color(1f, 0.15f, 0.15f, 1f);
    public Color insideColor   = new Color(1f, 0.85f, 0f, 1f);

    [Header("Khoảng cách giữa 3 cấu hình")]
    public float spacing = 3f;

    private bool _running;

    void Start()   => StartCoroutine(AutoPlay());
    void Update()
    {
        if (LessonInputBridge.NextPressed && !_running)
            StartCoroutine(AutoPlay());
        if (LessonInputBridge.ResetPressed)
        {
            StopAllCoroutines();
            foreach (Transform child in transform) Destroy(child.gameObject);
            _running = false;
            StartCoroutine(AutoPlay());
        }
    }

    IEnumerator AutoPlay()
    {
        _running = true;
        yield return StartCoroutine(ShowCase(transform.position + Vector3.left * spacing, CaseType.Parallel));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowCase(transform.position, CaseType.Intersect));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowCase(transform.position + Vector3.right * spacing, CaseType.Inside));
        _running = false;
    }

    enum CaseType { Parallel, Intersect, Inside }

    IEnumerator ShowCase(Vector3 center, CaseType type)
    {
        // Mặt phẳng
        GameObject plane = CreatePlane(center, planeColor);
        plane.transform.localScale = Vector3.zero;
        plane.transform.DOScale(new Vector3(2.5f, 1.5f, 1f), 0.6f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.7f);

        Color lineColor;
        string label;
        switch (type)
        {
            case CaseType.Parallel:
                lineColor = parallelColor;
                label = "d // (α)";
                CreateLine(center + Vector3.up * 0.8f + Vector3.left * 1.2f,
                           center + Vector3.up * 0.8f + Vector3.right * 1.2f, lineColor);
                break;
            case CaseType.Intersect:
                lineColor = intersectColor;
                label = "d ∩ (α) = {M}";
                CreateLine(center + Vector3.up * 1.2f, center + Vector3.down * 1.2f, lineColor);
                SpawnIntersectPoint(center);
                break;
            default: // Inside
                lineColor = insideColor;
                label = "d ⊂ (α)";
                CreateLine(center + Vector3.left * 1.2f, center + Vector3.right * 1.2f, lineColor);
                break;
        }

        yield return new WaitForSeconds(0.5f);
        SpawnLabel(center + Vector3.up * 1.6f, label, lineColor);
    }

    GameObject CreatePlane(Vector3 pos, Color c)
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Quad);
        p.transform.SetParent(transform);
        p.transform.position = pos;
        p.transform.localScale = new Vector3(2.5f, 1.5f, 1f);
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

    void CreateLine(Vector3 from, Vector3 to, Color c)
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.startWidth = lr.endWidth = 0.05f;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", c);
        lr.material = mat;
    }

    void SpawnIntersectPoint(Vector3 pos)
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.transform.SetParent(transform);
        p.transform.position = pos;
        p.transform.localScale = Vector3.zero;
        p.transform.DOScale(0.15f, 0.4f).SetEase(Ease.OutBack);
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", Color.white);
        p.GetComponent<Renderer>().material = mat;
        SpawnLabel(pos + Vector3.right * 0.25f + Vector3.up * 0.2f, "M", Color.white);
    }

    void SpawnLabel(Vector3 pos, string text, Color c)
    {
        GameObject go = new GameObject("Label_" + text);
        go.transform.SetParent(transform);
        go.transform.position = pos;
        go.AddComponent<Billboard>();
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = text;
        tm.color = c;
        tm.fontSize = 2.5f;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }
}
