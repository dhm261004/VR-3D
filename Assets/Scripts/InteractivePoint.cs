using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


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
        
        
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true; 
    }

    void Update()
    {
        
        
        
        
        if (Mouse.current == null) return;

        
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (IsPointerOverMe()) {
                
                Debug.Log("Đã chọn điểm để đo đạc (Bằng chuột)!");
            }
        }

        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverMe()) {
                _isDragging = true;
                _zDistance = _cam.WorldToScreenPoint(transform.position).z;
            }
        }

        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        
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

    
    
    
    
    
    public void OnVRSelected()
    {
        
        Debug.Log("Đã chọn điểm để đo đạc (Bằng VR Laser)!");
    }
}