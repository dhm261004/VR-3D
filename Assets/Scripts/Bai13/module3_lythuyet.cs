using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module3_lythuyet : MonoBehaviour
{
    private TextMeshPro uiStatusText;

    // Chiều cao các mặt phẳng (local) - nâng lên và thu hẹp khoảng cách
    private const float Y_P = 1.2f;   // Mặt phẳng P (trên)
    private const float Y_Q = 0.75f;  // Mặt phẳng Q (giữa)
    private const float Y_R = 0.3f;   // Mặt phẳng R (dưới) - đủ cao để không bị đất che

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor  = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);

        // UI Panel
        GameObject uiPanel = Fix(new GameObject("UIPanel"));
        uiPanel.transform.localPosition = new Vector3(-2f, 2.5f, 1.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject contentObj = Fix(new GameObject("ContentArea"));
        contentObj.transform.SetParent(uiPanel.transform);
        contentObj.transform.localPosition = Vector3.zero;
        TextMeshPro uiContentText = contentObj.AddComponent<TextMeshPro>();
        uiContentText.fontSize = 2f;
        uiContentText.alignment = TextAlignmentOptions.TopLeft;
        uiContentText.text = "Định lý Thalès:\n<color=yellow>AB / A'B' = BC / B'C' = AC / A'C'</color>";

        GameObject statusObj = Fix(new GameObject("StatusArea"));
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = new Vector3(0, -1.0f, 0);
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.2f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.TopLeft;
        uiStatusText.text = "Đang thiết lập không gian...";

        // 3 Mặt phẳng song song - khoảng cách 0.45, đủ cao để không bị đất che
        CreatePlane(Y_P, alphaColor, "(P)");
        CreatePlane(Y_Q, betaColor, "(Q)");
        CreatePlane(Y_R, gammaColor, "(R)");
        
        yield return new WaitForSeconds(1f);
        uiStatusText.text = "Tạo tia sáng cắt qua 3 mặt phẳng...";

        // Cát tuyến cố định - điểm giao với 3 mặt phẳng
        Vector3 A = new Vector3(-1.0f, Y_P, 0f);
        Vector3 B = new Vector3(-0.5f, Y_Q, 0f);
        Vector3 C = new Vector3( 0.0f, Y_R, 0f);

        // Hiệu ứng bắn laser
        GameObject laserPoint1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-1.5f, Y_P + 0.8f, 0f)), highlightColor, " ", false));
        GameObject laserPoint2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-1.5f, Y_P + 0.8f, 0f)), highlightColor, " ", false));
        GameObject lineLaser = Fix(GeoFactory.CreateLine(laserPoint1, laserPoint2, highlightColor, 0.03f));
        lineLaser.GetComponent<EdgeFollower>().isAnimating = false;

        laserPoint2.transform.DOLocalMove(new Vector3(0.5f, Y_R - 0.8f, 0f), 2f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(0.5f);
        GameObject ptA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(A), highlightColor, "A", false));
        ptA.transform.DOScale(0.05f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        GameObject ptB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(B), highlightColor, "B", false));
        ptB.transform.DOScale(0.05f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        GameObject ptC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(C), highlightColor, "C", false));
        ptC.transform.DOScale(0.05f, 0.3f);

        yield return new WaitForSeconds(1f);
        uiStatusText.text = "Hoàn tất! Hãy tiếp tục sang phần Thực hành.";
        uiStatusText.color = Color.green;
    }

    void CreatePlane(float yPos, Color color, string label)
    {
        GameObject p1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-3, yPos, -1.5f)), color, " ", false));
        GameObject p2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 3, yPos, -1.5f)), color, " ", false));
        GameObject p3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 3, yPos,  1.5f)), color, label, false));
        GameObject p4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-3, yPos,  1.5f)), color, " ", false));
        p1.transform.localScale = p2.transform.localScale = p3.transform.localScale = p4.transform.localScale = Vector3.one * 0.04f;
        
        GameObject face = Fix(GeoFactory.CreateFace(new GameObject[] { p1, p2, p3, p4 }, color));
        Material mat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (mat.HasProperty("_BaseColor")) mat.DOFade(0.4f, "_BaseColor", 0.5f);
        else mat.DOFade(0.4f, "_Color", 0.5f);
    }

    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }
}
