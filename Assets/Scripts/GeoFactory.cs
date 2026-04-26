using UnityEngine;
using System.Collections.Generic;
using TMPro;

public static class GeoFactory
{
    private static Dictionary<string, Material> _mats = new Dictionary<string, Material>();
    
    
    private static Dictionary<string, Transform> _groups = new Dictionary<string, Transform>();

    
    private static Transform GetGroup(string groupName) {
        if (!_groups.ContainsKey(groupName) || _groups[groupName] == null) {
            GameObject g = GameObject.Find(groupName);
            if (g == null) g = new GameObject(groupName);
            _groups[groupName] = g.transform;
        }
        return _groups[groupName];
    }

    private static Material GetMat(Color c, bool emit = false, float a = 1f) {
        string key = $"{c}_{emit}_{a}";
        if (!_mats.ContainsKey(key)) {
            Shader s = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Transparent");
            if (s == null) s = Shader.Find("Standard");

            Material m = new Material(s);
            Color finalC = new Color(c.r, c.g, c.b, a);
            
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", finalC);
            if (m.HasProperty("_Color")) m.SetColor("_Color", finalC);

            if (emit) {
                m.EnableKeyword("_EMISSION");
                if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", c * 2.5f); 
            }

            if (a < 1f) {
                m.SetFloat("_Surface", 1);
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0); 
                if (m.HasProperty("_Cull")) m.SetInt("_Cull", 0);
                m.renderQueue = 3000;
            }
            _mats[key] = m;
        }
        return _mats[key];
    }

    public static GameObject CreatePoint(Vector3 pos, Color c, string name, bool isDraggable = false) {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube); 
        p.name = name; 
        p.transform.position = pos; 
        p.transform.localScale = Vector3.zero; 
        p.GetComponent<Renderer>().sharedMaterial = GetMat(c, true);
        
        
        p.transform.SetParent(GetGroup("[POINTS]"));

        if (isDraggable) {
            p.AddComponent<InteractivePoint>();
            p.GetComponent<BoxCollider>().size = Vector3.one * 4f; 
        }

        CreateLabel(p.transform, name, new Color32(255, 255, 255, 255)); 
        return p;
    }

    public static GameObject CreatePointVR(Vector3 pos, Color c, string name, bool isDraggable = false) {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube); 
        p.name = name; 
        p.transform.position = pos; 
        p.transform.localScale = Vector3.zero; 
        p.GetComponent<Renderer>().sharedMaterial = GetMat(c, true);
        
        
        p.transform.SetParent(GetGroup("[POINTS]"));

        if (isDraggable) {
            p.AddComponent<InteractivePoint_VR>();
            p.GetComponent<BoxCollider>().size = Vector3.one * 4f; 
        }

        CreateLabel(p.transform, name, new Color32(255, 255, 255, 255)); 
        return p;
    }

    public static GameObject CreateLine(GameObject p1, GameObject p2, Color c, float w = 0.015f) {
        GameObject l = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.Destroy(l.GetComponent<Collider>()); 
        l.GetComponent<Renderer>().sharedMaterial = GetMat(c, true); 
        
        l.transform.localScale = new Vector3(w, 0, w); 
        
        
        l.transform.SetParent(GetGroup("[EDGES]"));
        
        var f = l.AddComponent<EdgeFollower>();
        f.p1 = p1.transform; f.p2 = p2.transform; f.width = w;
        
        return l;
    }

    public static GameObject CreateFace(GameObject[] pts, Color c) {
        GameObject f = new GameObject("DynamicFace");
        f.AddComponent<MeshFilter>().mesh = new Mesh();
        
        Material mat = GetMat(c, false, 0f); 
        f.AddComponent<MeshRenderer>().sharedMaterial = mat;
        
        
        f.transform.SetParent(GetGroup("[FACES]"));
        
        var fFollow = f.AddComponent<FaceFollower>();
        fFollow.targets = new Transform[pts.Length];
        for (int i = 0; i < pts.Length; i++) fFollow.targets[i] = pts[i].transform;
        
        return f;
    }

    public static void CreateLabel(Transform target, string txt, Color c) {
        GameObject l = new GameObject("Label_" + txt);
        
        
        l.transform.SetParent(GetGroup("[LABELS]"));
        
        var follower = l.AddComponent<PositionFollower>();
        follower.target = target;
        follower.offset = Vector3.up * 0.15f; 
        
        TextMeshPro tm = l.AddComponent<TextMeshPro>();
        tm.text = txt; tm.color = c; 
        tm.fontSize = 2.5f; tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        
        l.AddComponent<Billboard>(); 
    }

    public static GameObject CreateMeasure(GameObject p1, GameObject p2, Color c) {
        GameObject m = new GameObject($"Measure_{p1.name}_{p2.name}");
        
        
        m.transform.SetParent(GetGroup("[MEASURES]"));
        
        GameObject line = CreateLine(p1, p2, c, 0.003f); 
        line.transform.SetParent(m.transform); 
        line.GetComponent<EdgeFollower>().isAnimating = false; 

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(m.transform);
        
        TextMeshPro tm = textObj.AddComponent<TextMeshPro>();
        tm.color = c; tm.fontSize = 1.5f; tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        
        textObj.AddComponent<Billboard>();

        var mf = m.AddComponent<MeasureFollower>();
        mf.p1 = p1.transform; mf.p2 = p2.transform; 
        mf.tm = tm; mf.textTransform = textObj.transform;
        
        return m;
    }
}