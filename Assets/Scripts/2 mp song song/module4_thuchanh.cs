using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module4_thuchanh : MonoBehaviour
{
    private int state = 0;
    private TextMeshPro uiStatusText;
    private bool prevVRButtonPressed = false;
    
    // Points
    private Vector3 posA, posB, posC, posD;
    private GameObject ptA, ptB, ptC, ptD;
    private GameObject handleAprime;
    private GameObject ptBprime, ptCprime, ptDprime;

    // Edges
    private GameObject edgeAB, edgeBC, edgeCA;
    private GameObject edgeCD, edgeDA;
    private GameObject edgeAprimeBprime, edgeBprimeCprime, edgeCprimeAprime;
    private GameObject edgeCprimeDprime, edgeDprimeAprime;
    private GameObject edgeAA, edgeBB, edgeCC, edgeDD;

    // Faces
    private GameObject faceABB, faceBCC, faceCAA;
    private GameObject faceCDD, faceDAA;

    // Measures
    private GameObject measureAA, measureBB, measureAB, measureAprimeBprime;

    // Diagonals
    private GameObject diagACprime, diagBDprime, diagCAprime, diagDBprime;
    private GameObject centerO;

    private Vector3 startHandlePos;
    private Color highlightColor = new Color32(255, 255, 0, 255);
    private Color prismColor = new Color32(255, 100, 200, 150);
    private Color transparentGlass = new Color32(100, 200, 255, 60);
    private Color diagColor = new Color32(0, 255, 255, 255);

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);

        // Khởi tạo 2 Mặt phẳng song song
        CreatePlane(1.2f, alphaColor, "(P)");
        CreatePlane(-0.8f, betaColor, "(Q)");

        // Tính toán đa giác đều đáy (Tam giác đều)
        Vector3 center = new Vector3(0, -0.8f, 2.5f);
        float R = 1.5f;
        posA = center + new Vector3(0, 0, R);
        posB = center + new Vector3(R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        posC = center + new Vector3(-R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        posD = posA + (posC - posB); // Đỉnh thứ 4 cho Hình hộp (dùng ở State 1)

        // Phân đoạn Thực hành 1: Khởi tạo lăng trụ tam giác
        ptA = GeoFactory.CreatePoint(posA, highlightColor, "A", false);
        ptB = GeoFactory.CreatePoint(posB, highlightColor, "B", false);
        ptC = GeoFactory.CreatePoint(posC, highlightColor, "C", false);
        ptA.transform.localScale = ptB.transform.localScale = ptC.transform.localScale = Vector3.one * 0.05f;

        edgeAB = CreateLineNoAnim(ptA, ptB, highlightColor, 0.02f);
        edgeBC = CreateLineNoAnim(ptB, ptC, highlightColor, 0.02f);
        edgeCA = CreateLineNoAnim(ptC, ptA, highlightColor, 0.02f);

        // Đáy trên và Handle kéo
        Vector3 offset = new Vector3(1.0f, 2.0f, 0.5f);
        handleAprime = GeoFactory.CreatePointVR(posA + offset, new Color32(255, 50, 50, 255), "Kéo A'", true);
        handleAprime.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);
        startHandlePos = handleAprime.transform.position;

        ptBprime = GeoFactory.CreatePoint(posB + offset, highlightColor, "B'", false);
        ptCprime = GeoFactory.CreatePoint(posC + offset, highlightColor, "C'", false);
        ptBprime.transform.localScale = ptCprime.transform.localScale = Vector3.one * 0.05f;

        edgeAprimeBprime = CreateLineNoAnim(handleAprime, ptBprime, highlightColor, 0.02f);
        edgeBprimeCprime = CreateLineNoAnim(ptBprime, ptCprime, highlightColor, 0.02f);
        edgeCprimeAprime = CreateLineNoAnim(ptCprime, handleAprime, highlightColor, 0.02f);

        // Cạnh bên
        edgeAA = CreateLineNoAnim(ptA, handleAprime, prismColor, 0.025f);
        edgeBB = CreateLineNoAnim(ptB, ptBprime, prismColor, 0.025f);
        edgeCC = CreateLineNoAnim(ptC, ptCprime, prismColor, 0.025f);

        // Mặt bên kính mờ
        faceABB = GeoFactory.CreateFace(new GameObject[] { ptA, ptB, ptBprime, handleAprime }, transparentGlass);
        faceBCC = GeoFactory.CreateFace(new GameObject[] { ptB, ptC, ptCprime, ptBprime }, transparentGlass);
        faceCAA = GeoFactory.CreateFace(new GameObject[] { ptC, ptA, handleAprime, ptCprime }, transparentGlass);

        // Gắn công cụ đo (Caliper Tool) lên mặt bên ABB'A'
        measureAA = GeoFactory.CreateMeasure(ptA, handleAprime, Color.white);
        measureBB = GeoFactory.CreateMeasure(ptB, ptBprime, Color.white);
        measureAB = GeoFactory.CreateMeasure(ptA, ptB, Color.white);
        measureAprimeBprime = GeoFactory.CreateMeasure(handleAprime, ptBprime, Color.white);

        // Khởi tạo UI Panel
        GameObject uiPanel = new GameObject("UIPanel");
        uiPanel.transform.position = new Vector3(-2f, 2.5f, 4.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject statusObj = new GameObject("StatusArea");
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = Vector3.zero;
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.2f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.TopLeft;
        uiStatusText.text = "Thực hành 1: Kéo điểm A' để làm biến dạng Lăng trụ.\nNhấn nút [A/X] trên VR hoặc [Space] trên PC để sang phần tiếp theo.";

        yield return null;
    }

    void Update()
    {
        if (handleAprime == null) return;

        // Ép tọa độ handle A' luôn nằm trên mặt phẳng alpha (Y = 1.2)
        Vector3 pAprime = handleAprime.transform.position;
        if (Mathf.Abs(pAprime.y - 1.2f) > 0.001f) {
            pAprime.y = 1.2f;
            handleAprime.transform.position = pAprime;
        }

        // Tịnh tiến toàn bộ đáy trên
        Vector3 vecAAprime = pAprime - posA;
        ptBprime.transform.position = posB + vecAAprime;
        ptCprime.transform.position = posC + vecAAprime;

        if (state >= 1 && ptDprime != null) {
            ptDprime.transform.position = posD + vecAAprime;
        }
        
        // Cập nhật tâm O
        if (state >= 2 && centerO != null) {
            centerO.transform.position = (ptA.transform.position + ptCprime.transform.position) / 2f;
        }

        // Đọc tín hiệu nút bấm VR (Nút Primary - A/X)
        bool currentVRButtonPressed = false;
        var devices = new System.Collections.Generic.List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool btn) && btn) currentVRButtonPressed = true;
        }

        bool vrWasPressedThisFrame = currentVRButtonPressed && !prevVRButtonPressed;
        prevVRButtonPressed = currentVRButtonPressed;

        // Điều kiện hoàn thành Thực hành 1 và Chuyển tiếp (Nhấn Space hoặc nút VR để chuyển)
        if (state == 0 && ((UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) || vrWasPressedThisFrame)) {
            state = 1;
            if (uiStatusText != null) {
                uiStatusText.text = "Thực hành 2: Hình Hộp.\nCác đường chéo đồng quy tại Tâm đối xứng O.";
                uiStatusText.color = Color.green;
            }
            StartCoroutine(TransitionToParallelepiped());
        }
    }

    IEnumerator TransitionToParallelepiped()
    {
        // Xóa các thước đo và mặt chéo của lăng trụ tam giác
        Destroy(measureAA); Destroy(measureBB); Destroy(measureAB); Destroy(measureAprimeBprime);
        Destroy(edgeCA); Destroy(edgeCprimeAprime); 
        Destroy(faceCAA);

        // Tạo điểm D và D' cho Hình hộp
        ptD = GeoFactory.CreatePoint(posD, highlightColor, "D", false);
        ptD.transform.DOScale(0.05f, 0.5f);
        
        ptDprime = GeoFactory.CreatePoint(posD + (handleAprime.transform.position - posA), highlightColor, "D'", false);
        ptDprime.transform.DOScale(0.05f, 0.5f);

        // Nối các cạnh mới của hình hộp
        edgeCD = CreateLineNoAnim(ptC, ptD, highlightColor, 0.02f);
        edgeDA = CreateLineNoAnim(ptD, ptA, highlightColor, 0.02f);
        edgeCprimeDprime = CreateLineNoAnim(ptCprime, ptDprime, highlightColor, 0.02f);
        edgeDprimeAprime = CreateLineNoAnim(ptDprime, handleAprime, highlightColor, 0.02f);
        
        edgeDD = CreateLineNoAnim(ptD, ptDprime, prismColor, 0.025f);

        // Tạo các mặt bên mới
        faceCDD = GeoFactory.CreateFace(new GameObject[] { ptC, ptD, ptDprime, ptCprime }, transparentGlass);
        faceDAA = GeoFactory.CreateFace(new GameObject[] { ptD, ptA, handleAprime, ptDprime }, transparentGlass);

        yield return new WaitForSeconds(1.0f);

        // Dựng 4 đường chéo không gian
        diagACprime = CreateLineNoAnim(ptA, ptCprime, diagColor, 0.015f);
        diagBDprime = CreateLineNoAnim(ptB, ptDprime, diagColor, 0.015f);
        diagCAprime = CreateLineNoAnim(ptC, handleAprime, diagColor, 0.015f);
        diagDBprime = CreateLineNoAnim(ptD, ptBprime, diagColor, 0.015f);

        yield return new WaitForSeconds(1.0f);

        // Khởi tạo Tâm đối xứng O
        centerO = GeoFactory.CreatePoint((ptA.transform.position + ptCprime.transform.position) / 2f, Color.red, " ", false);
        centerO.transform.DOScale(0.08f, 0.5f).SetEase(Ease.OutBack);
        
        GameObject labelObj = new GameObject("Label_TamO");
        labelObj.transform.SetParent(centerO.transform);
        labelObj.transform.localPosition = Vector3.zero;
        GeoFactory.CreateLabel(labelObj.transform, "Tâm đối xứng O", Color.green);

        // Sang Phân đoạn Thực hành 2
        state = 2; 
    }

    GameObject CreateLineNoAnim(GameObject p1, GameObject p2, Color c, float w) {
        GameObject l = GeoFactory.CreateLine(p1, p2, c, w);
        l.GetComponent<EdgeFollower>().isAnimating = false;
        return l;
    }

    void CreatePlane(float yPos, Color color, string label)
    {
        GameObject p1 = GeoFactory.CreatePoint(new Vector3(-4, yPos, 1.0f), color, " ", false);
        GameObject p2 = GeoFactory.CreatePoint(new Vector3( 4, yPos, 1.0f), color, " ", false);
        GameObject p3 = GeoFactory.CreatePoint(new Vector3( 4, yPos, 5.0f), color, label, false);
        GameObject p4 = GeoFactory.CreatePoint(new Vector3(-4, yPos, 5.0f), color, " ", false);
        p1.transform.localScale = p2.transform.localScale = p3.transform.localScale = p4.transform.localScale = Vector3.one * 0.04f;
        
        GameObject face = GeoFactory.CreateFace(new GameObject[] { p1, p2, p3, p4 }, color);
        Material mat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (mat.HasProperty("_BaseColor")) mat.DOFade(0.4f, "_BaseColor", 0.5f);
        else mat.DOFade(0.4f, "_Color", 0.5f);
    }
}
