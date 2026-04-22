using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module2 : MonoBehaviour
{
    private float gammaYOffset = 2.5f; // Bắt đầu ở trên cao

    IEnumerator Start()
    {
        // 1. Nền Void Black nhám
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        // BẢNG MÀU
        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); // Màu Tím cho mặt phẳng cắt (Gamma)
        Color highlightColor = new Color32(255, 255, 0, 255); // Vàng nổi bật

        // 2. Dựng Mặt phẳng Alpha (Y = 0.6)
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

        // 3. Dựng Mặt phẳng Beta (Y = -0.4)
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

        // 4. Chuẩn bị Mặt phẳng Gamma (vô hình lúc đầu, sau đó hiện dần)
        GameObject g1 = new GameObject("G1"); GameObject g2 = new GameObject("G2");
        GameObject g3 = new GameObject("G3"); GameObject g4 = new GameObject("G4");
        
        // Cập nhật vị trí sơ bộ để tạo mesh
        g1.transform.position = new Vector3(-2.5f, gammaYOffset + 1.0f, 4.5f);
        g2.transform.position = new Vector3( 2.5f, gammaYOffset + 1.0f, 4.5f);
        g3.transform.position = new Vector3( 2.5f, gammaYOffset - 1.0f, 1.5f);
        g4.transform.position = new Vector3(-2.5f, gammaYOffset - 1.0f, 1.5f);

        GameObject faceGamma = GeoFactory.CreateFace(new GameObject[] { g1, g2, g3, g4 }, gammaColor);
        Material matGamma = faceGamma.GetComponent<MeshRenderer>().sharedMaterial;
        if (matGamma.HasProperty("_BaseColor")) matGamma.DOFade(0.4f, "_BaseColor", 1f);
        else matGamma.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.5f);

        // 5. Chuẩn bị các điểm và đường giao tuyến (Màu Vàng)
        GameObject iA1 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "a", false);
        GameObject iA2 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false);
        GameObject lineA = GeoFactory.CreateLine(iA1, iA2, highlightColor, 0.025f);
        lineA.GetComponent<EdgeFollower>().isAnimating = false;

        GameObject iB1 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "b", false);
        GameObject iB2 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false);
        GameObject lineB = GeoFactory.CreateLine(iB1, iB2, highlightColor, 0.025f);
        lineB.GetComponent<EdgeFollower>().isAnimating = false;

        // 6. Hoạt ảnh hạ mặt phẳng Gamma cắt qua Alpha và Beta
        DOTween.To(() => gammaYOffset, x => gammaYOffset = x, 0.1f, 3.5f)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() => {
                // Cập nhật vị trí mặt phẳng Gamma (Tilted Plane)
                g1.transform.position = new Vector3(-2.5f, gammaYOffset + 1.0f, 4.5f);
                g2.transform.position = new Vector3( 2.5f, gammaYOffset + 1.0f, 4.5f);
                g3.transform.position = new Vector3( 2.5f, gammaYOffset - 1.0f, 1.5f);
                g4.transform.position = new Vector3(-2.5f, gammaYOffset - 1.0f, 1.5f);

                // Tính toán Z giao tuyến a trên mặt Alpha (Y = 0.6)
                float zA = 1.5f + (1.6f - gammaYOffset) * 1.5f;
                UpdateIntersection(iA1, iA2, lineA, zA, 0.6f);

                // Tính toán Z giao tuyến b trên mặt Beta (Y = -0.4)
                float zB = 1.5f + (0.6f - gammaYOffset) * 1.5f;
                UpdateIntersection(iB1, iB2, lineB, zB, -0.4f);
            });

        yield return new WaitForSeconds(4.0f);

        // 7. Hiển thị Diegetic Text a // b ở giữa 2 mặt phẳng
        GameObject labelContainer = new GameObject("DiegeticText");
        labelContainer.transform.position = new Vector3(0, 0.1f, 3.0f); // Nằm lơ lửng giữa Y=0.6 và Y=-0.4
        GeoFactory.CreateLabel(labelContainer.transform, "a // b", highlightColor);
        labelContainer.transform.localScale = Vector3.zero;
        labelContainer.transform.DOScale(1.5f, 1f).SetEase(Ease.OutBack);
    }

    // Hàm phụ trợ ẩn hiện đường giao tuyến khi mặt phẳng quét qua
    void UpdateIntersection(GameObject p1, GameObject p2, GameObject line, float z, float y)
    {
        if (z >= 1.5f && z <= 4.5f) { // Nằm trong ranh giới mặt phẳng Z từ 1.5 đến 4.5
            p1.transform.position = new Vector3(-2f, y, z);
            p2.transform.position = new Vector3( 2f, y, z);
            p1.transform.localScale = Vector3.one * 0.04f;
            p2.transform.localScale = Vector3.one * 0.04f;
            line.SetActive(true);
        } else {
            p1.transform.localScale = Vector3.zero;
            p2.transform.localScale = Vector3.zero;
            line.SetActive(false);
        }
    }
}
