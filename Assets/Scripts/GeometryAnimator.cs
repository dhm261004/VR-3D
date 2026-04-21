using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class GeometryAnimator : MonoBehaviour
{
    IEnumerator Start()
    {
        // Nền Void Black nhám
        if(Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        // BẢNG MÀU NEO-BRUTALISM CYBER
        Color ptColor = new Color32(255, 0, 85, 255);      // Neon Pink (Các đỉnh)
        Color edgeColor = new Color32(0, 255, 255, 180);   // Cyan (Cạnh chính)
        Color highlightColor = new Color32(125, 0, 255, 255); // Deep Purple (Mặt phẳng thiết diện)

        // Các đỉnh nảy lên (Bật cho phép kéo thả)
        GameObject A = GeoFactory.CreatePoint(new Vector3(0, 1.5f, 0), ptColor, "A", true);
        A.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.3f);

        GameObject B = GeoFactory.CreatePoint(new Vector3(-1, 0, 0.8f), ptColor, "B", true);
        GameObject C = GeoFactory.CreatePoint(new Vector3(1, 0, 0.8f), ptColor, "C", true);
        GameObject D = GeoFactory.CreatePoint(new Vector3(0, 0, -1), ptColor, "D", true);
        
        B.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        C.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        D.transform.DOScale(0.05f, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);

        // Dựng Cạnh chính
        AnimateLine(GeoFactory.CreateLine(A, B, edgeColor));
        AnimateLine(GeoFactory.CreateLine(A, C, edgeColor));
        AnimateLine(GeoFactory.CreateLine(A, D, edgeColor));
        AnimateLine(GeoFactory.CreateLine(B, C, edgeColor));
        AnimateLine(GeoFactory.CreateLine(C, D, edgeColor));
        AnimateLine(GeoFactory.CreateLine(D, B, edgeColor));
        yield return new WaitForSeconds(0.8f);

        // Dựng thiết diện
        GameObject M = CreateLogicPoint(A, B, 0.5f, "M", highlightColor);
        GameObject N = CreateLogicPoint(A, C, 0.5f, "N", highlightColor);
        GameObject P = CreateLogicPoint(A, D, 0.5f, "P", highlightColor);
        yield return new WaitForSeconds(0.5f);

        // Mặt phẳng hiện ra (Màu Tím Cyber)
        GameObject face = GeoFactory.CreateFace(new GameObject[] { M, N, P }, highlightColor);
        Material faceMat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (faceMat.HasProperty("_BaseColor")) faceMat.DOFade(0.4f, "_BaseColor", 1f);
        else faceMat.DOFade(0.4f, "_Color", 1f);

        AnimateLine(GeoFactory.CreateLine(M, N, highlightColor, 0.015f));
        AnimateLine(GeoFactory.CreateLine(N, P, highlightColor, 0.015f));
        AnimateLine(GeoFactory.CreateLine(P, M, highlightColor, 0.015f));
        
        // Không tạo sẵn thước đo nữa, người dùng sẽ tự click chuột phải để đo
    }

    GameObject CreateLogicPoint(GameObject p1, GameObject p2, float r, string n, Color c) {
        GameObject p = GeoFactory.CreatePoint(Vector3.Lerp(p1.transform.position, p2.transform.position, r), c, n, false);
        p.transform.DOScale(0.04f, 0.5f).SetEase(Ease.OutBack);
        var f = p.AddComponent<MidpointFollower>();
        f.p1 = p1.transform; f.p2 = p2.transform; f.ratio = r;
        return p;
    }

    void AnimateLine(GameObject line) {
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        float targetLength = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
        
        line.transform.DOScaleY(targetLength, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => {
            ef.isAnimating = false; 
        });
    }
}