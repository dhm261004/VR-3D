using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B10_M4_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 180);     
    private Color edgeError = new Color32(255, 0, 0, 255);      
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    
    private Color planeBase = new Color32(0, 255, 255, 15);   

    [Header("Điều khiển Nhịp độ")]
    public float animSpeed = 1.5f; 
    public float observeTime = 5.0f; 

    [Header("Chế độ Test")]
    public bool autoPlayOnStart = true;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        
        if (autoPlayOnStart) {
            yield return new WaitForSeconds(1.0f); 
            // Bắt đầu trình diễn từ gốc tọa độ của Prefab
            StartCoroutine(Intro_Theory_10_4());
        }
    }

    // Hàm trợ lý để neo Object vào Prefab cha
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private IEnumerator Intro_Theory_10_4()
    {
        Vector3 origin = Vector3.zero;
        float yPos = 1.0f; 
        float zPos = 3.5f;

        // 1. TRÌNH DIỄN HÌNH CHÓP (Bên trái)
        yield return StartCoroutine(Build_HinhChop_Full(origin + new Vector3(-3.2f, yPos, zPos)));
        
        yield return new WaitForSeconds(2.0f); 

        // 2. TRÌNH DIỄN HÌNH TỨ DIỆN (Bên phải)
        yield return StartCoroutine(Build_TuDien_Full(origin + new Vector3(3.2f, yPos, zPos)));
    }

    // ==========================================
    // 1. DỰNG HÌNH CHÓP S.ABCD (LOCALIZED)
    // ==========================================
    private IEnumerator Build_HinhChop_Full(Vector3 localCenter)
    {
        TextMeshPro title = CreateDynamicTitle(localCenter + new Vector3(0, 2.5f, 0), "HÌNH CHÓP S.ABCD", whiteCyber);

        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-1.2f, 0, -0.8f)), ptColor, "A", false));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.2f, 0, -0.8f)), ptColor, "B", false));
        GameObject pC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.5f, 0, 0.8f)), ptColor, "C", false));
        GameObject pD = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-0.9f, 0, 0.8f)), ptColor, "D", false));
        GameObject pS = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0, 1.8f, 0)), whiteCyber, "S", true));

        AnimatePoints(new[] { pA, pB, pC, pD }, pS);
        yield return new WaitForSeconds(animSpeed);

        GameObject[] edges = {
            Fix(GeoFactory.CreateLine(pA, pB, edgeCyan)), Fix(GeoFactory.CreateLine(pB, pC, edgeCyan)),
            Fix(GeoFactory.CreateLine(pC, pD, edgeCyan)), Fix(GeoFactory.CreateLine(pD, pA, edgeCyan)),
            Fix(GeoFactory.CreateLine(pS, pA, edgeCyan)), Fix(GeoFactory.CreateLine(pS, pB, edgeCyan)),
            Fix(GeoFactory.CreateLine(pS, pC, edgeCyan)), Fix(GeoFactory.CreateLine(pS, pD, edgeCyan))
        };
        foreach(var e in edges) AnimateLine(e);
        yield return new WaitForSeconds(animSpeed);

        GameObject fBase = Fix(GeoFactory.CreateFace(new[] { pA, pB, pC, pD }, planeBase));
        GameObject fSAB = Fix(GeoFactory.CreateFace(new[] { pS, pA, pB }, planeBase));
        GameObject fSBC = Fix(GeoFactory.CreateFace(new[] { pS, pB, pC }, planeBase));
        GameObject fSCD = Fix(GeoFactory.CreateFace(new[] { pS, pC, pD }, planeBase));
        GameObject fSDA = Fix(GeoFactory.CreateFace(new[] { pS, pD, pA }, planeBase));

        FadePlane(fBase, 0.25f);
        FadePlane(fSAB, 0.15f); FadePlane(fSBC, 0.15f); 
        FadePlane(fSCD, 0.15f); FadePlane(fSDA, 0.15f);

        // Giám sát S dựa trên local Y của nó so với local Y của mặt đáy
        StartCoroutine(MonitorPyramid(pS, localCenter.y, edges, title));
        
        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // 2. DỰNG HÌNH TỨ DIỆN ABCD (LOCALIZED)
    // ==========================================
    private IEnumerator Build_TuDien_Full(Vector3 localCenter)
    {
        CreateDynamicTitle(localCenter + new Vector3(0, 2.5f, 0), "HÌNH TỨ DIỆN ABCD", whiteCyber);

        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0, 1.8f, 0)), whiteCyber, "A", false));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-1.2f, 0, -0.8f)), ptColor, "B", false));
        GameObject pC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.2f, 0, -0.8f)), ptColor, "C", false));
        GameObject pD = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0, 0, 1.2f)), ptColor, "D", false));

        AnimatePoints(new[] { pB, pC, pD }, pA);
        yield return new WaitForSeconds(animSpeed);

        GameObject[] edges = {
            Fix(GeoFactory.CreateLine(pA, pB, edgeCyan)), Fix(GeoFactory.CreateLine(pA, pC, edgeCyan)), Fix(GeoFactory.CreateLine(pA, pD, edgeCyan)),
            Fix(GeoFactory.CreateLine(pB, pC, edgeCyan)), Fix(GeoFactory.CreateLine(pC, pD, edgeCyan)), Fix(GeoFactory.CreateLine(pD, pB, edgeCyan))
        };
        foreach(var e in edges) AnimateLine(e);
        yield return new WaitForSeconds(animSpeed);

        GameObject fABC = Fix(GeoFactory.CreateFace(new[] { pA, pB, pC }, planeBase));
        GameObject fABD = Fix(GeoFactory.CreateFace(new[] { pA, pB, pD }, planeBase));
        GameObject fACD = Fix(GeoFactory.CreateFace(new[] { pA, pC, pD }, planeBase));
        GameObject fBCD = Fix(GeoFactory.CreateFace(new[] { pB, pC, pD }, planeBase));

        FadePlane(fABC, 0.2f); 
        FadePlane(fABD, 0.15f); FadePlane(fACD, 0.15f); FadePlane(fBCD, 0.15f);

        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // CÁC HÀM TIỆN ÍCH
    // ==========================================

    private IEnumerator MonitorPyramid(GameObject S, float localBaseY, GameObject[] edges, TextMeshPro title)
    {
        while (S != null) {
            // Kiểm tra localPosition.y để đảm bảo hoạt động đúng khi di chuyển Prefab
            float height = Mathf.Abs(S.transform.localPosition.y - localBaseY);
            if (height < 0.15f) {
                title.text = "<color=red>LỖI: S không được nằm trên mặt đáy!</color>";
                foreach(var e in edges) ChangeColor(e, edgeError);
            } else {
                title.text = "HÌNH CHÓP S.ABCD";
                foreach(var e in edges) ChangeColor(e, edgeCyan);
            }
            yield return null;
        }
    }

    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color) {
        GameObject anchor = new GameObject("Title_" + text.GetHashCode());
        Fix(anchor);
        anchor.transform.localPosition = localPos;
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = color; tm.fontSize = 1.8f; tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        anchor.AddComponent<Billboard>();
        return tm;
    }

    private void AnimatePoints(GameObject[] pts, GameObject apex) {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.04f, animSpeed).SetEase(Ease.OutBack);
        if(apex != null) apex.transform.DOScale(0.06f, animSpeed).SetEase(Ease.OutBounce).SetDelay(animSpeed/2);
    }

    private void AnimateLine(GameObject line) {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true;
            float len = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(len, animSpeed).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject plane, float alpha) {
        if (plane == null) return;
        Material mat = plane.GetComponent<Renderer>().material;
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(alpha, prop, animSpeed);
    }

    private void ChangeColor(GameObject obj, Color c) {
        if (obj == null) return;
        Material m = obj.GetComponent<Renderer>().material;
        if(m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        else m.color = c;
    }
}