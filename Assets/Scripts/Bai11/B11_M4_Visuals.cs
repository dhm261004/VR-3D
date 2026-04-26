using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B11_M4_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 100);     
    private Color edgeActive = new Color32(0, 255, 128, 255);   
    private Color yellowCyber = new Color32(255, 215, 0, 255);  
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    
    private Color planeSAB = new Color32(0, 255, 255, 35);      
    private Color planeSCD = new Color32(255, 0, 85, 35);       

    [Header("Cấu hình Nhịp độ")]
    public float stepSpeed = 2.0f; 
    public float waitBetweenSteps = 4.5f; 

    private GameObject _moduleRoot; 
    private TextMeshPro _instructionText;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        yield return new WaitForSeconds(1.0f); 

        
        StartCoroutine(Execute_Example4_CorrectPoles(transform.position));
    }

    
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private IEnumerator Execute_Example4_CorrectPoles(Vector3 center)
    {
        
        _moduleRoot = new GameObject("Module_11_4_Container");
        _moduleRoot.transform.SetParent(this.transform);
        _moduleRoot.transform.position = center;

        
        CreateDynamicTitle(new Vector3(0f, 3.8f, 0f), "VÍ DỤ 4: GIAO TUYẾN SONG SONG", whiteCyber, 1.5f);
        _instructionText = CreateDynamicTitle(new Vector3(0f, 3.1f, 0f), "Bước 1: Cho hình chóp S.ABCD (Đáy hình bình hành)", whiteCyber, 1.1f);

        
        float w = 1.2f; float d = 0.8f; float offset = 0.4f;
        GameObject pA = CreateAndParent(new Vector3(-w - offset, 0, -d), ptColor, "A", false);
        GameObject pB = CreateAndParent(new Vector3(w - offset, 0, -d), ptColor, "B", false);
        GameObject pC = CreateAndParent(new Vector3(w + offset, 0, d), ptColor, "C", false);
        GameObject pD = CreateAndParent(new Vector3(-w + offset, 0, d), ptColor, "D", false);
        GameObject pS = CreateAndParent(new Vector3(0f, 2.0f, 0f), whiteCyber, "S", true);

        AnimatePoints(new[] { pA, pB, pC, pD }, pS);
        yield return new WaitForSeconds(stepSpeed);

        GameObject[] edges = {
            CreateLineAndParent(pA, pB, edgeCyan), CreateLineAndParent(pB, pC, edgeCyan),
            CreateLineAndParent(pC, pD, edgeCyan), CreateLineAndParent(pD, pA, edgeCyan),
            CreateLineAndParent(pS, pA, edgeCyan), CreateLineAndParent(pS, pB, edgeCyan),
            CreateLineAndParent(pS, pC, edgeCyan), CreateLineAndParent(pS, pD, edgeCyan)
        };
        foreach(var e in edges) AnimateLine(e, stepSpeed);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 2: S là điểm chung. Có AB // CD";
        edges[0].GetComponent<Renderer>().material.DOColor(edgeActive, "_BaseColor", stepSpeed);
        edges[2].GetComponent<Renderer>().material.DOColor(edgeActive, "_BaseColor", stepSpeed);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 3: Dựng giao tuyến m qua S và song song AB, CD";
        
        GameObject mAnchor = new GameObject("m_Stabilizer");
        mAnchor.transform.SetParent(_moduleRoot.transform);
        mAnchor.transform.localPosition = new Vector3(0f, 2.0f, 0f); 

        Vector3 dirAB = (pB.transform.position - pA.transform.position).normalized;
        GameObject pM1 = new GameObject("m_ext_L"); 
        GameObject pM2 = new GameObject("m_ext_R");
        pM1.transform.SetParent(mAnchor.transform); 
        pM2.transform.SetParent(mAnchor.transform);
        pM1.transform.localPosition = -dirAB * 5.0f;
        pM2.transform.localPosition = dirAB * 5.0f;

        GameObject lineM = Fix(GeoFactory.CreateLine(pM1, pM2, yellowCyber, 0.025f));
        lineM.transform.SetParent(mAnchor.transform);
        AnimateLine(lineM, stepSpeed);
        GeoFactory.CreateLabel(pM2.transform, "m", yellowCyber);
        yield return new WaitForSeconds(waitBetweenSteps);

        
        _instructionText.text = "Bước 4: Mở rộng (SAB) và (SCD) hội tụ tại m";
        
        GameObject fSAB = Fix(GeoFactory.CreateFace(new[] { pA, pB, pM2, pM1 }, planeSAB));
        GameObject fSCD = Fix(GeoFactory.CreateFace(new[] { pD, pC, pM2, pM1 }, planeSCD));
        
        FadePlane(fSAB, 0.25f); FadePlane(fSCD, 0.25f);

        yield return new WaitForSeconds(1.0f);
        _instructionText.text = "<color=#00FF00>KẾT LUẬN:</color> Giao tuyến m thuộc cả hai mặt phẳng";
        _instructionText.color = yellowCyber;
    }

    
    private GameObject CreateAndParent(Vector3 localPos, Color c, string n, bool drag) {
        
        Vector3 worldPos = _moduleRoot.transform.TransformPoint(localPos);
        GameObject p = GeoFactory.CreatePoint(worldPos, c, n, drag);
        p.transform.SetParent(_moduleRoot.transform);
        return p;
    }

    private GameObject CreateLineAndParent(GameObject p1, GameObject p2, Color c) {
        GameObject l = Fix(GeoFactory.CreateLine(p1, p2, c));
        l.transform.SetParent(_moduleRoot.transform);
        return l;
    }

    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color, float size) {
        GameObject anchor = new GameObject("UI_Label");
        anchor.transform.SetParent(_moduleRoot.transform);
        anchor.transform.localPosition = localPos;
        
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = color; tm.fontSize = size;
        tm.fontStyle = FontStyles.Bold; tm.alignment = TextAlignmentOptions.Center;
        anchor.AddComponent<Billboard>(); 
        return tm;
    }

    private void AnimatePoints(GameObject[] pts, GameObject apex) {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.04f, stepSpeed).SetEase(Ease.OutBack);
        if(apex != null) apex.transform.DOScale(0.06f, stepSpeed).SetEase(Ease.OutBounce).SetDelay(stepSpeed/2);
    }

    private void AnimateLine(GameObject line, float speed) {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if(ef == null || ef.p1 == null || ef.p2 == null) return;
        ef.isAnimating = true;
        float len = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
        line.transform.DOScaleY(len, speed).OnComplete(() => { if(ef != null) ef.isAnimating = false; });
    }

    private void FadePlane(GameObject plane, float alpha) {
        if (plane == null) return;
        Renderer rend = plane.GetComponent<Renderer>();
        if (rend == null) return;
        Material mat = rend.material;
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(alpha, prop, 1.5f);
    }
}