using UnityEngine;

public class Bai14ScenarioRuntime : MonoBehaviour
{
    [SerializeField] private LessonUIWorldPanel uiPanel;
    [SerializeField] private Transform workspaceRoot;
    [SerializeField] private Vector3 workspaceOffset = Vector3.zero;
    [SerializeField] private GameObject[] lessonPrefabs = new GameObject[3];
    [SerializeField] private bool destroyCurrentLessonOnSwitch = true;
    [SerializeField] private float navigationCooldownSeconds = 0.2f;

    private int _currentIndex = -1;
    private GameObject _currentInstance;
    private bool _didBind;
    private float _lastNavTime = -999f;

    private void Start()
    {
        if (workspaceRoot == null) workspaceRoot = transform;
        if (uiPanel == null) uiPanel = FindFirstObjectByType<LessonUIWorldPanel>();
        if (uiPanel == null) { Debug.LogError("[Bai14] Missing LessonUIWorldPanel."); return; }

        uiPanel.SetHeader(Bai14ContentCatalog.LessonTitle, Bai14ContentCatalog.GetLesson(0).Title);
        uiPanel.SetModuleTitleLabels(Bai14ContentCatalog.LessonLabels);
        uiPanel.SetActiveModule(0);
        TryBindButtons();
        uiPanel.SetNavigationState(false, Bai14ContentCatalog.LessonCount > 1);
        ShowModule(0);
    }

    private void OnEnable() => TryBindButtons();

    private void TryBindButtons()
    {
        if (_didBind || uiPanel == null) return;
        uiPanel.BindNavigationButtons(OnPrevPressed, OnNextPressed);
        _didBind = true;
    }

    public void OnPrevPressed() { if (CanNav()) ShowModule(_currentIndex - 1); }
    public void OnNextPressed() { if (CanNav()) ShowModule(_currentIndex + 1); }

    private bool CanNav()
    {
        if (Time.unscaledTime - _lastNavTime < navigationCooldownSeconds) return false;
        _lastNavTime = Time.unscaledTime;
        return true;
    }

    private void ShowModule(int idx)
    {
        int count = Bai14ContentCatalog.LessonCount;
        int clamped = Mathf.Clamp(idx, 0, count - 1);
        if (clamped == _currentIndex && _currentInstance != null) return;
        _currentIndex = clamped;

        if (destroyCurrentLessonOnSwitch && _currentInstance != null)
        {
            Destroy(_currentInstance);
            _currentInstance = null;
        }

        var lesson = Bai14ContentCatalog.GetLesson(_currentIndex);
        uiPanel.SetHeader(Bai14ContentCatalog.LessonTitle, lesson.Title);
        uiPanel.SetContent(lesson.Description);
        uiPanel.SetActiveModule(_currentIndex);
        uiPanel.SetNavigationState(_currentIndex > 0, _currentIndex < count - 1);

        if (lessonPrefabs != null && _currentIndex < lessonPrefabs.Length
            && lessonPrefabs[_currentIndex] != null)
        {
            Vector3 pos = workspaceRoot.position + workspaceOffset;
            _currentInstance = Instantiate(lessonPrefabs[_currentIndex], pos, workspaceRoot.rotation);
            _currentInstance.transform.SetParent(workspaceRoot, true);
            _currentInstance.name = lessonPrefabs[_currentIndex].name + "_Runtime";
        }
        else
        {
            Debug.LogWarning($"[Bai14] lessonPrefabs[{_currentIndex}] chưa gán.");
        }
    }
}
