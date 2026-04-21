using UnityEngine;
using System.Collections.Generic;

public class MeasurementManager : MonoBehaviour
{
    private static MeasurementManager _instance;
    public static MeasurementManager Instance {
        get {
            if (_instance == null) _instance = new GameObject("MeasurementManager").AddComponent<MeasurementManager>();
            return _instance;
        }
    }

    private List<GameObject> _selected = new List<GameObject>();
    private Color _measureColor = new Color32(255, 234, 0, 255); // Cyber Yellow

    public void SelectPointForMeasurement(GameObject pt) {
        if (!_selected.Contains(pt)) {
            _selected.Add(pt);
            pt.transform.localScale *= 1.3f; // Phóng to nhẹ để báo hiệu đã chọn
        }

        if (_selected.Count == 2) {
            // Khởi tạo thước đo giữa 2 điểm
            GeoFactory.CreateMeasure(_selected[0], _selected[1], _measureColor);
            
            // Trả lại kích thước ban đầu
            _selected[0].transform.localScale /= 1.3f;
            _selected[1].transform.localScale /= 1.3f;
            _selected.Clear();
        }
    }
}