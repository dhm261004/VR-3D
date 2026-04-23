using UnityEngine;
using UnityEngine.InputSystem;
// Khai báo thêm thư viện XR
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Dòng này ÉP Unity phải tự động thêm XRGrabInteractable và Rigidbody vào object
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class InteractivePoint : MonoBehaviour
{
    private Camera _cam;
    private float _zDistance;
    private bool _isDragging = false;

    private Rigidbody _rb;

    void Start()
    {
        _cam = Camera.main;
        
        // Cấu hình vật lý cơ bản để VR có thể cầm được nhưng không bị rớt
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true; 
    }

    void Update()
    {
        // ==========================================
        // PHẦN NÀY CHỈ CHẠY TRÊN PC (DÙNG CHUỘT)
        // Khi đeo kính VR, phần này sẽ tự động bị bỏ qua
        // ==========================================
        if (Mouse.current == null) return;

        // 1. CHUỘT PHẢI (Kích hoạt Thước đo)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (IsPointerOverMe()) {
                // MeasurementManager.Instance.SelectPointForMeasurement(this.gameObject);
                Debug.Log("Đã chọn điểm để đo đạc (Bằng chuột)!");
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

        // 4. KÉO THẢ BẰNG CHUỘT
        if (_isDragging)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenPosition = new Vector3(mousePos.x, mousePos.y, _zDistance);
            transform.position = _cam.ScreenToWorldPoint(screenPosition);
        }
    }

    private bool IsPointerOverMe()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == transform;
        }
        return false;
    }

    // ==========================================
    // PHẦN DÀNH CHO VR (TÍCH HỢP ĐO ĐẠC)
    // ==========================================
    // Vì VR không có "Chuột Phải", nên khi tay cầm VR bắn tia laser vào và bóp cò, 
    // XR Toolkit sẽ tự gọi hàm này nếu bạn móc nó vào sự kiện SelectEntered.
    public void OnVRSelected()
    {
        // MeasurementManager.Instance.SelectPointForMeasurement(this.gameObject);
        Debug.Log("Đã chọn điểm để đo đạc (Bằng VR Laser)!");
    }
}