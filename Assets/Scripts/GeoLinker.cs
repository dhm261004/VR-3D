using UnityEngine;
using TMPro;


public class EdgeFollower : MonoBehaviour {
    public Transform p1, p2; 
    public float width;
    public bool isAnimating = true; 

    void LateUpdate() {
        if (!p1 || !p2) return;
        Vector3 dir = p2.position - p1.position;
        transform.position = p1.position + dir / 2;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        
        
        if (!isAnimating) {
            transform.localScale = new Vector3(width, dir.magnitude / 2, width);
        }
    }
}


public class FaceFollower : MonoBehaviour {
    public Transform[] targets;
    private Mesh _m;
    
    void Start() => _m = GetComponent<MeshFilter>().mesh;

    void LateUpdate() {
        if (targets == null || targets.Length < 3) return;
        Vector3[] v = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++) v[i] = targets[i].position;
        
        _m.vertices = v;

        
        int[] t = new int[(targets.Length - 2) * 3];
        for (int i = 0; i < targets.Length - 2; i++) {
            t[i*3] = 0; t[i*3+1] = i+1; t[i*3+2] = i+2;
        }
        _m.triangles = t;
        
        _m.RecalculateNormals();
        _m.RecalculateBounds(); 
    }
}


public class MidpointFollower : MonoBehaviour {
    public Transform p1, p2; 
    public float ratio;
    void LateUpdate() { 
        if(p1 && p2) transform.position = Vector3.Lerp(p1.position, p2.position, ratio); 
    }
}


public class PositionFollower : MonoBehaviour {
    public Transform target;
    public Vector3 offset;
    void LateUpdate() {
        if (target) transform.position = target.position + offset;
        else Destroy(gameObject); 
    }
}


public class MeasureFollower : MonoBehaviour {
    public Transform p1, p2; 
    public TextMeshPro tm;
    public Transform textTransform;
    
    void LateUpdate() {
        if(!p1 || !p2 || !tm) { 
            if(gameObject) Destroy(gameObject); 
            return; 
        }

        
        if(textTransform) {
            textTransform.position = (p1.position + p2.position) / 2 + Vector3.up * 0.1f;
        }

        
        float dist = Vector3.Distance(p1.position, p2.position);
        tm.text = $"{dist:F2}cm";
    }
}