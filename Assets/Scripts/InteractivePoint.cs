using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có thư viện này

public class InteractivePoint : MonoBehaviour
{
    private Camera _cam;
    private float _zDistance;
    private bool _isDragging = false;

    void Start() => _cam = Camera.main;

    void Update()
    {
        // Nếu không có chuột kết nối thì bỏ qua
        if (Mouse.current == null) return;

        // 1. CHUỘT PHẢI (Kích hoạt Thước đo)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (IsPointerOverMe()) {
                MeasurementManager.Instance.SelectPointForMeasurement(this.gameObject);
            }
        }

        // 2. CHUỘT TRÁI (Nhấn xuống để bắt đầu kéo)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverMe()) {
                _isDragging = true;
                _zDistance = _cam.WorldToScreenPoint(transform.position).z;
            }
        }

        // 3. THẢ CHUỘT TRÁI (Dừng kéo)
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        // 4. KÉO THẢ (Cập nhật vị trí liên tục)
        if (_isDragging)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenPosition = new Vector3(mousePos.x, mousePos.y, _zDistance);
            transform.position = _cam.ScreenToWorldPoint(screenPosition);
        }
    }

    // Hàm tự định nghĩa để kiểm tra xem chuột có đang trỏ vào khối này không
    private bool IsPointerOverMe()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Trả về true nếu tia Raycast chạm trúng chính object này
            return hit.transform == transform;
        }
        return false;
    }
}