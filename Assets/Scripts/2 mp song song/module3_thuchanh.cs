using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module3_thuchanh : MonoBehaviour
{
    private GameObject handleTop, handleBot;
    private GameObject ptAprime, ptBprime, ptCprime;
    private TextMeshPro col1, col2, col3;
    private TextMeshPro uiStatusText;
    private bool isSuccess = false;
    private Vector3 startHandlePos;

    // Fixed points for line 1
    private Vector3 A = new Vector3(-1.0f, 1.0f, 3.0f);
    private Vector3 B = new Vector3(-0.5f, 0.0f, 3.0f);
    private Vector3 C = new Vector3( 0.0f,-1.0f, 3.0f);

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);
        Color rodColor = new Color32(255, 100, 100, 255);

        // Khởi tạo 3 Mặt phẳng song song
        CreatePlane(1.0f, alphaColor, "(P)");
        CreatePlane(0.0f, betaColor, "(Q)");
        CreatePlane(-1.0f, gammaColor, "(R)");

        // Khởi tạo tia sáng 1 (Laser cố định)
        GameObject l1p1 = GeoFactory.CreatePoint(new Vector3(-1.5f, 2.0f, 3.0f), highlightColor, " ", false);
        GameObject l1p2 = GeoFactory.CreatePoint(new Vector3(0.5f, -2.0f, 3.0f), highlightColor, " ", false);
        GameObject line1 = GeoFactory.CreateLine(l1p1, l1p2, highlightColor, 0.02f);
        line1.GetComponent<EdgeFollower>().isAnimating = false;

        GeoFactory.CreatePoint(A, highlightColor, "A", false).transform.localScale = Vector3.one * 0.04f;
        GeoFactory.CreatePoint(B, highlightColor, "B", false).transform.localScale = Vector3.one * 0.04f;
        GeoFactory.CreatePoint(C, highlightColor, "C", false).transform.localScale = Vector3.one * 0.04f;

        // Khởi tạo Cát tuyến 2 (Thanh kim loại)
        handleTop = GeoFactory.CreatePoint(new Vector3(1.0f, 1.5f, 3.0f), new Color32(255, 50, 50, 255), "Kéo", true);
        handleBot = GeoFactory.CreatePoint(new Vector3(1.5f, -1.5f, 3.0f), new Color32(255, 50, 50, 255), "Kéo", true);
        handleTop.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);
        handleBot.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);

        GameObject line2 = GeoFactory.CreateLine(handleTop, handleBot, rodColor, 0.04f);
        line2.GetComponent<EdgeFollower>().isAnimating = false;

        ptAprime = GeoFactory.CreatePoint(Vector3.zero, rodColor, "A'", false);
        ptBprime = GeoFactory.CreatePoint(Vector3.zero, rodColor, "B'", false);
        ptCprime = GeoFactory.CreatePoint(Vector3.zero, rodColor, "C'", false);
        ptAprime.transform.localScale = ptBprime.transform.localScale = ptCprime.transform.localScale = Vector3.one * 0.05f;

        startHandlePos = handleTop.transform.position;

        // Khởi tạo UI Panel
        GameObject uiPanel = new GameObject("UIPanel");
        uiPanel.transform.position = new Vector3(0, 2.8f, 4.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject col1Obj = new GameObject("Col1"); col1Obj.transform.SetParent(uiPanel.transform); col1Obj.transform.localPosition = new Vector3(-1.8f, 0, 0);
        col1 = col1Obj.AddComponent<TextMeshPro>(); col1.fontSize = 1.2f; col1.alignment = TextAlignmentOptions.Center;

        GameObject col2Obj = new GameObject("Col2"); col2Obj.transform.SetParent(uiPanel.transform); col2Obj.transform.localPosition = new Vector3(0, 0, 0);
        col2 = col2Obj.AddComponent<TextMeshPro>(); col2.fontSize = 1.2f; col2.alignment = TextAlignmentOptions.Center;

        GameObject col3Obj = new GameObject("Col3"); col3Obj.transform.SetParent(uiPanel.transform); col3Obj.transform.localPosition = new Vector3(1.8f, 0, 0);
        col3 = col3Obj.AddComponent<TextMeshPro>(); col3.fontSize = 1.2f; col3.alignment = TextAlignmentOptions.Center;

        GameObject statusObj = new GameObject("StatusArea");
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = new Vector3(0, -1.2f, 0);
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.0f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.Center;
        uiStatusText.text = "Nhiệm vụ:\nDùng thanh trục cắt đâm xuyên 3 mặt phẳng ở nhiều góc độ để kiểm chứng tỉ lệ.";

        yield return null;
    }

    void Update()
    {
        if (handleTop == null || handleBot == null) return;

        // Ép tọa độ handle để tránh thanh nằm ngang không cắt được mặt phẳng
        Vector3 pT = handleTop.transform.position;
        if (pT.y < 1.1f) { pT.y = 1.1f; handleTop.transform.position = pT; }
        
        Vector3 pB = handleBot.transform.position;
        if (pB.y > -1.1f) { pB.y = -1.1f; handleBot.transform.position = pB; }

        // Tính giao điểm A', B', C' với các mặt phẳng Y=1, Y=0, Y=-1
        float dy = pB.y - pT.y;
        
        // Alpha Y = 1.0
        float tA = (1.0f - pT.y) / dy;
        Vector3 Aprime = pT + tA * (pB - pT);
        ptAprime.transform.position = Aprime;

        // Beta Y = 0.0
        float tB = (0.0f - pT.y) / dy;
        Vector3 Bprime = pT + tB * (pB - pT);
        ptBprime.transform.position = Bprime;

        // Gamma Y = -1.0
        float tC = (-1.0f - pT.y) / dy;
        Vector3 Cprime = pT + tC * (pB - pT);
        ptCprime.transform.position = Cprime;

        // Cập nhật UI
        float AB = Vector3.Distance(A, B);
        float BC = Vector3.Distance(B, C);
        float AC = Vector3.Distance(A, C);

        float ApBp = Vector3.Distance(Aprime, Bprime);
        float BpCp = Vector3.Distance(Bprime, Cprime);
        float ApCp = Vector3.Distance(Aprime, Cprime);

        float ratio1 = AB / ApBp;
        float ratio2 = BC / BpCp;
        float ratio3 = AC / ApCp;

        col1.text = $"<color=white>AB / A'B'</color>\n<size=80%>{AB:F2} / {ApBp:F2}</size>\n<color=yellow>= {ratio1:F2}</color>";
        col2.text = $"<color=white>BC / B'C'</color>\n<size=80%>{BC:F2} / {BpCp:F2}</size>\n<color=yellow>= {ratio2:F2}</color>";
        col3.text = $"<color=white>AC / A'C'</color>\n<size=80%>{AC:F2} / {ApCp:F2}</size>\n<color=yellow>= {ratio3:F2}</color>";

        if (!isSuccess && Vector3.Distance(pT, startHandlePos) > 1.0f)
        {
            isSuccess = true;
            uiStatusText.text = "Định lý Thalès đã được chứng minh!\n(Nút 'Module Tiếp theo' sáng lên)";
            uiStatusText.color = Color.green;
            uiStatusText.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
        }
    }

    void CreatePlane(float yPos, Color color, string label)
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
    }
}
