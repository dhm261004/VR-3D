using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B10_M2_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 180);     
    private Color edgePink = new Color32(255, 0, 128, 180);    
    private Color yellowCyber = new Color32(255, 215, 0, 255);  
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    
    private Color planePColor = new Color32(0, 255, 255, 25);   
    private Color planeQColor = new Color32(255, 0, 128, 25);   

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
            
            StartCoroutine(Build_Module2_Intersection(Vector3.zero));
        }
    }

    
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private IEnumerator Build_Module2_Intersection(Vector3 localCenter)
    {
        
        TextMeshPro title = CreateDynamicTitle(localCenter + new Vector3(0, 2.2f, 0), "GIAO TUYẾN CỦA HAI MẶT PHẲNG", whiteCyber);

        
        GameObject p1P = CreateAnchor(localCenter + new Vector3(-2f, 0, -1.5f));
        GameObject p2P = CreateAnchor(localCenter + new Vector3(2f, 0, -1.5f));
        GameObject p3P = CreateAnchor(localCenter + new Vector3(2.5f, 0, 1.5f));
        GameObject p4P = CreateAnchor(localCenter + new Vector3(-1.5f, 0, 1.5f));

        GameObject faceP = Fix(GeoFactory.CreateFace(new[] { p1P, p2P, p3P, p4P }, planePColor));
        FadePlane(faceP, 0.2f);
        CreateDynamicTitle(p3P.transform.localPosition + new Vector3(0.3f, 0, 0), "(P)", edgeCyan, 1.5f);

        yield return new WaitForSeconds(animSpeed);

        
        GameObject p1Q = CreateAnchor(localCenter + new Vector3(-2f, 1f, 1f));
        GameObject p2Q = CreateAnchor(localCenter + new Vector3(2f, 1f, 1f));
        GameObject p3Q = CreateAnchor(localCenter + new Vector3(2f, -1f, -1f));
        GameObject p4Q = CreateAnchor(localCenter + new Vector3(-2f, -1f, -1f));

        GameObject faceQ = Fix(GeoFactory.CreateFace(new[] { p1Q, p2Q, p3Q, p4Q }, planeQColor));
        FadePlane(faceQ, 0.2f);
        CreateDynamicTitle(p1Q.transform.localPosition + new Vector3(-0.3f, 0.2f, 0), "(Q)", edgePink, 1.5f);

        yield return new WaitForSeconds(animSpeed + observeTime - 2f);

        
        title.text = "Hai mặt phẳng có điểm chung\nthì có một GIAO TUYẾN duy nhất";

        
        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-1.2f, 0, 0)), ptColor, "A", true));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.2f, 0, 0)), ptColor, "B", true));
        
        AnimatePoints(new[] { pA, pB });
        yield return new WaitForSeconds(animSpeed);

        
        GameObject pD1 = CreateAnchor(localCenter + new Vector3(-3f, 0, 0));
        GameObject pD2 = CreateAnchor(localCenter + new Vector3(3f, 0, 0));
        GameObject lineD = Fix(GeoFactory.CreateLine(pD1, pD2, yellowCyber, 0.02f));
        
        CreateDynamicTitle(pD2.transform.localPosition + new Vector3(0.3f, 0, 0), "d", yellowCyber, 1.5f);
        AnimateLine(lineD);

        
        lineD.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f);
        yield return new WaitForSeconds(observeTime);
    }

    
    private GameObject CreateAnchor(Vector3 localPos) 
    {
        
        Vector3 worldPos = transform.TransformPoint(localPos);
        return Fix(GeoFactory.CreatePoint(worldPos, Color.clear, "", false));
    }

    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color, float size = 2.0f)
    {
        GameObject anchor = new GameObject("Title_" + text.GetHashCode());
        Fix(anchor);
        anchor.transform.localPosition = localPos;
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; tm.color = color; tm.fontSize = size;
        tm.alignment = TextAlignmentOptions.Center;
        anchor.AddComponent<Billboard>();
        return tm;
    }

    private void AnimatePoints(GameObject[] pts)
    {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.06f, animSpeed).SetEase(Ease.OutBack);
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

    private void FadePlane(GameObject face, float targetAlpha)
    {
        if (face == null) return;
        Material mat = face.GetComponent<Renderer>().material;
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(targetAlpha, prop, animSpeed);
    }
}