using UnityEngine;
using System.Collections;
using DG.Tweening;

public class B11_M1_Visuals : MonoBehaviour
{
    [Header("Bảng màu Cyber Lab")]
    private Color ptColor = new Color32(255, 0, 85, 255);       
    private Color d1Color = new Color32(0, 255, 255, 200);     
    private Color d2Color = new Color32(0, 255, 128, 200);     
    private Color overlapColor = new Color32(255, 215, 0, 255); 
    private Color skewColor = new Color32(255, 69, 0, 255);    
    private Color whiteCyber = new Color32(255, 255, 255, 255); 

    void Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);
        StartCoroutine(SpawnPositionGallery());
    }

    IEnumerator SpawnPositionGallery()
    {
        
        Vector3 origin = transform.position;
        float z = 0f; 
        float y = 0f; 
        float spacing = 4.0f;

        
        Build_Intersecting(origin + new Vector3(-spacing * 1.5f, y, z));
        yield return new WaitForSeconds(0.3f);

        
        Build_Parallel(origin + new Vector3(-spacing * 0.5f, y, z));
        yield return new WaitForSeconds(0.3f);

        
        Build_Coincident(origin + new Vector3(spacing * 0.5f, y, z));
        yield return new WaitForSeconds(0.3f);

        
        Build_Skew(origin + new Vector3(spacing * 1.5f, y, z));
    }

    
    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }

    private void Build_Intersecting(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "HAI ĐƯỜNG THẲNG\nCẮT NHAU", d1Color);

        GameObject A = Fix(GeoFactory.CreatePoint(center + new Vector3(-1f, 0.5f, 0), ptColor, "A", true));
        GameObject B = Fix(GeoFactory.CreatePoint(center + new Vector3(1f, -0.5f, 0), ptColor, "B", true));
        GameObject C = Fix(GeoFactory.CreatePoint(center + new Vector3(-0.5f, -1f, 0), ptColor, "C", true));
        GameObject D = Fix(GeoFactory.CreatePoint(center + new Vector3(0.5f, 1f, 0), ptColor, "D", true));
        GameObject M = Fix(GeoFactory.CreatePoint(center, whiteCyber, "M", true));

        AnimatePoints(new[] { A, B, C, D }, M);
        AnimateLines(Fix(GeoFactory.CreateLine(A, B, d1Color)), Fix(GeoFactory.CreateLine(C, D, d2Color)));
    }

    private void Build_Parallel(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "HAI ĐƯỜNG THẲNG\nSONG SONG", d2Color);

        GameObject A = Fix(GeoFactory.CreatePoint(center + new Vector3(-1f, 0.4f, 0), ptColor, "A", true));
        GameObject B = Fix(GeoFactory.CreatePoint(center + new Vector3(1f, 0.4f, 0), ptColor, "B", true));
        GameObject C = Fix(GeoFactory.CreatePoint(center + new Vector3(-1f, -0.4f, 0), ptColor, "C", true));
        GameObject D = Fix(GeoFactory.CreatePoint(center + new Vector3(1f, -0.4f, 0), ptColor, "D", true));

        AnimatePoints(new[] { A, B, C, D }, null);
        AnimateLines(Fix(GeoFactory.CreateLine(A, B, d1Color)), Fix(GeoFactory.CreateLine(C, D, d2Color)));
    }

    private void Build_Coincident(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "HAI ĐƯỜNG THẲNG\nTRÙNG NHAU", overlapColor);

        GameObject A = Fix(GeoFactory.CreatePoint(center + new Vector3(-1f, 0, 0), ptColor, "A", true));
        GameObject B = Fix(GeoFactory.CreatePoint(center + new Vector3(1f, 0, 0), ptColor, "B", true));
        GameObject C = Fix(GeoFactory.CreatePoint(center + new Vector3(-0.6f, 0, 0), ptColor, "C", true));
        GameObject D = Fix(GeoFactory.CreatePoint(center + new Vector3(0.6f, 0, 0), ptColor, "D", true));

        AnimatePoints(new[] { A, B, C, D }, null);
        
        GameObject d1 = Fix(GeoFactory.CreateLine(A, B, whiteCyber, 0.01f));
        GameObject d2 = Fix(GeoFactory.CreateLine(C, D, overlapColor, 0.025f));
        AnimateLines(d1, d2);
    }

    private void Build_Skew(Vector3 center)
    {
        CreateTitle(center + Vector3.up * 1.5f, "HAI ĐƯỜNG THẲNG\nCHÉO NHAU", skewColor);

        GameObject A = Fix(GeoFactory.CreatePoint(center + new Vector3(-1f, -0.5f, -0.5f), ptColor, "A", true));
        GameObject B = Fix(GeoFactory.CreatePoint(center + new Vector3(1f, -0.5f, 0.5f), ptColor, "B", true));
        GameObject C = Fix(GeoFactory.CreatePoint(center + new Vector3(0.5f, 0.8f, -1f), ptColor, "C", true));
        GameObject D = Fix(GeoFactory.CreatePoint(center + new Vector3(-0.5f, -0.8f, 1f), ptColor, "D", true));

        AnimatePoints(new[] { A, B, C, D }, null);
        AnimateLines(Fix(GeoFactory.CreateLine(A, B, skewColor)), Fix(GeoFactory.CreateLine(C, D, skewColor)));
    }

    private void CreateTitle(Vector3 pos, string text, Color color)
    {
        GameObject anchor = new GameObject("Title_" + text.Replace("\n", "_"));
        anchor.transform.position = pos;
        Fix(anchor); 
        GeoFactory.CreateLabel(anchor.transform, text, color);
    }

    private void AnimatePoints(GameObject[] pts, GameObject mid)
    {
        foreach(var p in pts) if(p != null) p.transform.DOScale(0.04f, 0.5f).SetEase(Ease.OutBack);
        if(mid != null) mid.transform.DOScale(0.07f, 0.5f).SetEase(Ease.OutBounce).SetDelay(0.2f);
    }

    private void AnimateLines(GameObject l1, GameObject l2)
    {
        if(l1 != null) StartCoroutine(GrowLine(l1));
        if(l2 != null) StartCoroutine(GrowLine(l2));
    }

    private IEnumerator GrowLine(GameObject line)
    {
        yield return new WaitForSeconds(0.5f);
        EdgeFollower ef = line.GetComponent<EdgeFollower>();
        if (ef != null)
        {
            ef.isAnimating = true;
            float dist = Vector3.Distance(ef.p1.position, ef.p2.position) / 2f;
            line.transform.DOScaleY(dist, 0.7f).SetEase(Ease.OutCubic).OnComplete(() => ef.isAnimating = false);
        }
    }
}