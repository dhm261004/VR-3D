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

    private void OnDisable() => CleanupDetachedRuntimeObjects();
    private void OnDestroy() => CleanupDetachedRuntimeObjects();

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor    = new Color32( 0, 150, 255, 100);
        Color betaColor     = new Color32(0, 255, 150, 100);
        Color gammaColor    = new Color32(255, 50, 255, 80); 
        Color highlightColor = new Color32(255, 255, 0, 255);

        
        GameObject a1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f,  1.5f)), alphaColor, "(P)", false));
        GameObject a4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f,  1.5f)), alphaColor, " ", false));
        a1.transform.localScale = a2.transform.localScale = a3.transform.localScale = a4.transform.localScale = Vector3.one * 0.04f;
        GameObject faceAlpha = Fix(GeoFactory.CreateFace(new GameObject[] { a1, a2, a3, a4 }, alphaColor));
        SetFaceAlpha(faceAlpha, 0.4f);

        GameObject b1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f,  1.5f)), betaColor, "(Q)", false));
        GameObject b4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f,  1.5f)), betaColor, " ", false));
        b1.transform.localScale = b2.transform.localScale = b3.transform.localScale = b4.transform.localScale = Vector3.one * 0.04f;
        GameObject faceBeta = Fix(GeoFactory.CreateFace(new GameObject[] { b1, b2, b3, b4 }, betaColor));
        SetFaceAlpha(faceBeta, 0.4f);

        yield return new WaitForSeconds(0.5f);

        
        g1 = Fix(new GameObject("G1")); g2 = Fix(new GameObject("G2"));
        g3 = Fix(new GameObject("G3")); g4 = Fix(new GameObject("G4"));
        GameObject faceGamma = Fix(GeoFactory.CreateFace(new GameObject[] { g1, g2, g3, g4 }, gammaColor));
        SetFaceAlpha(faceGamma, 0.5f);

        
        iA1 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "a", false));
        iA2 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false));
        lineA = Fix(GeoFactory.CreateLine(iA1, iA2, highlightColor, 0.025f));
        lineA.GetComponent<EdgeFollower>().isAnimating = false;

        iB1 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "b", false));
        iB2 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false));
        lineB = Fix(GeoFactory.CreateLine(iB1, iB2, highlightColor, 0.025f));
        lineB.GetComponent<EdgeFollower>().isAnimating = false;

        
        lever = GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(0f, 1.3f, 0f)), new Color32(255, 50, 50, 255), "CẦN GẠT", true);
        lever.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);

        
        GameObject uiPanel = Fix(new GameObject("UIPanel"));
        uiPanel.transform.localPosition = new Vector3(-2.0f, 2.2f, 0f);
        uiPanel.AddComponent<Billboard>();

        GameObject contentObj = Fix(new GameObject("ContentArea"));
        contentObj.transform.SetParent(uiPanel.transform);
        contentObj.transform.localPosition = Vector3.zero;
        uiContentText = contentObj.AddComponent<TextMeshPro>();
        uiContentText.fontSize = 1.5f; 
        uiContentText.alignment = TextAlignmentOptions.Center;

        GameObject statusObj = Fix(new GameObject("StatusArea"));
        statusObj.transform.SetParent(uiPanel.transform);
        statusObj.transform.localPosition = new Vector3(0, -0.6f, 0);
        uiStatusText = statusObj.AddComponent<TextMeshPro>();
        uiStatusText.fontSize = 1.2f;
        uiStatusText.color = Color.yellow;
        uiStatusText.alignment = TextAlignmentOptions.Center;
        
        uiStatusText.text = "Nhiệm vụ:\nSử dụng CẦN GẠT (khối đỏ) để lật mặt phẳng cắt.\nQuan sát tham số góc giữa 2 giao tuyến.";
    }

    void Update()
    {
        if (lever == null) return;

        
        var ipvr = lever.GetComponent<InteractivePoint_VR>();
        
        bool grabbing = ipvr != null && ipvr.IsGrabbed;

        
        Vector3 pos = transform.InverseTransformPoint(lever.transform.position);

        if (!grabbing)
        {
            
            pos.y = 1.3f;
            pos.z = 0f;
            pos.x = Mathf.Clamp(pos.x, -2.5f, 2.5f);
            lever.transform.position = transform.TransformPoint(pos);
        }
        else
        {
            
            pos.x = Mathf.Clamp(pos.x, -2.5f, 2.5f);
        }

        float cotTheta = pos.x * 0.5f;

        
        float zTop = 0f - cotTheta * 1.0f;
        float zBot = 0f + cotTheta * 1.0f;

        g1.transform.localPosition = new Vector3(-2.5f, 1.1f, zTop);
        g2.transform.localPosition = new Vector3( 2.5f, 1.1f, zTop);
        g3.transform.localPosition = new Vector3( 2.5f, -0.9f, zBot);
        g4.transform.localPosition = new Vector3(-2.5f, -0.9f, zBot);

        
        float zA = 0f - cotTheta * 0.5f;
        UpdateIntersection(iA1, iA2, lineA, zA, 0.6f);

        
        float zB = 0f + cotTheta * 0.5f;
        UpdateIntersection(iB1, iB2, lineB, zB, -0.4f);

        float deltaZ = Mathf.Abs(zB - zA);
        float distance = Mathf.Sqrt(1.0f * 1.0f + deltaZ * deltaZ);

        uiContentText.text = $"Khoảng cách d = {distance:F2} đơn vị\n<color=yellow>Góc giữa a và b = 0°</color>";

        if (!isSuccess && Mathf.Abs(pos.x) > 1.0f)
        {
            isSuccess = true;
            uiStatusText.text = "Xác minh thành công!\nGóc luôn bằng 0° - Hai giao tuyến luôn song song.\n(Hãy chuyển qua module tiếp theo để hoàn thành nhiệm vụ)";
            uiStatusText.color = Color.green;
            uiStatusText.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
        }
    }

    void UpdateIntersection(GameObject p1, GameObject p2, GameObject line, float z, float y)
    {
        
        if (z >= -1.5f && z <= 1.5f) {
            p1.transform.localPosition = new Vector3(-2f, y, z);
            p2.transform.localPosition = new Vector3( 2f, y, z);
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

    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private void CleanupDetachedRuntimeObjects()
    {
        DestroyHandleAndLinkedLabels(ref lever);
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
