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

    private void OnDisable() => CleanupDetachedRuntimeObjects();
    private void OnDestroy() => CleanupDetachedRuntimeObjects();

    // Chiều cao các mặt phẳng (local) - khớp với module3_lythuyet
    private const float Y_P = 1.2f;
    private const float Y_Q = 0.75f;
    private const float Y_R = 0.3f;

    // Cát tuyến cố định 1 - giao điểm với 3 mặt phẳng tại Z=0
    private Vector3 A = new Vector3(-1.0f, Y_P, 0f);
    private Vector3 B = new Vector3(-0.5f, Y_Q, 0f);
    private Vector3 C = new Vector3( 0.0f, Y_R, 0f);

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor  = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);
        Color rodColor   = new Color32(255, 100, 100, 255);

        // 3 Mặt phẳng song song
        CreatePlane(Y_P, alphaColor, "(P)");
        CreatePlane(Y_Q, betaColor, "(Q)");
        CreatePlane(Y_R, gammaColor, "(R)");

        // Cát tuyến 1 (cố định)
        GameObject l1p1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-1.5f, Y_P + 0.8f, 0f)), highlightColor, " ", false));
        GameObject l1p2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 0.5f, Y_R - 0.8f, 0f)), highlightColor, " ", false));
        GameObject line1 = Fix(GeoFactory.CreateLine(l1p1, l1p2, highlightColor, 0.02f));
        line1.GetComponent<EdgeFollower>().isAnimating = false;

        Fix(GeoFactory.CreatePoint(transform.TransformPoint(A), highlightColor, "A", false)).transform.localScale = Vector3.one * 0.04f;
        Fix(GeoFactory.CreatePoint(transform.TransformPoint(B), highlightColor, "B", false)).transform.localScale = Vector3.one * 0.04f;
        Fix(GeoFactory.CreatePoint(transform.TransformPoint(C), highlightColor, "C", false)).transform.localScale = Vector3.one * 0.04f;

        // Cát tuyến 2 (kéo được) - KHÔNG Fix() để XR Grab hoạt động bình thường
        handleTop = GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(1.0f, Y_P + 0.3f, 0f)), new Color32(255, 50, 50, 255), "Kéo", true);
        handleBot = GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(1.5f, Y_R - 0.3f, 0f)), new Color32(255, 50, 50, 255), "Kéo", true);
        handleTop.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);
        handleBot.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);

        GameObject line2 = Fix(GeoFactory.CreateLine(handleTop, handleBot, rodColor, 0.04f));
        line2.GetComponent<EdgeFollower>().isAnimating = false;

        ptAprime = Fix(GeoFactory.CreatePoint(Vector3.zero, rodColor, "A'", false));
        ptBprime = Fix(GeoFactory.CreatePoint(Vector3.zero, rodColor, "B'", false));
        ptCprime = Fix(GeoFactory.CreatePoint(Vector3.zero, rodColor, "C'", false));
        ptAprime.transform.localScale = ptBprime.transform.localScale = ptCprime.transform.localScale = Vector3.one * 0.05f;

        startHandlePos = transform.InverseTransformPoint(handleTop.transform.position);

        // UI Panel
        GameObject uiPanel = Fix(new GameObject("UIPanel"));
        uiPanel.transform.localPosition = new Vector3(0, Y_P + 1.6f, 1.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject col1Obj = Fix(new GameObject("Col1")); col1Obj.transform.SetParent(uiPanel.transform); col1Obj.transform.localPosition = new Vector3(-1.8f, 0, 0);
        col1 = col1Obj.AddComponent<TextMeshPro>(); col1.fontSize = 1.2f; col1.alignment = TextAlignmentOptions.Center;

        GameObject col2Obj = Fix(new GameObject("Col2")); col2Obj.transform.SetParent(uiPanel.transform); col2Obj.transform.localPosition = new Vector3(0, 0, 0);
        col2 = col2Obj.AddComponent<TextMeshPro>(); col2.fontSize = 1.2f; col2.alignment = TextAlignmentOptions.Center;

        GameObject col3Obj = Fix(new GameObject("Col3")); col3Obj.transform.SetParent(uiPanel.transform); col3Obj.transform.localPosition = new Vector3(1.8f, 0, 0);
        col3 = col3Obj.AddComponent<TextMeshPro>(); col3.fontSize = 1.2f; col3.alignment = TextAlignmentOptions.Center;

        GameObject statusObj = Fix(new GameObject("StatusArea"));
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

        // Handles ở world space (không Fix) → convert về local để tính toán
        Vector3 pT = transform.InverseTransformPoint(handleTop.transform.position);
        Vector3 pB = transform.InverseTransformPoint(handleBot.transform.position);

        bool grabbingTop = handleTop.GetComponent<InteractivePoint_VR>()?.IsGrabbed ?? false;
        bool grabbingBot = handleBot.GetComponent<InteractivePoint_VR>()?.IsGrabbed ?? false;

        // Chỉ ép constraint khi KHÔNG đang bị grab
        if (!grabbingTop && pT.y < Y_P + 0.1f) {
            pT.y = Y_P + 0.1f;
            handleTop.transform.position = transform.TransformPoint(pT);
        }
        if (!grabbingBot && pB.y > Y_R - 0.1f) {
            pB.y = Y_R - 0.1f;
            handleBot.transform.position = transform.TransformPoint(pB);
        }

        // Tính giao điểm A', B', C' với 3 mặt phẳng (trong local space)
        float dy = pB.y - pT.y;
        if (Mathf.Abs(dy) < 0.01f) return; // tránh chia cho 0

        float tA = (Y_P - pT.y) / dy;
        Vector3 Aprime = pT + tA * (pB - pT);
        ptAprime.transform.localPosition = Aprime;

        float tB = (Y_Q - pT.y) / dy;
        Vector3 Bprime = pT + tB * (pB - pT);
        ptBprime.transform.localPosition = Bprime;

        float tC = (Y_R - pT.y) / dy;
        Vector3 Cprime = pT + tC * (pB - pT);
        ptCprime.transform.localPosition = Cprime;

        // Tỉ lệ Thales
        float AB = Vector3.Distance(A, B);
        float BC = Vector3.Distance(B, C);
        float AC = Vector3.Distance(A, C);

        float ApBp = Vector3.Distance(Aprime, Bprime);
        float BpCp = Vector3.Distance(Bprime, Cprime);
        float ApCp = Vector3.Distance(Aprime, Cprime);

        if (ApBp < 0.001f || BpCp < 0.001f || ApCp < 0.001f) return;

        float ratio1 = AB / ApBp;
        float ratio2 = BC / BpCp;
        float ratio3 = AC / ApCp;

        col1.text = $"<color=white>AB / A'B'</color>\n<size=80%>{AB:F2} / {ApBp:F2}</size>\n<color=yellow>= {ratio1:F2}</color>";
        col2.text = $"<color=white>BC / B'C'</color>\n<size=80%>{BC:F2} / {BpCp:F2}</size>\n<color=yellow>= {ratio2:F2}</color>";
        col3.text = $"<color=white>AC / A'C'</color>\n<size=80%>{AC:F2} / {ApCp:F2}</size>\n<color=yellow>= {ratio3:F2}</color>";

        Vector3 curHandleLocalPos = transform.InverseTransformPoint(handleTop.transform.position);
        if (!isSuccess && Vector3.Distance(curHandleLocalPos, startHandlePos) > 0.8f)
        {
            isSuccess = true;
            uiStatusText.text = "Định lý Thalès đã được chứng minh!\n(Nút 'Module Tiếp theo' sáng lên)";
            uiStatusText.color = Color.green;
            uiStatusText.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
        }
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

    private void CleanupDetachedRuntimeObjects()
    {
        DestroyHandleAndLinkedLabels(ref handleTop);
        DestroyHandleAndLinkedLabels(ref handleBot);
    }

    private static void DestroyHandleAndLinkedLabels(ref GameObject handle)
    {
        if (handle == null) return;

        PositionFollower[] followers = FindObjectsByType<PositionFollower>(FindObjectsSortMode.None);
        for (int i = 0; i < followers.Length; i++)
        {
            PositionFollower follower = followers[i];
            if (follower == null || follower.target != handle.transform) continue;
            if (follower.gameObject != null) Destroy(follower.gameObject);
        }

        Destroy(handle);
        handle = null;
    }
}
