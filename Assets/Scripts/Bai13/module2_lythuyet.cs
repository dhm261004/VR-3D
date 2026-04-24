using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module2 : MonoBehaviour
{
    private float gammaYOffset = 2.5f;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor  = new Color32(0, 255, 150, 100);
        Color gammaColor = new Color32(255, 50, 255, 80);
        Color highlightColor = new Color32(255, 255, 0, 255);

        // Mặt phẳng Alpha (Y = 0.6) - tâm Z = 0
        GameObject a1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f, -1.5f)), alphaColor, " ", false));
        GameObject a3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, 0.6f,  1.5f)), alphaColor, "(P)", false));
        GameObject a4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, 0.6f,  1.5f)), alphaColor, " ", false));
        a1.transform.DOScale(0.04f, 0.5f); a2.transform.DOScale(0.04f, 0.5f);
        a3.transform.DOScale(0.04f, 0.5f); a4.transform.DOScale(0.04f, 0.5f);
        GameObject faceAlpha = Fix(GeoFactory.CreateFace(new GameObject[] { a1, a2, a3, a4 }, alphaColor));
        Material matAlpha = faceAlpha.GetComponent<MeshRenderer>().sharedMaterial;
        if (matAlpha.HasProperty("_BaseColor")) matAlpha.DOFade(0.4f, "_BaseColor", 1f);
        else matAlpha.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.5f);

        // Mặt phẳng Beta (Y = -0.4)
        GameObject b1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f, -1.5f)), betaColor, " ", false));
        GameObject b3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 2, -0.4f,  1.5f)), betaColor, "(Q)", false));
        GameObject b4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-2, -0.4f,  1.5f)), betaColor, " ", false));
        b1.transform.DOScale(0.04f, 0.5f); b2.transform.DOScale(0.04f, 0.5f);
        b3.transform.DOScale(0.04f, 0.5f); b4.transform.DOScale(0.04f, 0.5f);
        GameObject faceBeta = Fix(GeoFactory.CreateFace(new GameObject[] { b1, b2, b3, b4 }, betaColor));
        Material matBeta = faceBeta.GetComponent<MeshRenderer>().sharedMaterial;
        if (matBeta.HasProperty("_BaseColor")) matBeta.DOFade(0.4f, "_BaseColor", 1f);
        else matBeta.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.8f);

        // Mặt phẳng Gamma (nghiêng, bắt đầu ở trên rồi hạ xuống)
        GameObject g1 = Fix(new GameObject("G1")); GameObject g2 = Fix(new GameObject("G2"));
        GameObject g3 = Fix(new GameObject("G3")); GameObject g4 = Fix(new GameObject("G4"));

        // Vị trí ban đầu (trên cao)
        g1.transform.position = transform.TransformPoint(new Vector3(-2.5f, gammaYOffset + 1.0f,  1.5f));
        g2.transform.position = transform.TransformPoint(new Vector3( 2.5f, gammaYOffset + 1.0f,  1.5f));
        g3.transform.position = transform.TransformPoint(new Vector3( 2.5f, gammaYOffset - 1.0f, -1.5f));
        g4.transform.position = transform.TransformPoint(new Vector3(-2.5f, gammaYOffset - 1.0f, -1.5f));

        GameObject faceGamma = Fix(GeoFactory.CreateFace(new GameObject[] { g1, g2, g3, g4 }, gammaColor));
        Material matGamma = faceGamma.GetComponent<MeshRenderer>().sharedMaterial;
        if (matGamma.HasProperty("_BaseColor")) matGamma.DOFade(0.4f, "_BaseColor", 1f);
        else matGamma.DOFade(0.4f, "_Color", 1f);
        yield return new WaitForSeconds(0.5f);

        // Giao tuyến a và b (ẩn lúc đầu)
        GameObject iA1 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "a", false));
        GameObject iA2 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false));
        GameObject lineA = Fix(GeoFactory.CreateLine(iA1, iA2, highlightColor, 0.025f));
        lineA.GetComponent<EdgeFollower>().isAnimating = false;

        GameObject iB1 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, "b", false));
        GameObject iB2 = Fix(GeoFactory.CreatePoint(new Vector3(0, -100, 0), highlightColor, " ", false));
        GameObject lineB = Fix(GeoFactory.CreateLine(iB1, iB2, highlightColor, 0.025f));
        lineB.GetComponent<EdgeFollower>().isAnimating = false;

        // Animation hạ mặt phẳng Gamma xuống cắt qua Alpha và Beta
        DOTween.To(() => gammaYOffset, x => gammaYOffset = x, 0.1f, 3.5f)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() => {
                // Cập nhật vị trí Gamma (tâm Z = 0)
                g1.transform.position = transform.TransformPoint(new Vector3(-2.5f, gammaYOffset + 1.0f,  1.5f));
                g2.transform.position = transform.TransformPoint(new Vector3( 2.5f, gammaYOffset + 1.0f,  1.5f));
                g3.transform.position = transform.TransformPoint(new Vector3( 2.5f, gammaYOffset - 1.0f, -1.5f));
                g4.transform.position = transform.TransformPoint(new Vector3(-2.5f, gammaYOffset - 1.0f, -1.5f));

                // Giao tuyến a với Alpha (Y = 0.6): z = -1.5 + (1.6f - gammaYOffset) * 1.5f
                float zA = -1.5f + (1.6f - gammaYOffset) * 1.5f;
                UpdateIntersection(iA1, iA2, lineA, zA, 0.6f);

                // Giao tuyến b với Beta (Y = -0.4)
                float zB = -1.5f + (0.6f - gammaYOffset) * 1.5f;
                UpdateIntersection(iB1, iB2, lineB, zB, -0.4f);
            });

        yield return new WaitForSeconds(4.0f);

        // Diegetic text a // b - giữa 2 mặt phẳng, tâm Z = 0
        GameObject labelContainer = Fix(new GameObject("DiegeticText"));
        labelContainer.transform.localPosition = new Vector3(0, 0.1f, 0f);
        GeoFactory.CreateLabel(labelContainer.transform, "a // b", highlightColor);
        labelContainer.transform.localScale = Vector3.zero;
        labelContainer.transform.DOScale(1.5f, 1f).SetEase(Ease.OutBack);
    }

    void UpdateIntersection(GameObject p1, GameObject p2, GameObject line, float z, float y)
    {
        // Ranh giới Z: -1.5 đến 1.5
        if (z >= -1.5f && z <= 1.5f) {
            p1.transform.position = transform.TransformPoint(new Vector3(-2f, y, z));
            p2.transform.position = transform.TransformPoint(new Vector3( 2f, y, z));
            p1.transform.localScale = Vector3.one * 0.04f;
            p2.transform.localScale = Vector3.one * 0.04f;
            line.SetActive(true);
        } else {
            p1.transform.localScale = Vector3.zero;
            p2.transform.localScale = Vector3.zero;
            line.SetActive(false);
        }
    }

    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }
}
