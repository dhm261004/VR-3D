using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class Module1 : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1. Nền Void Black nhám
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        // BẢNG MÀU
        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color highlightColor = new Color32(255, 255, 0, 255); // Vàng nổi bật

        // 2. Dựng Mặt phẳng Alpha (Y = 0.6) lơ lửng cách người dùng 3m
        GameObject a1 = GeoFactory.CreatePoint(new Vector3(-2, 0.6f, 1.5f), alphaColor, " ", false);
        GameObject a2 = GeoFactory.CreatePoint(new Vector3( 2, 0.6f, 1.5f), alphaColor, " ", false);
        GameObject a3 = GeoFactory.CreatePoint(new Vector3( 2, 0.6f, 4.5f), alphaColor, "α", false);
        GameObject a4 = GeoFactory.CreatePoint(new Vector3(-2, 0.6f, 4.5f), alphaColor, " ", false);
        
        a1.transform.DOScale(0.04f, 0.5f); a2.transform.DOScale(0.04f, 0.5f);
        a3.transform.DOScale(0.04f, 0.5f); a4.transform.DOScale(0.04f, 0.5f);
        
        GameObject faceAlpha = GeoFactory.CreateFace(new GameObject[] { a1, a2, a3, a4 }, alphaColor);
        Material matAlpha = faceAlpha.GetComponent<MeshRenderer>().sharedMaterial;
        if (matAlpha.HasProperty("_BaseColor")) matAlpha.DOFade(0.4f, "_BaseColor", 1f);
        else matAlpha.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.5f);

        // 3. Dựng Mặt phẳng Beta (Y = -0.4) lơ lửng song song bên dưới
        GameObject b1 = GeoFactory.CreatePoint(new Vector3(-2, -0.4f, 1.5f), betaColor, " ", false);
        GameObject b2 = GeoFactory.CreatePoint(new Vector3( 2, -0.4f, 1.5f), betaColor, " ", false);
        GameObject b3 = GeoFactory.CreatePoint(new Vector3( 2, -0.4f, 4.5f), betaColor, "β", false);
        GameObject b4 = GeoFactory.CreatePoint(new Vector3(-2, -0.4f, 4.5f), betaColor, " ", false);
        
        b1.transform.DOScale(0.04f, 0.5f); b2.transform.DOScale(0.04f, 0.5f);
        b3.transform.DOScale(0.04f, 0.5f); b4.transform.DOScale(0.04f, 0.5f);
        
        GameObject faceBeta = GeoFactory.CreateFace(new GameObject[] { b1, b2, b3, b4 }, betaColor);
        Material matBeta = faceBeta.GetComponent<MeshRenderer>().sharedMaterial;
        if (matBeta.HasProperty("_BaseColor")) matBeta.DOFade(0.4f, "_BaseColor", 1f);
        else matBeta.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.8f);

        // 4. Dựng hai đường thẳng a và b cắt nhau trên mặt phẳng Alpha (Y = 0.6)
        // Tạo giao điểm I ở giữa mặt Alpha
        GameObject intersectionPoint = GeoFactory.CreatePoint(new Vector3(0, 0.6f, 3.0f), highlightColor, "I", false);
        intersectionPoint.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.3f);

        // Tạo các đầu mút của đường thẳng a (chéo trên mặt phẳng)
        GameObject a_end1 = GeoFactory.CreatePoint(new Vector3(-1.5f, 0.6f, 2.0f), highlightColor, " ", false);
        GameObject a_end2 = GeoFactory.CreatePoint(new Vector3( 1.5f, 0.6f, 4.0f), highlightColor, "a", false);
        a_end1.transform.DOScale(0.04f, 0.5f);
        a_end2.transform.DOScale(0.04f, 0.5f);

        // Tạo các đầu mút của đường thẳng b (chéo hướng kia)
        GameObject b_end1 = GeoFactory.CreatePoint(new Vector3(-1.5f, 0.6f, 4.0f), highlightColor, " ", false);
        GameObject b_end2 = GeoFactory.CreatePoint(new Vector3( 1.5f, 0.6f, 2.0f), highlightColor, "b", false);
        b_end1.transform.DOScale(0.04f, 0.5f);
        b_end2.transform.DOScale(0.04f, 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        // Vẽ và tạo hoạt ảnh chạy đường thẳng a và b
        GameObject lineA = GeoFactory.CreateLine(a_end1, a_end2, highlightColor, 0.02f);
        GameObject lineB = GeoFactory.CreateLine(b_end1, b_end2, highlightColor, 0.02f);
        
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
}
