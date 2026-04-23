using UnityEngine;
using System.Collections;
using DG.Tweening;

public class B11_M3_Visuals : MonoBehaviour
{
    [Header("Bảng màu Neo-Brutalism Cyber")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color edgeCyan = new Color32(0, 255, 255, 255);     
    private Color edgePink = new Color32(255, 0, 128, 255);    
    private Color edgeYellow = new Color32(255, 215, 0, 255);   
    private Color yellowCyber = new Color32(255, 215, 0, 255); 
    private Color whiteCyber = new Color32(255, 255, 255, 255); 
    
    private Color planeCyan = new Color32(0, 255, 255, 30);   // (P)
    private Color planePink = new Color32(255, 0, 128, 30);   // (Q)
    private Color planeYellow = new Color32(255, 215, 0, 30); // (R)

    [Header("Điều khiển Nhịp độ (Pacing)")]
    public float animSpeed = 1.5f; 
    public float observeTime = 5.0f; 

    [Header("Chế độ Test")]
    public bool autoPlayOnStart = true;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        
        if (autoPlayOnStart) {
            yield return new WaitForSeconds(1.5f); 
            StartCoroutine(SpawnModule3Gallery());
        }
    }

    private IEnumerator SpawnModule3Gallery()
    {
        // Lấy vị trí của Prefab làm gốc
        Vector3 origin = transform.position;
        float zPos = 0f; 
        float yPos = 0f;

        // Trường hợp 1: Đồng quy
        yield return StartCoroutine(Build_DongQuy(origin + new Vector3(-3.5f, yPos, zPos)));
        
        yield return new WaitForSeconds(3.0f); 
        
        // Trường hợp 2: Song song
        yield return StartCoroutine(Build_SongSong(origin + new Vector3(3.5f, yPos, zPos)));
    }

    // Hàm trợ lý để tự động gom con vào Prefab
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    // ==========================================
    // TRƯỜNG HỢP 1: BA GIAO TUYẾN ĐỒNG QUY
    // ==========================================
    private IEnumerator Build_DongQuy(Vector3 center)
    {
        CreateTitle(center + new Vector3(0, 1.8f, 0), "TRƯỜNG HỢP 1:\nBA GIAO TUYẾN ĐỒNG QUY", whiteCyber);

        // Mặt phẳng (P)
        GameObject p_P1 = CreateCorner(center + new Vector3(1.2f, 0.5f, 1.2f));
        GameObject p_P2 = CreateCorner(center + new Vector3(-1.2f, 0.5f, 1.2f));
        GameObject p_P3 = CreateCorner(center + new Vector3(-1.2f, 0.5f, -1.2f));
        GameObject p_P4 = CreateCorner(center + new Vector3(1.2f, 0.5f, -1.2f));
        
        // Mặt phẳng (Q)
        GameObject p_Q1 = CreateCorner(center + new Vector3(1.2f, 1.5f, 0f));
        GameObject p_Q2 = CreateCorner(center + new Vector3(-1.2f, 1.5f, 0f));
        GameObject p_Q3 = CreateCorner(center + new Vector3(-1.2f, -0.5f, 0f));
        GameObject p_Q4 = CreateCorner(center + new Vector3(1.2f, -0.5f, 0f));

        // Mặt phẳng (R)
        GameObject p_R1 = CreateCorner(center + new Vector3(0f, 1.5f, 1.2f));
        GameObject p_R2 = CreateCorner(center + new Vector3(0f, 1.5f, -1.2f));
        GameObject p_R3 = CreateCorner(center + new Vector3(0f, -0.5f, -1.2f));
        GameObject p_R4 = CreateCorner(center + new Vector3(0f, -0.5f, 1.2f));

        GameObject faceP = Fix(GeoFactory.CreateFace(new[] { p_P1, p_P2, p_P3, p_P4 }, planeCyan));
        FadePlane(faceP, 0.2f);
        yield return new WaitForSeconds(0.5f);

        GameObject faceQ = Fix(GeoFactory.CreateFace(new[] { p_Q1, p_Q2, p_Q3, p_Q4 }, planePink));
        FadePlane(faceQ, 0.2f);
        yield return new WaitForSeconds(0.5f);

        GameObject faceR = Fix(GeoFactory.CreateFace(new[] { p_R1, p_R2, p_R3, p_R4 }, planeYellow));
        FadePlane(faceR, 0.2f);
        
        yield return new WaitForSeconds(1.5f);

        // Tạo và Fix giao tuyến
        GameObject lineA = Fix(GeoFactory.CreateLine(CreateCorner(center + new Vector3(-1.2f, 0.5f, 0f)), CreateCorner(center + new Vector3(1.2f, 0.5f, 0f)), edgeCyan));
        GameObject lineB = Fix(GeoFactory.CreateLine(CreateCorner(center + new Vector3(0f, 0.5f, -1.2f)), CreateCorner(center + new Vector3(0f, 0.5f, 1.2f)), edgePink));
        GameObject lineC = Fix(GeoFactory.CreateLine(CreateCorner(center + new Vector3(0f, -0.5f, 0f)), CreateCorner(center + new Vector3(0f, 1.5f, 0f)), edgeYellow));

        AnimateLine(lineA); AnimateLine(lineB); AnimateLine(lineC);
        yield return new WaitForSeconds(animSpeed);

        GameObject pointM = Fix(GeoFactory.CreatePoint(center + new Vector3(0f, 0.5f, 0f), whiteCyber, "M", true));
        pointM.transform.DOScale(0.08f, animSpeed).SetEase(Ease.OutBounce);
        
        yield return new WaitForSeconds(observeTime);
    }

