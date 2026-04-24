using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class Module1 : MonoBehaviour
{
    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color highlightColor = new Color32(255, 255, 0, 255);

        // Mặt phẳng Alpha (Y = 0.6) - tâm Z = 0 (ngang với spawn root)
        GameObject a1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f,  1.5f)), alphaColor, "(P)", false));
        GameObject a4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f,  1.5f)), alphaColor, " ", false));
        
        a1.transform.DOScale(0.04f, 0.5f); a2.transform.DOScale(0.04f, 0.5f);
        a3.transform.DOScale(0.04f, 0.5f); a4.transform.DOScale(0.04f, 0.5f);
        
        GameObject faceAlpha = Fix(GeoFactory.CreateFace(new GameObject[] { a1, a2, a3, a4 }, alphaColor));
        Material matAlpha = faceAlpha.GetComponent<MeshRenderer>().sharedMaterial;
        if (matAlpha.HasProperty("_BaseColor")) matAlpha.DOFade(0.4f, "_BaseColor", 1f);
        else matAlpha.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.5f);

        // Mặt phẳng Beta (Y = -0.4) - song song bên dưới
        GameObject b1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f,  1.5f)), betaColor, "(Q)", false));
        GameObject b4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f,  1.5f)), betaColor, " ", false));
        
        b1.transform.DOScale(0.04f, 0.5f); b2.transform.DOScale(0.04f, 0.5f);
        b3.transform.DOScale(0.04f, 0.5f); b4.transform.DOScale(0.04f, 0.5f);
        
        GameObject faceBeta = Fix(GeoFactory.CreateFace(new GameObject[] { b1, b2, b3, b4 }, betaColor));
        Material matBeta = faceBeta.GetComponent<MeshRenderer>().sharedMaterial;
        if (matBeta.HasProperty("_BaseColor")) matBeta.DOFade(0.4f, "_BaseColor", 1f);
        else matBeta.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.8f);

        // Giao điểm I ở giữa mặt Alpha - tâm Z = 0
        GameObject intersectionPoint = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(0, 0.6f, 0f)), highlightColor, "I", false));
        intersectionPoint.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.3f);

        // Đường thẳng a
        GameObject a_end1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-1.5f, 0.6f, -1.0f)), highlightColor, " ", false));
        GameObject a_end2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 1.5f, 0.6f,  1.0f)), highlightColor, "a", false));
        a_end1.transform.DOScale(0.04f, 0.5f);
        a_end2.transform.DOScale(0.04f, 0.5f);

        // Đường thẳng b
        GameObject b_end1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-1.5f, 0.6f,  1.0f)), highlightColor, " ", false));
        GameObject b_end2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 1.5f, 0.6f, -1.0f)), highlightColor, "b", false));
        b_end1.transform.DOScale(0.04f, 0.5f);
        b_end2.transform.DOScale(0.04f, 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        GameObject lineA = Fix(GeoFactory.CreateLine(a_end1, a_end2, highlightColor, 0.02f));
        GameObject lineB = Fix(GeoFactory.CreateLine(b_end1, b_end2, highlightColor, 0.02f));
        
        AnimateLine(lineA);
        AnimateLine(lineB);
    }

    void AnimateLine(GameObject line) {
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        float targetLength = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
        
        line.transform.DOScaleY(targetLength, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => {
            ef.isAnimating = false; 
        });
    }

    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }
}
