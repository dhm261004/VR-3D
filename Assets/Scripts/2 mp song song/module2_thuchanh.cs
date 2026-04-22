using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module2_thuchanh : MonoBehaviour
{
    private GameObject lever;
    private GameObject g1, g2, g3, g4;
    private GameObject iA1, iA2, iB1, iB2;
    private GameObject lineA, lineB;
    
    private TextMeshPro uiContentText;
    private TextMeshPro uiStatusText;
    private bool isSuccess = false;

    IEnumerator Start()
    {
        // 1. Nền Void Black nhám
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);

        // 2. Dựng Mặt phẳng Alpha (Y = 0.6) và Beta (Y = -0.4)
        GameObject a1 = GeoFactory.CreatePoint(new Vector3(-2, 0.6f, 1.5f), alphaColor, " ", false);
        GameObject a2 = GeoFactory.CreatePoint(new Vector3( 2, 0.6f, 1.5f), alphaColor, " ", false);
        GameObject a3 = GeoFactory.CreatePoint(new Vector3( 2, 0.6f, 4.5f), alphaColor, "α", false);
        GameObject a4 = GeoFactory.CreatePoint(new Vector3(-2, 0.6f, 4.5f), alphaColor, " ", false);
        a1.transform.localScale = a2.transform.localScale = a3.transform.localScale = a4.transform.localScale = Vector3.one * 0.04f;
        GameObject faceAlpha = GeoFactory.CreateFace(new GameObject[] { a1, a2, a3, a4 }, alphaColor);
        SetFaceAlpha(faceAlpha, 0.4f);

        GameObject b1 = GeoFactory.CreatePoint(new Vector3(-2, -0.4f, 1.5f), betaColor, " ", false);
        GameObject b2 = GeoFactory.CreatePoint(new Vector3( 2, -0.4f, 1.5f), betaColor, " ", false);
        GameObject b3 = GeoFactory.CreatePoint(new Vector3( 2, -0.4f, 4.5f), betaColor, "β", false);
        GameObject b4 = GeoFactory.CreatePoint(new Vector3(-2, -0.4f, 4.5f), betaColor, " ", false);
        b1.transform.localScale = b2.transform.localScale = b3.transform.localScale = b4.transform.localScale = Vector3.one * 0.04f;
        GameObject faceBeta = GeoFactory.CreateFace(new GameObject[] { b1, b2, b3, b4 }, betaColor);
        SetFaceAlpha(faceBeta, 0.4f);

        yield return new WaitForSeconds(0.5f);

        // 3. Chuẩn bị Mặt phẳng Gamma
        g1 = new GameObject("G1"); g2 = new GameObject("G2");
        g3 = new GameObject("G3"); g4 = new GameObject("G4");
        GameObject faceGamma = GeoFactory.CreateFace(new GameObject[] { g1, g2, g3, g4 }, gammaColor);
        SetFaceAlpha(faceGamma, 0.5f);

        // 4. Chuẩn bị các điểm và đường giao tuyến
        iA1 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "a", false);
        iA2 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false);
        lineA = GeoFactory.CreateLine(iA1, iA2, highlightColor, 0.025f);
        lineA.GetComponent<EdgeFollower>().isAnimating = false;

        iB1 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "b", false);
        iB2 = GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false);
        lineB = GeoFactory.CreateLine(iB1, iB2, highlightColor, 0.025f);
        lineB.GetComponent<EdgeFollower>().isAnimating = false;

        // 5. Khởi tạo Cần gạt Vật lý (Interactive Point)
        // isDraggable = true cho phép VR lấy con trỏ kéo thả được
        lever = GeoFactory.CreatePoint(new Vector3(0f, 1.3f, 3.0f), new Color32(255, 50, 50, 255), "CẦN GẠT", true);
        lever.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);

        // 6. Khởi tạo UI (Top Left Panel)
        GameObject uiPanel = new GameObject("UIPanel");
        uiPanel.transform.position = new Vector3(-2.0f, 2.2f, 3.0f); // Kéo lên cao và đẩy ra sát mép góc trái
        uiPanel.transform.rotation = Quaternion.Euler(0, 0, 0); 
        uiPanel.AddComponent<Billboard>(); // Đảm bảo luôn quay mặt về phía Camera

        GameObject contentObj = new GameObject("ContentArea");
        contentObj.transform.SetParent(uiPanel.transform);
        contentObj.transform.localPosition = Vector3.zero;
        uiContentText = contentObj.AddComponent<TextMeshPro>();
        uiContentText.fontSize = 1.5f; 
        uiContentText.alignment = TextAlignmentOptions.Center; // Trả về Center để text bám đúng tọa độ tâm

        GameObject statusObj = new GameObject("StatusArea");
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = new Vector3(0, -0.6f, 0);
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.2f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.Center; // Trả về Center
        
        uiStatusText.text = "Nhiệm vụ:\nSử dụng CẦN GẠT (khối đỏ) để lật mặt phẳng cắt.\nQuan sát tham số góc giữa 2 giao tuyến.";
    }

    void Update()
    {
        if (lever == null) return;

        // 1. Ép toạ độ Cần gạt để nó chỉ trượt dọc theo trục X (trái/phải) cho dễ kéo trên màn 2D
        Vector3 pos = lever.transform.position;
        pos.y = 1.3f;
        pos.z = 3.0f;
        pos.x = Mathf.Clamp(pos.x, -2.5f, 2.5f);
        lever.transform.position = pos;

        // 2. Tính toán độ nghiêng của mặt phẳng Gamma dựa trên vị trí Cần gạt
        // Khi pos.x = 0 -> cotTheta = 0 -> Mặt phẳng thẳng đứng
        float cotTheta = pos.x * 0.5f;

        // 3. Cập nhật 4 góc của mặt phẳng Gamma
        float zTop = 3.0f - cotTheta * 1.0f; // Y = 1.1
        float zBot = 3.0f + cotTheta * 1.0f; // Y = -0.9

        g1.transform.position = new Vector3(-2.5f, 1.1f, zTop);
        g2.transform.position = new Vector3( 2.5f, 1.1f, zTop);
        g3.transform.position = new Vector3( 2.5f, -0.9f, zBot);
        g4.transform.position = new Vector3(-2.5f, -0.9f, zBot);

        // 4. Cập nhật giao tuyến a (trên mặt Alpha Y = 0.6)
        float zA = 3.0f - cotTheta * 0.5f;
        UpdateIntersection(iA1, iA2, lineA, zA, 0.6f);

        // 5. Cập nhật giao tuyến b (trên mặt Beta Y = -0.4)
        float zB = 3.0f + cotTheta * 0.5f;
        UpdateIntersection(iB1, iB2, lineB, zB, -0.4f);

        // 6. Tính khoảng cách giữa hai giao tuyến
        // Cạnh góc vuông dọc = 1.0 (0.6 - (-0.4)), Cạnh góc vuông ngang = khoảng cách Z giữa a và b
        float deltaZ = Mathf.Abs(zB - zA);
        float distance = Mathf.Sqrt(1.0f * 1.0f + deltaZ * deltaZ);

        // 7. Cập nhật UI
        uiContentText.text = $"Khoảng cách d = {distance:F2} đơn vị\n<color=yellow>Góc giữa a và b = 0°</color>";

        // 8. Đánh giá hoàn thành (Người dùng đã kéo cần gạt một đoạn đủ xa)
        if (!isSuccess && Mathf.Abs(pos.x) > 1.0f)
        {
            isSuccess = true;
            uiStatusText.text = "Xác minh thành công!\nGóc luôn bằng 0° - Hai giao tuyến luôn song song.\n(Nút 'Module Tiếp theo' đã kích hoạt)";
            uiStatusText.color = Color.green;
            uiStatusText.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
        }
    }

    void UpdateIntersection(GameObject p1, GameObject p2, GameObject line, float z, float y)
    {
        if (z >= 1.5f && z <= 4.5f) {
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

    void SetFaceAlpha(GameObject face, float alpha)
    {
        Material m = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (m.HasProperty("_BaseColor")) m.DOFade(alpha, "_BaseColor", 0.1f);
        else m.DOFade(alpha, "_Color", 0.1f);
    }
}
