using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class module4_lythuyet : MonoBehaviour
{
    private TextMeshPro uiStatusText;

    IEnumerator Start()
    {
        if (Camera.main) Camera.main.backgroundColor = new Color32(10, 10, 12, 255);

        Color alphaColor     = new Color32(0, 150, 255, 100);
        Color betaColor      = new Color32(0, 255, 150, 100);
        Color highlightColor = new Color32(255, 255, 0, 255);
        Color prismColor     = new Color32(255, 100, 200, 150);
        Color transparentGlass = new Color32(100, 200, 255, 60);

        // 2 Mặt phẳng song song - nâng để Q không bị sàn che
        // P: Y = 2.0, Q: Y = 0.0
        CreatePlane(2.0f, alphaColor, "(P)");
        CreatePlane(0.0f, betaColor, "(Q)");
        
        yield return new WaitForSeconds(1.0f);

        // Tam giác đều ở đáy (Y = 0.0, tâm Z = 0)
        Vector3 center = new Vector3(0, 0.0f, 0f);
        float R = 1.5f;
        Vector3 posA = center + new Vector3(0, 0, R);
        Vector3 posB = center + new Vector3(R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));
        Vector3 posC = center + new Vector3(-R * Mathf.Cos(Mathf.PI/6), 0, -R * Mathf.Sin(Mathf.PI/6));

        GameObject pA = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posA), highlightColor, "A", false));
        GameObject pB = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posB), highlightColor, "B", false));
        GameObject pC = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posC), highlightColor, "C", false));
        pA.transform.DOScale(0.05f, 0.3f);
        pB.transform.DOScale(0.05f, 0.3f);
        pC.transform.DOScale(0.05f, 0.3f);

        yield return new WaitForSeconds(0.5f);
        GameObject edgeAB = Fix(GeoFactory.CreateLine(pA, pB, highlightColor, 0.02f));
        GameObject edgeBC = Fix(GeoFactory.CreateLine(pB, pC, highlightColor, 0.02f));
        GameObject edgeCA = Fix(GeoFactory.CreateLine(pC, pA, highlightColor, 0.02f));
        edgeAB.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBC.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCA.GetComponent<EdgeFollower>().isAnimating = false;

        yield return new WaitForSeconds(1f);

        // Đáy A'B'C' bắt đầu cùng vị trí với ABC
        GameObject pA_prime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posA), highlightColor, "A'", false));
        GameObject pB_prime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posB), highlightColor, "B'", false));
        GameObject pC_prime = Fix(GeoFactory.CreatePoint(transform.TransformPoint(posC), highlightColor, "C'", false));
        pA_prime.transform.localScale = Vector3.one * 0.05f;
        pB_prime.transform.localScale = Vector3.one * 0.05f;
        pC_prime.transform.localScale = Vector3.one * 0.05f;

        GameObject edgeAprimeBprime = Fix(GeoFactory.CreateLine(pA_prime, pB_prime, highlightColor, 0.02f));
        GameObject edgeBprimeCprime = Fix(GeoFactory.CreateLine(pB_prime, pC_prime, highlightColor, 0.02f));
        GameObject edgeCprimeAprime = Fix(GeoFactory.CreateLine(pC_prime, pA_prime, highlightColor, 0.02f));
        edgeAprimeBprime.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBprimeCprime.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCprimeAprime.GetComponent<EdgeFollower>().isAnimating = false;

        // Cạnh bên
        GameObject edgeAA = Fix(GeoFactory.CreateLine(pA, pA_prime, prismColor, 0.025f));
        GameObject edgeBB = Fix(GeoFactory.CreateLine(pB, pB_prime, prismColor, 0.025f));
        GameObject edgeCC = Fix(GeoFactory.CreateLine(pC, pC_prime, prismColor, 0.025f));
        edgeAA.GetComponent<EdgeFollower>().isAnimating = false;
        edgeBB.GetComponent<EdgeFollower>().isAnimating = false;
        edgeCC.GetComponent<EdgeFollower>().isAnimating = false;

        // Mặt bên kính mờ
        Fix(GeoFactory.CreateFace(new GameObject[] { pA, pB, pB_prime, pA_prime }, transparentGlass));
        Fix(GeoFactory.CreateFace(new GameObject[] { pB, pC, pC_prime, pB_prime }, transparentGlass));
        Fix(GeoFactory.CreateFace(new GameObject[] { pC, pA, pA_prime, pC_prime }, transparentGlass));

        // Animation kéo đáy trên lên mặt Alpha
        Vector3 upVector = new Vector3(1.0f, 2.8f, 0.5f); // chạm mặt phẳng P (Y=2.0)
        pA_prime.transform.DOLocalMove(transform.InverseTransformPoint(transform.TransformPoint(posA) + upVector), 3f).SetEase(Ease.InOutQuad);
        pB_prime.transform.DOLocalMove(transform.InverseTransformPoint(transform.TransformPoint(posB) + upVector), 3f).SetEase(Ease.InOutQuad);
        pC_prime.transform.DOLocalMove(transform.InverseTransformPoint(transform.TransformPoint(posC) + upVector), 3f).SetEase(Ease.InOutQuad);


        yield return new WaitForSeconds(3.5f);
    }

    GameObject CreatePlane(float yPos, Color color, string label)
    {
        // Z: -2.0 đến 2.0 (tâm Z = 0)
        GameObject p1 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-3, yPos, -2.0f)), color, " ", false));
        GameObject p2 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 4, yPos, -2.0f)), color, " ", false));
        GameObject p3 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3( 4, yPos,  2.0f)), color, label, false));
        GameObject p4 = Fix(GeoFactory.CreatePoint(transform.TransformPoint(new Vector3(-3, yPos,  2.0f)), color, " ", false));
        p1.transform.localScale = p2.transform.localScale = p3.transform.localScale = p4.transform.localScale = Vector3.one * 0.04f;
        
        GameObject face = Fix(GeoFactory.CreateFace(new GameObject[] { p1, p2, p3, p4 }, color));
        Material mat = face.GetComponent<MeshRenderer>().sharedMaterial;
        if (mat.HasProperty("_BaseColor")) mat.DOFade(0.4f, "_BaseColor", 0.5f);
        else mat.DOFade(0.4f, "_Color", 0.5f);

        return face;
    }

    private GameObject Fix(GameObject obj)
    {
        if (obj != null) obj.transform.SetParent(this.transform);
        return obj;
    }
}
