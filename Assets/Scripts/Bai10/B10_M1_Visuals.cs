using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class B10_M1_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 180);     
    private Color yellowCyber = new Color32(255, 215, 0, 255);  
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    private Color planeColor = new Color32(0, 255, 255, 30);    

    [Header("Điều khiển Nhịp độ")]
    public float animSpeed = 1.5f; 
    public float observeTime = 4.0f; 

    private TextMeshPro _labelA;
    private TextMeshPro _labelB;
    private GameObject _pointA;
    private GameObject _pointB;
    private float _localPlaneY; // Chuyển sang dùng Local Y

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        yield return new WaitForSeconds(1.0f); 
        
        // Sử dụng local Vector3.zero để làm tâm tại đúng vị trí đặt Prefab
        StartCoroutine(Build_Module1_Concept(Vector3.zero)); 
    }

    // Hàm Fix trợ lý
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private IEnumerator Build_Module1_Concept(Vector3 localCenter)
    {
        _localPlaneY = localCenter.y;
        // Tạo Title và gán cha
        TextMeshPro title = CreateDynamicTitle(localCenter + new Vector3(0, 1.8f, 0.5f), "KHÁI NIỆM MẶT PHẲNG", edgeCyan);

        // --- PHẦN 1: DỰNG MẶT PHẲNG (P) ---
        // Sử dụng tọa độ Local để tính toán
        GameObject p1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-1.0f, 0, -0.8f)), Color.clear, "", false));
        GameObject p2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.0f, 0, -0.8f)), Color.clear, "", false));
        GameObject p3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(1.5f, 0, 0.8f)), Color.clear, "", false));
        GameObject p4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-0.5f, 0, 0.8f)), Color.clear, "", false));

        GameObject[] edges = {
            Fix(GeoFactory.CreateLine(p1, p2, edgeCyan, 0.008f)),
            Fix(GeoFactory.CreateLine(p2, p3, edgeCyan, 0.008f)),
            Fix(GeoFactory.CreateLine(p3, p4, edgeCyan, 0.008f)),
            Fix(GeoFactory.CreateLine(p4, p1, edgeCyan, 0.008f))
        };
        foreach(var e in edges) AnimateLine(e);
        yield return new WaitForSeconds(animSpeed);

        GameObject faceP = Fix(GeoFactory.CreateFace(new[] { p1, p2, p3, p4 }, planeColor));
        FadePlane(faceP, 0.25f);

        yield return new WaitForSeconds(1f);

        // --- PHẦN 2: HIỆU ỨNG MỞ RỘNG ---
        float spreadX = 3.5f; 
        float spreadZ = 2.5f;

        // Di chuyển bằng LocalPosition để đảm bảo bám theo cha
        p1.transform.DOLocalMove(localCenter + new Vector3(-spreadX, 0, -spreadZ), animSpeed * 2.5f);
        p2.transform.DOLocalMove(localCenter + new Vector3(spreadX, 0, -spreadZ), animSpeed * 2.5f);
        p3.transform.DOLocalMove(localCenter + new Vector3(spreadX + 1f, 0, spreadZ), animSpeed * 2.5f);
        p4.transform.DOLocalMove(localCenter + new Vector3(-spreadX + 1f, 0, spreadZ), animSpeed * 2.5f);

        foreach(var e in edges) {
            e.GetComponent<Renderer>().material.DOFade(0.02f, animSpeed * 2.5f);
        }

        yield return new WaitForSeconds(animSpeed);

        // --- PHẦN 3: QUAN HỆ ĐIỂM - MẶT PHẲNG ---
        title.text = "QUAN HỆ ĐIỂM VÀ MẶT PHẲNG\n(Hãy thử kéo các điểm)";
        
        _pointA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(0.5f, 0, 0f)), ptColor, "A", true));
        _pointB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(localCenter + new Vector3(-0.5f, 0.8f, 0.2f)), ptColor, "B", true));

        _pointA.transform.DOScale(0.05f, animSpeed).SetEase(Ease.OutBounce);
        _pointB.transform.DOScale(0.05f, animSpeed).SetEase(Ease.OutBounce);

        _labelA = CreateDynamicTitle(localCenter, "A IN (P)", whiteCyber);
        _labelB = CreateDynamicTitle(localCenter, "B NOT IN (P)", whiteCyber);
        _labelA.fontSize = 1.2f; 
        _labelB.fontSize = 1.2f;

        StartCoroutine(MonitorPointPlaneRelation());
    }

    private IEnumerator MonitorPointPlaneRelation()
    {
        while (true)
        {
            if (_pointA != null && _labelA != null) UpdatePointLabel(_pointA, _labelA, "A");
            if (_pointB != null && _labelB != null) UpdatePointLabel(_pointB, _labelB, "B");
            yield return null; 
        }
    }

    private void UpdatePointLabel(GameObject point, TextMeshPro label, string name)
    {
        // Sử dụng localPosition để tính toán khoảng cách Y tương đối với Prefab cha
        label.transform.localPosition = point.transform.localPosition + Vector3.up * 0.3f;
        float dist = Mathf.Abs(point.transform.localPosition.y - _localPlaneY);
        
        if (dist < 0.05f) {
            label.text = name + " thuộc (P)"; // Dùng ký hiệu thuộc toán học cho chuyên nghiệp
            label.color = yellowCyber;
        } else {
            label.text = name + " không thuộc (P)"; // Dùng ký hiệu không thuộc
            label.color = whiteCyber;
        }
    }

    private TextMeshPro CreateDynamicTitle(Vector3 localPos, string text, Color color)
    {
        GameObject anchor = new GameObject("Label_" + text.GetHashCode());
        Fix(anchor);
        anchor.transform.localPosition = localPos;
        TextMeshPro tm = anchor.AddComponent<TextMeshPro>();
        tm.text = text; 
        tm.color = color; 
        tm.fontSize = 1.8f;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        anchor.AddComponent<Billboard>();
        return tm;
    }

    private void AnimateLine(GameObject line)
    {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true; 
            float targetLength = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(targetLength, animSpeed).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject face, float targetAlpha)
    {
        if (face == null) return;
        Material mat = face.GetComponent<Renderer>().material;
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(targetAlpha, prop, animSpeed);
    }
}