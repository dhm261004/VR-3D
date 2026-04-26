using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B11_M2_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 180);     
    private Color edgeGreen = new Color32(0, 255, 128, 180);    
    private Color yellowCyber = new Color32(255, 215, 0, 255);  
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    private Color scanColor = new Color32(255, 255, 255, 30);

    [Header("Điều khiển Nhịp độ")]
    public float animSpeed = 1.5f; 
    public float observeTime = 4.0f; 
    public bool autoPlayOnStart = true;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        if (autoPlayOnStart) {
            yield return new WaitForSeconds(1.0f); 
            StartCoroutine(SpawnModule2Gallery());
        }
    }

    private IEnumerator SpawnModule2Gallery()
    {
        
        Vector3 origin = transform.position;
        float zPos = 0f; float yPos = 0f; 

        
        yield return StartCoroutine(Build_Uniqueness_Final(origin + new Vector3(-3.0f, yPos, zPos)));
        
        yield return new WaitForSeconds(2.0f); 

        
        yield return StartCoroutine(Build_Transitivity_Book(origin + new Vector3(3.0f, yPos, zPos)));
    }

    
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    
    
    
    private IEnumerator Build_Uniqueness_Final(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "TÍNH DUY NHẤT", whiteCyber);

        GameObject pA = Fix(GeoFactory.CreatePoint(center + new Vector3(-1.2f, -0.5f, 0), ptColor, "A", true));
        GameObject pB = Fix(GeoFactory.CreatePoint(center + new Vector3(1.2f, -0.5f, 0), ptColor, "B", true));
        GameObject lineD = Fix(GeoFactory.CreateLine(pA, pB, edgeCyan));
        GameObject pM = Fix(GeoFactory.CreatePoint(center + new Vector3(0, 0.6f, 0), whiteCyber, "M", true));

        AnimatePoints(new[] { pA, pB }, pM);
        yield return new WaitForSeconds(animSpeed);
        AnimateLine(lineD);

        yield return new WaitForSeconds(1f);

        
        GameObject pS1 = new GameObject("Scan1"); 
        GameObject pS2 = new GameObject("Scan2");
        pS1.transform.SetParent(pM.transform); 
        pS2.transform.SetParent(pM.transform);
        pS1.transform.localPosition = new Vector3(-1.2f, 0.5f, 0.1f);
        pS2.transform.localPosition = new Vector3(1.2f, -0.5f, -0.1f);
        
        GameObject scanLine = Fix(GeoFactory.CreateLine(pS1, pS2, scanColor, 0.003f));
        AnimateLine(scanLine);

        pM.transform.DORotate(new Vector3(0, 0, 720f), 2.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(2.6f);

        
        pM.transform.DORotate(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.6f);

        GameObject pM1 = Fix(GeoFactory.CreatePoint(center + new Vector3(-1.2f, 0.6f, 0), ptColor, "m1", true));
        GameObject pM2 = Fix(GeoFactory.CreatePoint(center + new Vector3(1.2f, 0.6f, 0), ptColor, "m2", true));
        GameObject lineM = Fix(GeoFactory.CreateLine(pM1, pM2, yellowCyber, 0.02f));
        
        pM1.transform.DOScale(0.04f, 0.5f); 
        pM2.transform.DOScale(0.04f, 0.5f);
        AnimateLine(lineM);
        
        lineM.transform.DOPunchScale(Vector3.one * 0.05f, 0.5f);
        Destroy(scanLine);

        CreateTitle(pM.transform.position + Vector3.up * 0.4f, "m // d", yellowCyber);
        yield return new WaitForSeconds(observeTime);
    }

    
    
    
    private IEnumerator Build_Transitivity_Book(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "TÍNH BẮC CẦU", whiteCyber);

        GameObject pC1 = Fix(GeoFactory.CreatePoint(center + new Vector3(-1.2f, 0, 0), whiteCyber, "c1", true));
        GameObject pC2 = Fix(GeoFactory.CreatePoint(center + new Vector3(1.2f, 0, 0), whiteCyber, "c2", true));
        GameObject lineC = Fix(GeoFactory.CreateLine(pC1, pC2, whiteCyber));
        
        AnimatePoints(new[] { pC1, pC2 }, null);
        AnimateLine(lineC);
        yield return new WaitForSeconds(animSpeed);

        
        GameObject pA1 = Fix(GeoFactory.CreatePoint(center + new Vector3(-1.2f, 0.8f, 0.5f), ptColor, "a1", true));
        GameObject pA2 = Fix(GeoFactory.CreatePoint(center + new Vector3(1.2f, 0.8f, 0.5f), ptColor, "a2", true));
        GameObject lineA = Fix(GeoFactory.CreateLine(pA1, pA2, edgeCyan));
        GameObject fAlpha = Fix(GeoFactory.CreateFace(new[] { pC1, pC2, pA2, pA1 }, new Color32(0, 255, 255, 30)));

        AnimatePoints(new[] { pA1, pA2 }, null);
        AnimateLine(lineA);
        FadePlane(fAlpha, 0.2f);
        yield return new WaitForSeconds(animSpeed);

        
        GameObject pB1 = Fix(GeoFactory.CreatePoint(center + new Vector3(-1.2f, -0.6f, 0.8f), ptColor, "b1", true));
        GameObject pB2 = Fix(GeoFactory.CreatePoint(center + new Vector3(1.2f, -0.6f, 0.8f), ptColor, "b2", true));
        GameObject lineB = Fix(GeoFactory.CreateLine(pB1, pB2, edgeGreen));
        GameObject fBeta = Fix(GeoFactory.CreateFace(new[] { pC1, pC2, pB2, pB1 }, new Color32(0, 255, 128, 30)));

        AnimatePoints(new[] { pB1, pB2 }, null);
        AnimateLine(lineB);
        FadePlane(fBeta, 0.2f);
        yield return new WaitForSeconds(2f);

        
        if(lineA != null) lineA.GetComponent<Renderer>().material.DOColor(yellowCyber, 1f);
        if(lineB != null) lineB.GetComponent<Renderer>().material.DOColor(yellowCyber, 1f);
        if(lineC != null) lineC.GetComponent<Renderer>().material.DOFade(0.2f, 1f);
        
        CreateTitle(center + Vector3.down * 0.5f, "a // b", yellowCyber);

        yield return new WaitForSeconds(observeTime);
    }

    
    private void CreateTitle(Vector3 pos, string text, Color color) {
        GameObject anchor = new GameObject("Title_" + text.GetHashCode());
        anchor.transform.position = pos;
        Fix(anchor); 
        GeoFactory.CreateLabel(anchor.transform, text, color);
    }

    private void AnimatePoints(GameObject[] pts, GameObject mid) {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.04f, animSpeed).SetEase(Ease.OutBack);
        if(mid != null) mid.transform.DOScale(0.06f, animSpeed).SetEase(Ease.OutBounce).SetDelay(animSpeed/2);
    }

    private void AnimateLine(GameObject line) {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true;
            float len = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(len, animSpeed).SetEase(Ease.OutQuad).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject plane, float alpha) {
        if (plane == null) return;
        Renderer rend = plane.GetComponent<Renderer>();
        if (rend != null) {
            Material mat = rend.material;
            mat.DOFade(alpha, mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color", animSpeed);
        }
    }
}