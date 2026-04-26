using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class InteractivePoint_VR : MonoBehaviour
{
    private Camera _cam;
    private float _zDistance;
    private bool _isDragging = false;

    private Rigidbody _rb;
    private XRGrabInteractable _grabInteractable;

    
    
    
    
    public bool IsGrabbed => _grabInteractable != null && _grabInteractable.isSelected;

    void Start()
    {
        _cam = Camera.main;
        
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _grabInteractable = GetComponent<XRGrabInteractable>();
        if (_grabInteractable != null)
        {
            
            _grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
            _grabInteractable.trackPosition = true;
            _grabInteractable.trackRotation = false; 
            _grabInteractable.throwOnDetach = false; 

            _grabInteractable.activated.AddListener(OnVRActivated);
            
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

    
    
    
    
    private void OnSelectExited(SelectExitEventArgs args)
    {
        
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
        
        if (IsGrabbed)
        {
            _isDragging = false;
            return;
        }

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

        
        if (Mouse.current.leftButton.wasReleasedThisFrame || !Mouse.current.leftButton.isPressed)
        {
            _isDragging = false;
        }

        
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

    
    
    
    
    
    private void OnVRActivated(ActivateEventArgs args)
    {
        OnVRSelected();
    }

    public void OnVRSelected()
    {
        
        Debug.Log("Đã chọn điểm để đo đạc (Bằng VR Laser/Trigger)!");
    }
}