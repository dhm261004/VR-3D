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

    /// <summary>
    /// True khi tay VR đang cầm vật thể này.
    /// Module scripts dùng để tạm dừng constraint Update() tránh xung đột với XR.
    /// </summary>
    public bool IsGrabbed => _grabInteractable != null && _grabInteractable.isSelected;

    void Start()
    {
        _cam = Camera.main;
        
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        // isKinematic = FALSE để XRGrabInteractable có thể di chuyển vật thể
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // Khoá xoay để vật thể không lăn lung tung
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _grabInteractable = GetComponent<XRGrabInteractable>();
        if (_grabInteractable != null)
        {
            // Dùng Kinematic movement type: XR Toolkit tự tắt/bật isKinematic khi grab
            _grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
            _grabInteractable.trackPosition = true;
            _grabInteractable.trackRotation = false; // Không xoay theo tay cầm
            _grabInteractable.throwOnDetach = false; // Không bắn vật thể khi thả

            _grabInteractable.activated.AddListener(OnVRActivated);
            // Fix stickiness bug: khi nhả grab, đảm bảo Rigidbody reset đúng cách
            _grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDestroy()
    {
        if (_grabInteractable != null)
        {
            _grabInteractable.activated.RemoveListener(OnVRActivated);
            _grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    /// <summary>
    /// Fix stickiness bug: Khi XR Device Simulator nhả grab (Shift+G lần 2),
    /// đảm bảo Rigidbody không còn bám theo tay cầm nữa.
    /// </summary>
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Đảm bảo isKinematic = false và velocity = 0 sau khi nhả
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        _isDragging = false;
    }

    void Update()
    {
        // Nếu VR đang cầm → bỏ qua xử lý chuột
        if (IsGrabbed)
        {
            _isDragging = false;
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

        // 4. KÉO THẢ BẰNG CHUỘT - dùng MovePosition tương thích Rigidbody
        if (_isDragging)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenPosition = new Vector3(mousePos.x, mousePos.y, _zDistance);
            _rb.MovePosition(_cam.ScreenToWorldPoint(screenPosition));
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