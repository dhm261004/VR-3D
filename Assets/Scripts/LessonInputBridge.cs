using UnityEngine;
using UnityEngine.InputSystem;



public class LessonInputBridge : MonoBehaviour
{
    public static bool NextPressed { get; private set; }
    public static bool ResetPressed { get; private set; }

    private InputAction _next;
    private InputAction _reset;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<LessonInputBridge>() == null)
            new GameObject("[LessonInputBridge]").AddComponent<LessonInputBridge>();
    }

    void Awake()
    {
        
        _next = new InputAction("LessonNext");
        _next.AddBinding("<Keyboard>/space");
        _next.AddBinding("<XRController>{RightHand}/primaryButton");
        _next.AddBinding("<XRController>{LeftHand}/primaryButton");
        _next.Enable();

        
        _reset = new InputAction("LessonReset");
        _reset.AddBinding("<Keyboard>/r");
        _reset.AddBinding("<XRController>{RightHand}/secondaryButton");
        _reset.AddBinding("<XRController>{LeftHand}/secondaryButton");
        _reset.Enable();
    }

    void Update()
    {
        NextPressed = _next.WasPressedThisFrame();
        ResetPressed = _reset.WasPressedThisFrame();
    }

    void OnDestroy()
    {
        _next?.Disable();
        _reset?.Disable();
    }
}
