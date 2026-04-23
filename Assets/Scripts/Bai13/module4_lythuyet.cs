using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module4_lythuyet : MonoBehaviour
{
    private TextMeshPro uiStatusText;

    IEnumerator Start()
    {
        // 1. Nền không gian
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor = new Color32(0, 150, 255, 100);
        Color betaColor = new Color32(0, 255, 150, 100);
        Color highlightColor = new Color32(255, 255, 0, 255);
        Color prismColor = new Color32(255, 100, 200, 150);
        Color transparentGlass = new Color32(100, 200, 255, 60);

        // 2. Khởi tạo 2 Mặt phẳng song song
        // Alpha: Y = 1.2, Beta: Y = -0.8
        CreatePlane(1.2f, alphaColor, "(P)");
        CreatePlane(-0.8f, betaColor, "(Q)");
        
        yield return new WaitForSeconds(1.0f);

        // 3. Vẽ đa giác đều (tam giác đều) ở đáy dưới
        Vector3 center = new Vector3(0, -0.8f, 2.5f);
        float R = 1.5f;
        Vector3 posA = center + new Vector3(0, 0, R);
        Vector3 posB = center + new Vector3(R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        Vector3 posC = center + new Vector3(-R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));

        GameObject pA = GeoFactory.CreatePoint(posA, highlightColor, "A", false);
        GameObject pB = GeoFactory.CreatePoint(posB, highlightColor, "B", false);
        GameObject pC = GeoFactory.CreatePoint(posC, highlightColor, "C", false);
        pA.transform.DOScale(0.05f, 0.3f);
        pB.transform.DOScale(0.05f, 0.3f);
        pC.transform.DOScale(0.05f, 0.3f);

        yield return new WaitForSeconds(0.5f);
        GameObject edgeAB = GeoFactory.CreateLine(pA, pB, highlightColor, 0.02f);
        GameObject edgeBC = GeoFactory.CreateLine(pB, pC, highlightColor, 0.02f);
        GameObject edgeCA = GeoFactory.CreateLine(pC, pA, highlightColor, 0.02f);
        edgeAB.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBC.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCA.GetComponent<EdgeFollower>().isAnimating = false;

        yield return new WaitForSeconds(1f);

        // 4. Tạo đáy A'B'C' bằng cách copy ABC tại chỗ
        GameObject pA_prime = GeoFactory.CreatePoint(posA, highlightColor, "A'", false);
        GameObject pB_prime = GeoFactory.CreatePoint(posB, highlightColor, "B'", false);
        GameObject pC_prime = GeoFactory.CreatePoint(posC, highlightColor, "C'", false);
        pA_prime.transform.localScale = Vector3.one * 0.05f;
        pB_prime.transform.localScale = Vector3.one * 0.05f;
        pC_prime.transform.localScale = Vector3.one * 0.05f;

        GameObject edgeAprimeBprime = GeoFactory.CreateLine(pA_prime, pB_prime, highlightColor, 0.02f);
        GameObject edgeBprimeCprime = GeoFactory.CreateLine(pB_prime, pC_prime, highlightColor, 0.02f);
        GameObject edgeCprimeAprime = GeoFactory.CreateLine(pC_prime, pA_prime, highlightColor, 0.02f);
        edgeAprimeBprime.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBprimeCprime.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCprimeAprime.GetComponent<EdgeFollower>().isAnimating = false;

        // 5. Nối các cạnh bên
        GameObject edgeAA = GeoFactory.CreateLine(pA, pA_prime, prismColor, 0.025f);
        GameObject edgeBB = GeoFactory.CreateLine(pB, pB_prime, prismColor, 0.025f);
        GameObject edgeCC = GeoFactory.CreateLine(pC, pC_prime, prismColor, 0.025f);
        edgeAA.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBB.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCC.GetComponent<EdgeFollower>().isAnimating = false;

        // 6. Tạo các mặt bên bằng vật liệu kính mờ (transparent)
        GameObject faceABB = GeoFactory.CreateFace(new GameObject[] { pA, pB, pB_prime, pA_prime }, transparentGlass);
        GameObject faceBCC = GeoFactory.CreateFace(new GameObject[] { pB, pC, pC_prime, pB_prime }, transparentGlass);
        GameObject faceCAA = GeoFactory.CreateFace(new GameObject[] { pC, pA, pA_prime, pC_prime }, transparentGlass);

        // 7. Animation dựng các cạnh bên bằng cách tịnh tiến mặt đáy trên lên mặt phẳng alpha
        Vector3 upVector = new Vector3(1.0f, 2.0f, 0.5f); // Kéo chéo lên
        pA_prime.transform.DOMove(posA + upVector, 3f).SetEase(Ease.InOutQuad);
        pB_prime.transform.DOMove(posB + upVector, 3f).SetEase(Ease.InOutQuad);
        pC_prime.transform.DOMove(posC + upVector, 3f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(3.5f);
    }

    GameObject CreatePlane(float yPos, Color color, string label)
    {
        GameObject p1 = GeoFactory.CreatePoint(new Vector3(-3, yPos, 1.0f), color, " ", false);
        GameObject p2 = GeoFactory.CreatePoint(new Vector3( 4, yPos, 1.0f), color, " ", false);
        GameObject p3 = GeoFactory.CreatePoint(new Vector3( 4, yPos, 5.0f), color, label, false);
        GameObject p4 = GeoFactory.CreatePoint(new Vector3(-3, yPos, 5.0f), color, " ", false);
        p1.transform.localScale = p2.transform.localScale = p3.transform.localScale = p4.transform.localScale = Vector3.one * 0.04f;
        
        GameObject face = GeoFactory.CreateFace(new GameObject[] { p1, p2, p3, p4 }, color);
        Material mat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (mat.HasProperty("_BaseColor")) mat.DOFade(0.4f, "_BaseColor", 0.5f);
        else mat.DOFade(0.4f, "_Color", 0.5f);

        return face;
    }
}
