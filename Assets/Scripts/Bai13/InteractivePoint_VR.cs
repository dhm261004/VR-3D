using UnityEngine;
using UnityEngine.InputSystem;
// Khai báo thêm thư viện XR
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Dòng này ÉP Unity phải tự động thêm XRGrabInteractable và Rigidbody vào object
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class InteractivePoint_VR : MonoBehaviour
{
    private Camera _cam;
    private float _zDistance;
    private bool _isDragging = false;

    private Rigidbody _rb;
    private XRGrabInteractable _grabInteractable;

    void Start()
    {
        _cam = Camera.main;
        
        // Cấu hình vật lý cơ bản để VR có thể cầm được nhưng không bị rớt
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true; 

        // Móc sự kiện VR tự động
        _grabInteractable = GetComponent<XRGrabInteractable>();
        if (_grabInteractable != null)
        {
            // Bắt sự kiện khi tay cầm kích hoạt (bóp cò) vào vật thể này trong lúc đang cầm
            _grabInteractable.activated.AddListener(OnVRActivated);
        }
    }

    void OnDestroy()
    {
        if (_grabInteractable != null)
        {
            _grabInteractable.activated.RemoveListener(OnVRActivated);
        }
    }

    void Update()
    {
        // ==========================================
        // PHẦN NÀY CHỈ CHẠY TRÊN PC (DÙNG CHUỘT)
        // Khi đeo kính VR, phần này sẽ tự động bị bỏ qua
        // ==========================================
        // Ngăn xung đột: Nếu tay cầm VR đang cầm vật thể này, bỏ qua xử lý chuột
        if (_grabInteractable != null && _grabInteractable.isSelected) 
        {
            _isDragging = false; // Xoá trạng thái kéo chuột để tránh bị kẹt khi thả VR ra
            return;
        }

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
        if (Mouse.current.leftButton.wasReleasedThisFrame || !Mouse.current.leftButton.isPressed)
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
    private void OnVRActivated(ActivateEventArgs args)
    {
        OnVRSelected();
    }

    public void OnVRSelected()
    {
        // MeasurementManager.Instance.SelectPointForMeasurement(this.gameObject);
        Debug.Log("Đã chọn điểm để đo đạc (Bằng VR Laser/Trigger)!");
    }
}