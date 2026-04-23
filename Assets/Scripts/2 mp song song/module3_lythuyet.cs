using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module3_lythuyet : MonoBehaviour
{
    private TextMeshPro uiStatusText;

    IEnumerator Start()
    {
        // 1. Nền không gian
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);

        // 2. UI Panel
        GameObject uiPanel = new GameObject("UIPanel");
        uiPanel.transform.position = new Vector3(-2f, 2.5f, 4.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject contentObj = new GameObject("ContentArea");
        contentObj.transform.SetParent(uiPanel.transform);
        contentObj.transform.localPosition = Vector3.zero;
        TextMeshPro uiContentText = contentObj.AddComponent<TextMeshPro>();
        uiContentText.fontSize = 2f;
        uiContentText.alignment = TextAlignmentOptions.TopLeft;
        uiContentText.text = "Định lý Thalès:\n<color=yellow>AB / A'B' = BC / B'C' = AC / A'C'</color>";

        GameObject statusObj = new GameObject("StatusArea");
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = new Vector3(0, -1.0f, 0);
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.2f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.TopLeft;
        uiStatusText.text = "Đang thiết lập không gian...";

        // 3. Khởi tạo 3 Mặt phẳng song song cách đều nhau
        // Alpha: Y = 1.0, Beta: Y = 0.0, Gamma: Y = -1.0
        GameObject faceAlpha = CreatePlane(1.0f, alphaColor, "(P)");
        GameObject faceBeta = CreatePlane(0.0f, betaColor, "(Q)");
        GameObject faceGamma = CreatePlane(-1.0f, gammaColor, "(R)");
        
        yield return new WaitForSeconds(1f);
        uiStatusText.text = "Tạo tia sáng cắt qua 3 mặt phẳng...";

        // 4. Tạo cát tuyến cố định (Laser Line 1)
        Vector3 A = new Vector3(-1.0f, 1.0f, 3.0f);
        Vector3 B = new Vector3(-0.5f, 0.0f, 3.0f);
        Vector3 C = new Vector3( 0.0f,-1.0f, 3.0f);

        // Hiệu ứng bắn laser từ trên xuống
        GameObject laserPoint1 = GeoFactory.CreatePoint(new Vector3(-1.5f, 2.0f, 3.0f), highlightColor, " ", false);
        GameObject laserPoint2 = GeoFactory.CreatePoint(new Vector3(-1.5f, 2.0f, 3.0f), highlightColor, " ", false);
        GameObject lineLaser = GeoFactory.CreateLine(laserPoint1, laserPoint2, highlightColor, 0.03f);
        lineLaser.GetComponent<EdgeFollower>().isAnimating = false;

        laserPoint2.transform.DOMove(new Vector3(0.5f, -2.0f, 3.0f), 2f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(0.5f);
        GameObject ptA = GeoFactory.CreatePoint(A, highlightColor, "A", false);
        ptA.transform.DOScale(0.05f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        GameObject ptB = GeoFactory.CreatePoint(B, highlightColor, "B", false);
        ptB.transform.DOScale(0.05f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        GameObject ptC = GeoFactory.CreatePoint(C, highlightColor, "C", false);
        ptC.transform.DOScale(0.05f, 0.3f);

        yield return new WaitForSeconds(1f);
        uiStatusText.text = "Hoàn tất! Hãy tiếp tục sang phần Thực hành.";
        uiStatusText.color = Color.green;
    }

    GameObject CreatePlane(float yPos, Color color, string label)
    {
        GameObject p1 = GeoFactory.CreatePoint(new Vector3(-3, yPos, 1.5f), color, " ", false);
        GameObject p2 = GeoFactory.CreatePoint(new Vector3( 3, yPos, 1.5f), color, " ", false);
        GameObject p3 = GeoFactory.CreatePoint(new Vector3( 3, yPos, 4.5f), color, label, false);
        GameObject p4 = GeoFactory.CreatePoint(new Vector3(-3, yPos, 4.5f), color, " ", false);
        p1.transform.localScale = p2.transform.localScale = p3.transform.localScale = p4.transform.localScale = Vector3.one * 0.04f;
        
        GameObject face = GeoFactory.CreateFace(new GameObject[] { p1, p2, p3, p4 }, color);
        Material mat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (mat.HasProperty("_BaseColor")) mat.DOFade(0.4f, "_BaseColor", 0.5f);
        else mat.DOFade(0.4f, "_Color", 0.5f);

        return face;
    }
}
