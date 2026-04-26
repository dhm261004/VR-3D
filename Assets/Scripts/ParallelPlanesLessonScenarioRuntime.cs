using System.Collections;
using UnityEngine;

public class ParallelPlanesLessonScenarioRuntime : MonoBehaviour
{
    [SerializeField] private LessonUIWorldPanel uiPanel;
    [SerializeField] private Transform workspaceRoot;
    [SerializeField] private Vector3 workspaceOffset = new Vector3(1.6f, 0f, 0f);
    [SerializeField] private GameObject[] lessonPrefabs = new GameObject[5];
    [SerializeField] private float[] lessonScaleMultipliers = new float[] { 1f, 100f, 1f, 1f, 1f };
    [SerializeField] private bool destroyCurrentLessonOnSwitch = true;
    [SerializeField] private float navigationCooldownSeconds = 0.2f;
    [SerializeField] private bool logLessonLifecycle = true;

    private int currentModuleIndex = -1;
    private GameObject currentLessonInstance;
    private bool didBindButtons;
    private float lastNavigationTime = -999f;

    private void Start()
    {
        if (workspaceRoot == null) workspaceRoot = transform;
        if (uiPanel == null) uiPanel = FindFirstObjectByType<LessonUIWorldPanel>();
        if (uiPanel == null)
        {
            Debug.LogError("ParallelPlanesLessonScenarioRuntime: Missing LessonUIWorldPanel reference.");
            return;
        }

        int lessonCount = LessonContentCatalog.LessonCount;
        if (lessonCount == 0)
        {
            uiPanel.SetHeader(LessonContentCatalog.LessonTitle, "Chưa có bài học");
            uiPanel.SetContent("Catalog đang trống. Hãy thêm dữ liệu bài học trong LessonContentCatalog.Lessons.");
            uiPanel.SetNavigationState(false, false);
            return;
        }

        uiPanel.SetHeader(LessonContentCatalog.LessonTitle, LessonContentCatalog.GetLesson(0).Title);
        uiPanel.SetModuleTitleLabels(LessonContentCatalog.LessonLabels);
        uiPanel.SetActiveModule(0);
        TryBindButtons();
        uiPanel.SetNavigationState(false, lessonCount > 1);
        ShowModule(0);
    }

    private void OnEnable()
    {
        TryBindButtons();
    }

    private void TryBindButtons()
    {
        if (didBindButtons || uiPanel == null) return;
        uiPanel.BindNavigationButtons(OnPreviousButtonPressed, OnNextButtonPressed);
        didBindButtons = true;
    }

    
    public void OnPreviousButtonPressed() => GoToPreviousModule();
    public void OnNextButtonPressed() => GoToNextModule();

    public void GoToPreviousModule()
    {
        if (!CanNavigateNow()) return;
        ShowModule(currentModuleIndex - 1);
    }

    public void GoToNextModule()
    {
        if (!CanNavigateNow()) return;
        ShowModule(currentModuleIndex + 1);
    }

    private bool CanNavigateNow()
    {
        if (Time.unscaledTime - lastNavigationTime < navigationCooldownSeconds) return false;
        lastNavigationTime = Time.unscaledTime;
        return true;
    }

    private void ShowModule(int idx)
    {
        int lessonCount = LessonContentCatalog.LessonCount;
        if (lessonCount == 0) return;

        int clamped = Mathf.Clamp(idx, 0, lessonCount - 1);
        if (clamped == currentModuleIndex && currentLessonInstance != null) return;
        currentModuleIndex = clamped;
        ClearWorkspace();

        LessonContentCatalog.LessonEntry lesson = LessonContentCatalog.GetLesson(currentModuleIndex);
        uiPanel.SetHeader(LessonContentCatalog.LessonTitle, lesson.Title);
        uiPanel.SetContent(lesson.Description);
        uiPanel.SetActiveModule(currentModuleIndex);
        uiPanel.SetNavigationState(currentModuleIndex > 0, currentModuleIndex < lessonCount - 1);

        SpawnLessonPrefab(currentModuleIndex);
        if (logLessonLifecycle)
            Debug.Log($"[LessonMenu] Active lesson index = {currentModuleIndex}, title = {lesson.Title}");
    }

    private void SpawnLessonPrefab(int index)
    {
        if (lessonPrefabs == null || index < 0 || index >= lessonPrefabs.Length)
        {
            if (logLessonLifecycle)
                Debug.LogWarning($"[LessonMenu] Missing prefab mapping for lesson index {index}.");
            return;
        }

        GameObject prefab = lessonPrefabs[index];
        if (prefab == null)
        {
            if (logLessonLifecycle)
                Debug.LogWarning($"[LessonMenu] lessonPrefabs[{index}] is null.");
            return;
        }

        Vector3 spawnPosition = workspaceRoot.position + workspaceOffset;
        Quaternion spawnRotation = workspaceRoot.rotation;
        currentLessonInstance = Instantiate(prefab, spawnPosition, spawnRotation);
        if (workspaceRoot != null)
            currentLessonInstance.transform.SetParent(workspaceRoot, true);

        float scaleMultiplier = GetScaleMultiplier(index);
        if (!Mathf.Approximately(scaleMultiplier, 1f))
            currentLessonInstance.transform.localScale *= scaleMultiplier;

        currentLessonInstance.SetActive(true);
        currentLessonInstance.name = prefab.name + "_Runtime";
        StartCoroutine(ApplyScaleNextFrame(currentLessonInstance, index));
        if (logLessonLifecycle)
            Debug.Log($"[LessonMenu] Spawned {prefab.name} with scale multiplier {scaleMultiplier} at index {index}");
    }

    private float GetScaleMultiplier(int index)
    {
        if (lessonScaleMultipliers == null || index < 0 || index >= lessonScaleMultipliers.Length) return 1f;
        return lessonScaleMultipliers[index] <= 0f ? 1f : lessonScaleMultipliers[index];
    }

    private IEnumerator ApplyScaleNextFrame(GameObject instance, int index)
    {
        yield return null;
        if (instance == null) yield break;

        float scaleMultiplier = GetScaleMultiplier(index);
        if (Mathf.Approximately(scaleMultiplier, 1f)) yield break;
        instance.transform.localScale *= scaleMultiplier;
    }

    private void ClearWorkspace()
    {
        if (!destroyCurrentLessonOnSwitch) return;
        if (currentLessonInstance != null) Destroy(currentLessonInstance);
        currentLessonInstance = null;
    }

    private void OnValidate()
    {
        if (lessonScaleMultipliers == null || lessonScaleMultipliers.Length != LessonContentCatalog.LessonCount)
        {
            lessonScaleMultipliers = new float[LessonContentCatalog.LessonCount];
            for (int i = 0; i < lessonScaleMultipliers.Length; i++) lessonScaleMultipliers[i] = 1f;
        }
    }
}