    // ==========================================
    // TRƯỜNG HỢP 2: BA GIAO TUYẾN SONG SONG
    // ==========================================
    private IEnumerator Build_SongSong(Vector3 center)
    {
        CreateTitle(center + new Vector3(0, 1.8f, 0), "TRƯỜNG HỢP 2:\nBA GIAO TUYẾN SONG SONG", whiteCyber);

        GameObject a1 = CreateCorner(center + new Vector3(-0.8f, -0.2f, -1.2f));
        GameObject a2 = CreateCorner(center + new Vector3(-0.8f, -0.2f, 1.2f));
        GameObject b1 = CreateCorner(center + new Vector3(0.8f, -0.2f, -1.2f));
        GameObject b2 = CreateCorner(center + new Vector3(0.8f, -0.2f, 1.2f));
        GameObject c1 = CreateCorner(center + new Vector3(0f, 1.2f, -1.2f));
        GameObject c2 = CreateCorner(center + new Vector3(0f, 1.2f, 1.2f));

        GameObject faceBottom = Fix(GeoFactory.CreateFace(new[] { a1, a2, b2, b1 }, planeCyan));
        FadePlane(faceBottom, 0.2f);
        yield return new WaitForSeconds(0.5f);

        GameObject faceLeft = Fix(GeoFactory.CreateFace(new[] { a1, a2, c2, c1 }, planePink));
        FadePlane(faceLeft, 0.2f);
        yield return new WaitForSeconds(0.5f);

        GameObject faceRight = Fix(GeoFactory.CreateFace(new[] { b1, b2, c2, c1 }, planeYellow));
        FadePlane(faceRight, 0.2f);

        yield return new WaitForSeconds(1.5f);

        GameObject lineA = Fix(GeoFactory.CreateLine(a1, a2, edgeCyan));
        GameObject lineB = Fix(GeoFactory.CreateLine(b1, b2, edgePink));
        GameObject lineC = Fix(GeoFactory.CreateLine(c1, c2, edgeYellow));

        AnimateLine(lineA); AnimateLine(lineB); AnimateLine(lineC);

        yield return new WaitForSeconds(animSpeed);
        lineA.GetComponent<Renderer>().material.DOColor(yellowCyber, "_BaseColor", 0.5f);
        lineB.GetComponent<Renderer>().material.DOColor(yellowCyber, "_BaseColor", 0.5f);
        lineC.GetComponent<Renderer>().material.DOColor(yellowCyber, "_BaseColor", 0.5f);

        yield return new WaitForSeconds(observeTime);
    }

    private GameObject CreateCorner(Vector3 pos) {
        return Fix(GeoFactory.CreatePoint(pos, Color.clear, "", false));
    }

    private void CreateTitle(Vector3 pos, string text, Color color)
    {
        GameObject anchor = new GameObject("TitleAnchor_" + text.GetHashCode());
        anchor.transform.position = pos;
        Fix(anchor);
        GeoFactory.CreateLabel(anchor.transform, text, color);
    }

    private void AnimateLine(GameObject line)
    {
        if (line == null) return;
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null) {
            ef.isAnimating = true; 
            float targetLength = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(targetLength, animSpeed).SetEase(Ease.OutQuad).OnComplete(() => ef.isAnimating = false);
        }
    }

    private void FadePlane(GameObject plane, float targetAlpha)
    {
        if (plane == null) return;
        Material mat = plane.GetComponent<Renderer>().material;
        Color col = mat.color;
        mat.color = new Color(col.r, col.g, col.b, 0);
        string prop = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
        mat.DOFade(targetAlpha, prop, animSpeed);
    }
}