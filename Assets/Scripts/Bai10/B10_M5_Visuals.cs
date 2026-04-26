using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B10_M5_Visuals : MonoBehaviour
{
    [Header("Bảng màu Cyber Lab")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    private Color edgeCyan = new Color32(0, 255, 255, 120);     
    private Color edgePink = new Color32(255, 0, 128, 255);    
    private Color edgeYellow = new Color32(255, 215, 0, 255);   
    
    private Color planeBase = new Color32(0, 255, 255, 15);   
    private Color planeAux = new Color32(255, 215, 0, 40);    

    [Header("Cấu hình Nhịp độ (Chậm)")]
    public float stepSpeed = 2.0f; 
    public float waitBetweenSteps = 4.5f;

    private TextMeshPro _problemText;
    private TextMeshPro _instructionText;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        yield return new WaitForSeconds(1.0f);
        
        StartCoroutine(Solve_Example6_FullColor(Vector3.zero));
    }

    
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private IEnumerator Solve_Example6_FullColor(Vector3 localCenter)
    {
        
        _problemText = CreateDynamicTitle(localCenter + new Vector3(-2.8f, 2.5f, 0), 
            "<color=#00FFFF>VÍ DỤ 6:</color>\nCho tứ diện ABCD.\n- E nằm trong tam giác BCD.\n- F nằm giữa A và E.\n<color=#FFD700>Tìm N = BF \u2229 (ACD)</color>", 
            whiteCyber, 1.1f, TextAlignmentOptions.Left);

        
        _instructionText = CreateDynamicTitle(localCenter + new Vector3(0, 2.8f, 0), "Bước 1: Dựng hình tứ diện đặc ABCD", whiteCyber);
        
        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0, 1.8f, 0)), whiteCyber, "A", false));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-1.2f, 0, -0.8f)), ptColor, "B", false));
        GameObject pC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0.5f, 0, -1.2f)), ptColor, "C", false));
        GameObject pD = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.2f, 0, 0.8f)), ptColor, "D", false));

        pA.transform.DOScale(0.05f, stepSpeed); pB.transform.DOScale(0.05f, stepSpeed); 
        pC.transform.DOScale(0.05f, stepSpeed); pD.transform.DOScale(0.05f, stepSpeed);
        yield return new WaitForSeconds(stepSpeed);

        GameObject[] frame = {
            Fix(GeoFactory.CreateLine(pA, pB, edgeCyan)), Fix(GeoFactory.CreateLine(pA, pC, edgeCyan)), Fix(GeoFactory.CreateLine(pA, pD, edgeCyan)),
            Fix(GeoFactory.CreateLine(pB, pC, edgeCyan)), Fix(GeoFactory.CreateLine(pC, pD, edgeCyan)), Fix(GeoFactory.CreateLine(pD, pB, edgeCyan))
        };
        foreach(var e in frame) AnimateLine(e, stepSpeed);

        FadePlane(Fix(GeoFactory.CreateFace(new[] { pA, pB, pC }, planeBase)), 0.12f);
        FadePlane(Fix(GeoFactory.CreateFace(new[] { pA, pB, pD }, planeBase)), 0.12f);
        FadePlane(Fix(GeoFactory.CreateFace(new[] { pA, pC, pD }, planeBase)), 0.25f); 
        FadePlane(Fix(GeoFactory.CreateFace(new[] { pB, pC, pD }, planeBase)), 0.12f);

        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 2: Lấy E trong mp(BCD). Kẻ BE cắt CD tại M";
        Vector3 localE = localCenter + new Vector3(0.2f, 0, -0.3f); 
        GameObject pE = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localE), whiteCyber, "E", false));
        pE.transform.DOScale(0.04f, stepSpeed);
        
        Vector3 dirBE = (pE.transform.position - pB.transform.position).normalized;
        Vector3 worldM = RayLineIntersection(pB.transform.position, dirBE, pC.transform.position, pD.transform.position);
        GameObject pM = Fix(GeoFactory.CreatePoint(worldM, edgeYellow, "M", false));
        pM.transform.DOScale(0.04f, stepSpeed);

        AnimateLine(Fix(GeoFactory.CreateLine(pB, pM, edgePink, 0.005f)), stepSpeed);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 3: Lấy F nằm giữa A và E (F thuộc AE)";
        AnimateLine(Fix(GeoFactory.CreateLine(pA, pE, edgePink, 0.005f)), stepSpeed);
        Vector3 worldF = Vector3.Lerp(pA.transform.position, pE.transform.position, 0.5f);
        GameObject pF = Fix(GeoFactory.CreatePoint(worldF, whiteCyber, "F", false));
        pF.transform.DOScale(0.04f, stepSpeed);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 4: Xét mp phụ (ABM) chứa đường thẳng BF";
        _instructionText.color = edgeYellow;
        GameObject faceABM = Fix(GeoFactory.CreateFace(new[] { pA, pB, pM }, planeAux));
        FadePlane(faceABM, 0.45f); 
        
        GameObject lineAM = Fix(GeoFactory.CreateLine(pA, pM, edgeYellow, 0.012f));
        AnimateLine(lineAM, stepSpeed);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 5: Trong mp(ABM), BF cắt AM tại N";
        Vector3 worldN = GetLineLineIntersection(pB.transform.position, pF.transform.position, pA.transform.position, pM.transform.position);
        GameObject pN = Fix(GeoFactory.CreatePoint(worldN, edgeYellow, "N", false));
        pN.transform.DOScale(0.08f, stepSpeed).SetEase(Ease.OutBounce);
        
        AnimateLine(Fix(GeoFactory.CreateLine(pB, pN, edgePink, 0.015f)), stepSpeed); 
        yield return new WaitForSeconds(1.0f);

        
        _instructionText.text = "<color=#FFD700>KẾT LUẬN: N chính là giao điểm của BF và (ACD)</color>";
        pN.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f).SetLoops(4);
    }

    
    private Vector3 GetLineLineIntersection(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2) {
        Vector3 da = a2 - a1; Vector3 db = b2 - b1; Vector3 dc = b1 - a1;
        float s = Vector3.Dot(Vector3.Cross(dc, db), Vector3.Cross(da, db)) / Vector3.Cross(da, db).sqrMagnitude;
        return a1 + da * s;
    }

    private Vector3 RayLineIntersection(Vector3 origin, Vector3 dir, Vector3 seg1, Vector3 seg2) {
        Vector3 segDir = (seg2 - seg1).normalized;
        Vector3 cross = Vector3.Cross(dir, segDir);
        float t = Vector3.Dot(Vector3.Cross(seg1 - origin, segDir), cross) / cross.sqrMagnitude;
        return origin + dir * t;
    }

    
    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color, float size = 1.8f, TextAlignmentOptions align = TextAlignmentOptions.Center) {
        GameObject anchor = new GameObject("Label_" + text.GetHashCode());
        Fix(anchor);
        anchor.transform.localPosition = localPos;
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = color; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold; tm.alignment = align;
        anchor.AddComponent<Billboard>();
        return tm;
    }

    private void AnimateLine(GameObject line, float speed) {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true;
            float len = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(len, speed).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject plane, float alpha) {
        if (plane == null) return;
        Material mat = plane.GetComponent<Renderer>().material;
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(alpha, prop, 1.5f);
    }
}