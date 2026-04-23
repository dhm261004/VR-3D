using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B10_M3_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 180);     
    private Color edgePink = new Color32(255, 0, 128, 180);    
    private Color yellowCyber = new Color32(255, 215, 0, 255);  
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    
    private Color planeBase = new Color32(0, 255, 255, 0);      
    private Color planeHighlight = new Color32(0, 255, 255, 40); 

    [Header("Điều khiển Nhịp độ")]
    public float animSpeed = 1.5f; 
    public float observeTime = 4.0f; 

    [Header("Chế độ Test")]
    public bool autoPlayOnStart = true;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        
        if (autoPlayOnStart) {
            yield return new WaitForSeconds(1.5f); 
            StartCoroutine(SpawnModule3Gallery());
        }
    }

    private IEnumerator SpawnModule3Gallery()
    {
        // Sử dụng localPosition của chính Prefab làm gốc
        Vector3 origin = Vector3.zero;

        // Cách 1: Trái
        yield return StartCoroutine(Build_Way1_3Points(origin + new Vector3(-3.5f, 1.0f, 3.0f)));
        yield return new WaitForSeconds(2.0f); 
        
        // Cách 2: Giữa, lùi xa
        yield return StartCoroutine(Build_Way2_PointAndLine(origin + new Vector3(0f, 2.5f, 4.5f)));
        yield return new WaitForSeconds(2.0f);
        
        // Cách 3: Phải
        yield return StartCoroutine(Build_Way3_IntersectingLines(origin + new Vector3(3.5f, 1.0f, 3.0f)));
    }

    // Hàm Fix trợ lý để neo Object vào Prefab
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    // ==========================================
    // CÁCH 1: QUA 3 ĐIỂM KHÔNG THẲNG HÀNG
    // ==========================================
    private IEnumerator Build_Way1_3Points(Vector3 localCenter)
    {
        CreateDynamicTitle(localCenter + new Vector3(0, 1.5f, 0), "CÁCH 1:\nQua 3 điểm không thẳng hàng", whiteCyber);

        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-0.6f, 0, -0.4f)), ptColor, "A", true));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0.6f, 0, -0.6f)), ptColor, "B", true));
        GameObject pC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0.1f, 0, 0.6f)), ptColor, "C", true));

        AnimatePoints(new[] { pA, pB, pC }, null);
        yield return new WaitForSeconds(1f);

        GameObject lineAB = Fix(GeoFactory.CreateLine(pA, pB, edgeCyan, 0.005f));
        GameObject lineBC = Fix(GeoFactory.CreateLine(pB, pC, edgeCyan, 0.005f));
        GameObject lineCA = Fix(GeoFactory.CreateLine(pC, pA, edgeCyan, 0.005f));
        AnimateLine(lineAB); AnimateLine(lineBC); AnimateLine(lineCA);

        yield return new WaitForSeconds(animSpeed);

        GameObject plane = Fix(BuildConceptualPlane(localCenter, 1.2f, 1.0f));
        FadePlane(plane, 0.4f, yellowCyber); 
        
        CreateDynamicTitle(localCenter + new Vector3(1.2f, 0.2f, 0.8f), "(ABC)", yellowCyber, 1.2f);
        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // CÁCH 2: QUA 1 ĐƯỜNG THẲNG & 1 ĐIỂM NẰM NGOÀI
    // ==========================================
    private IEnumerator Build_Way2_PointAndLine(Vector3 localCenter)
    {
        CreateDynamicTitle(localCenter + new Vector3(0, 1.5f, 0), "CÁCH 2:\nQua 1 đường thẳng & 1 điểm nằm ngoài", whiteCyber);

        GameObject p1 = CreateAnchor(localCenter + new Vector3(-1.0f, 0, -0.3f));
        GameObject p2 = CreateAnchor(localCenter + new Vector3(1.0f, 0, 0.1f));
        GameObject lineD = Fix(GeoFactory.CreateLine(p1, p2, edgeCyan));
        
        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-0.2f, 0, 0.6f)), ptColor, "A", true));
        
        AnimateLine(lineD);
        AnimatePoints(new[] { pA }, null);
        CreateDynamicTitle(p2.transform.localPosition + new Vector3(0.2f, 0, 0), "d", edgeCyan, 1.5f);

        yield return new WaitForSeconds(1.5f);

        GameObject plane = Fix(BuildConceptualPlane(localCenter, 1.2f, 1.0f));
        FadePlane(plane, 0.4f, yellowCyber); 
        CreateDynamicTitle(localCenter + new Vector3(1.2f, 0.2f, 0.8f), "(A, d)", yellowCyber, 1.2f);

        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // CÁCH 3: QUA 2 ĐƯỜNG THẲNG CẮT NHAU
    // ==========================================
    private IEnumerator Build_Way3_IntersectingLines(Vector3 localCenter)
    {
        CreateDynamicTitle(localCenter + new Vector3(0, 1.5f, 0), "CÁCH 3:\nQua 2 đường thẳng cắt nhau", whiteCyber);

        GameObject p1 = CreateAnchor(localCenter + new Vector3(-0.8f, 0, -0.6f));
        GameObject p2 = CreateAnchor(localCenter + new Vector3(0.8f, 0, 0.6f));
        GameObject lineA = Fix(GeoFactory.CreateLine(p1, p2, edgeCyan));
        
        GameObject p3 = CreateAnchor(localCenter + new Vector3(-0.6f, 0, 0.6f));
        GameObject p4 = CreateAnchor(localCenter + new Vector3(0.6f, 0, -0.6f));
        GameObject lineB = Fix(GeoFactory.CreateLine(p3, p4, edgePink));

        GameObject pM = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter), whiteCyber, "M", true));

        AnimateLine(lineA); AnimateLine(lineB);
        AnimatePoints(new[] { pM }, null);

        CreateDynamicTitle(p2.transform.localPosition + new Vector3(0.2f, 0, 0), "a", edgeCyan, 1.5f);
        CreateDynamicTitle(p3.transform.localPosition + new Vector3(-0.2f, 0, 0), "b", edgePink, 1.5f);

        yield return new WaitForSeconds(1.5f);

        GameObject plane = Fix(BuildConceptualPlane(localCenter, 1.2f, 1.0f));
        FadePlane(plane, 0.4f, yellowCyber); 
        CreateDynamicTitle(localCenter + new Vector3(1.2f, 0.2f, 0.8f), "(a, b)", yellowCyber, 1.2f);

        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // HÀM TIỆN ÍCH
    // ==========================================
    
    private GameObject BuildConceptualPlane(Vector3 localCenter, float w, float d) {
        GameObject p1 = CreateAnchor(localCenter + new Vector3(-w, 0, -d));
        GameObject p2 = CreateAnchor(localCenter + new Vector3(w, 0, -d));
        GameObject p3 = CreateAnchor(localCenter + new Vector3(w, 0, d));
        GameObject p4 = CreateAnchor(localCenter + new Vector3(-w, 0, d));
        return GeoFactory.CreateFace(new[] { p1, p2, p3, p4 }, planeBase);
    }

    private GameObject CreateAnchor(Vector3 localPos) {
        return Fix(GeoFactory.CreatePoint(transform.TransformPoint(localPos), Color.clear, "", false));
    }

    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color, float size = 2.0f)
    {
        GameObject anchor = new GameObject("Title_" + text.GetHashCode());
        Fix(anchor);
        anchor.transform.localPosition = localPos;
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = color; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        anchor.AddComponent<Billboard>();
        return tm;
    }

    private void AnimatePoints(GameObject[] pts, GameObject midPoint)
    {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.05f, animSpeed).SetEase(Ease.OutBack);
        if(midPoint != null) midPoint.transform.DOScale(0.07f, animSpeed).SetEase(Ease.OutBounce).SetDelay(animSpeed / 2f);
    }

    private void AnimateLine(GameObject line)
    {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true; 
            float targetLength = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(targetLength, animSpeed).SetEase(Ease.OutQuad).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject face, float targetAlpha, Color targetColor)
    {
        if (face == null) return;
        Material mat = face.GetComponent<Renderer>().material;
        mat.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0); 
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(targetAlpha, prop, animSpeed);
    }
}