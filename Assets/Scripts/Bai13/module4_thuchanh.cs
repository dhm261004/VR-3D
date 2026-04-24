using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module4_thuchanh : MonoBehaviour
{
    private int state = 0;
    private TextMeshPro uiStatusText;
    private bool prevVRButtonPressed = false;
    
    private Vector3 posA, posB, posC, posD;
    private GameObject ptA, ptB, ptC, ptD;
    private GameObject handleAprime;
    private GameObject ptBprime, ptCprime, ptDprime;

    private GameObject edgeAB, edgeBC, edgeCA;
    private GameObject edgeCD, edgeDA;
    private GameObject edgeAprimeBprime, edgeBprimeCprime, edgeCprimeAprime;
    private GameObject edgeCprimeDprime, edgeDprimeAprime;
    private GameObject edgeAA, edgeBB, edgeCC, edgeDD;

    private GameObject faceABB, faceBCC, faceCAA;
    private GameObject faceCDD, faceDAA;

    private GameObject measureAA, measureBB, measureAB, measureAprimeBprime;

    private GameObject diagACprime, diagBDprime, diagCAprime, diagDBprime;
    private GameObject centerO;

    private Vector3 startHandlePos;
    private Color highlightColor  = new Color32(255, 255, 0, 255);
    private Color prismColor      = new Color32(255, 100, 200, 150);
    private Color transparentGlass = new Color32(100, 200, 255, 60);
    private Color diagColor       = new Color32(0, 255, 255, 255);

    private void OnDisable() => CleanupDetachedRuntimeObjects();
    private void OnDestroy() => CleanupDetachedRuntimeObjects();

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor  = new Color32(0, 255, 150, 100);

        // 2 Mặt phẳng song song - Q nằm ở sàn
        CreatePlane(2.0f, alphaColor, "(P)");
        CreatePlane(0.0f, betaColor, "(Q)");

        // Tam giác đều ở đáy (Y = 0.2, tâm Z = 0)
        Vector3 center = new Vector3(0, 0.0f, 0f);
        float R = 1.5f;
        posA = center + new Vector3(0, 0, R);
        posB = center + new Vector3(R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        posC = center + new Vector3(-R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        posD = posA + (posC - posB);

        ptA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posA), highlightColor, "A", false));
        ptB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posB), highlightColor, "B", false));
        ptC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posC), highlightColor, "C", false));
        ptA.transform.localScale = ptB.transform.localScale = ptC.transform.localScale = Vector3.one * 0.05f;

        edgeAB = CreateLineNoAnim(ptA, ptB, highlightColor, 0.02f);
        edgeBC = CreateLineNoAnim(ptB, ptC, highlightColor, 0.02f);
        edgeCA = CreateLineNoAnim(ptC, ptA, highlightColor, 0.02f);

        // Handle A' - KHÔNG Fix() để XR Grab hoạt động bình thường
        Vector3 offset = new Vector3(1.0f, 2.0f, 0.5f);
        handleAprime = GeoFactory.CreatePoint(transform.TransformPoint(posA + offset), new Color32(255, 50, 50, 255), "Kéo A'", true);
        handleAprime.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);
        startHandlePos = transform.InverseTransformPoint(handleAprime.transform.position);

        ptBprime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posB + offset), highlightColor, "B'", false));
        ptCprime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posC + offset), highlightColor, "C'", false));
        ptBprime.transform.localScale = ptCprime.transform.localScale = Vector3.one * 0.05f;

        edgeAprimeBprime = CreateLineNoAnim(handleAprime, ptBprime, highlightColor, 0.02f);
        edgeBprimeCprime = CreateLineNoAnim(ptBprime, ptCprime, highlightColor, 0.02f);
        edgeCprimeAprime = CreateLineNoAnim(ptCprime, handleAprime, highlightColor, 0.02f);

        edgeAA = CreateLineNoAnim(ptA, handleAprime, prismColor, 0.025f);
        edgeBB = CreateLineNoAnim(ptB, ptBprime, prismColor, 0.025f);
        edgeCC = CreateLineNoAnim(ptC, ptCprime, prismColor, 0.025f);

        faceABB = Fix(GeoFactory.CreateFace(new GameObject[] { ptA, ptB, ptBprime, handleAprime }, transparentGlass));
        faceBCC = Fix(GeoFactory.CreateFace(new GameObject[] { ptB, ptC, ptCprime, ptBprime }, transparentGlass));
        faceCAA = Fix(GeoFactory.CreateFace(new GameObject[] { ptC, ptA, handleAprime, ptCprime }, transparentGlass));

        measureAA = Fix(GeoFactory.CreateMeasure(ptA, handleAprime, Color.white));
        measureBB = Fix(GeoFactory.CreateMeasure(ptB, ptBprime, Color.white));
        measureAB = Fix(GeoFactory.CreateMeasure(ptA, ptB, Color.white));
        measureAprimeBprime = Fix(GeoFactory.CreateMeasure(handleAprime, ptBprime, Color.white));

        // UI Panel
        GameObject uiPanel = Fix(new GameObject("UIPanel"));
        uiPanel.transform.localPosition = new Vector3(-2f, 3.5f, 1.5f);
        uiPanel.AddComponent<Billboard>();

        GameObject statusObj = Fix(new GameObject("StatusArea"));
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

        // handleAprime ở world space (không Fix) → convert về local
        Vector3 pAprime = transform.InverseTransformPoint(handleAprime.transform.position);

        bool grabbing = handleAprime.GetComponent<InteractivePoint_VR>()?.IsGrabbed ?? false;

        // Chỉ ép Y = 2.0 khi KHÔNG đang bị grab
        if (!grabbing && Mathf.Abs(pAprime.y - 2.0f) > 0.001f) {
            pAprime.y = 2.0f;
            handleAprime.transform.position = transform.TransformPoint(pAprime);
        }

        // Tịnh tiến toàn bộ đáy trên theo offset của A'
        Vector3 vecAAprime = pAprime - posA; // cả hai đều trong local space
        ptBprime.transform.localPosition = posB + vecAAprime;
        ptCprime.transform.localPosition = posC + vecAAprime;

        if (state >= 1 && ptDprime != null) {
            ptDprime.transform.localPosition = posD + vecAAprime;
        }
        
        // Cập nhật tâm O
        if (state >= 2 && centerO != null) {
            centerO.transform.localPosition = (ptA.transform.localPosition + ptCprime.transform.localPosition) / 2f;
        }

        // Đọc nút VR
        bool currentVRButtonPressed = false;
        var devices = new System.Collections.Generic.List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool btn) && btn) currentVRButtonPressed = true;
        }

        bool vrWasPressedThisFrame = currentVRButtonPressed && !prevVRButtonPressed;
        prevVRButtonPressed = currentVRButtonPressed;

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
        Destroy(measureAA); Destroy(measureBB); Destroy(measureAB); Destroy(measureAprimeBprime);
        Destroy(edgeCA); Destroy(edgeCprimeAprime); 
        Destroy(faceCAA);

        // Tạo D và D' cho hình hộp
        ptD = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posD), highlightColor, "D", false));
        ptD.transform.DOScale(0.05f, 0.5f);
        
        ptDprime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posD + (transform.InverseTransformPoint(handleAprime.transform.position) - posA)), highlightColor, "D'", false));
        ptDprime.transform.DOScale(0.05f, 0.5f);


        edgeCD = CreateLineNoAnim(ptC, ptD, highlightColor, 0.02f);
        edgeDA = CreateLineNoAnim(ptD, ptA, highlightColor, 0.02f);
        edgeCprimeDprime = CreateLineNoAnim(ptCprime, ptDprime, highlightColor, 0.02f);
        edgeDprimeAprime = CreateLineNoAnim(ptDprime, handleAprime, highlightColor, 0.02f);
        edgeDD = CreateLineNoAnim(ptD, ptDprime, prismColor, 0.025f);

        faceCDD = Fix(GeoFactory.CreateFace(new GameObject[] { ptC, ptD, ptDprime, ptCprime }, transparentGlass));
        faceDAA = Fix(GeoFactory.CreateFace(new GameObject[] { ptD, ptA, handleAprime, ptDprime }, transparentGlass));

        yield return new WaitForSeconds(1.0f);

        // 4 đường chéo không gian
        diagACprime = CreateLineNoAnim(ptA, ptCprime, diagColor, 0.015f);
        diagBDprime = CreateLineNoAnim(ptB, ptDprime, diagColor, 0.015f);
        diagCAprime = CreateLineNoAnim(ptC, handleAprime, diagColor, 0.015f);
        diagDBprime = CreateLineNoAnim(ptD, ptBprime, diagColor, 0.015f);

        yield return new WaitForSeconds(1.0f);

        // Tâm đối xứng O
        centerO = Fix(GeoFactory.CreatePoint(
            (ptA.transform.position + ptCprime.transform.position) / 2f, Color.red, " ", false));
        centerO.transform.DOScale(0.08f, 0.5f).SetEase(Ease.OutBack);
        
        GameObject labelObj = Fix(new GameObject("Label_TamO"));
        labelObj.transform.SetParent(centerO.transform);
        labelObj.transform.localPosition = Vector3.zero;
        GeoFactory.CreateLabel(labelObj.transform, "Tâm đối xứng O", Color.green);

        state = 2; 
    }

    GameObject CreateLineNoAnim(GameObject p1, GameObject p2, Color c, float w) {
        GameObject l = Fix(GeoFactory.CreateLine(p1, p2, c, w));
        l.GetComponent<EdgeFollower>().isAnimating = false;
        return l;
    }

    void CreatePlane(float yPos, Color color, string label)
    {
        // Z: -2.0 đến 2.0 (tâm Z = 0)
        GameObject p1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-4, yPos, -2.0f)), color, " ", false));
        GameObject p2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 4, yPos, -2.0f)), color, " ", false));
        GameObject p3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 4, yPos,  2.0f)), color, label, false));
        GameObject p4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-4, yPos,  2.0f)), color, " ", false));
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
        DestroyHandleAndLinkedLabels(ref handleAprime);
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
