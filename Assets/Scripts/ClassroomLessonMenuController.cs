using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClassroomLessonMenuController : MonoBehaviour
{
    [Serializable]
    public class LessonItem
    {
        public string lessonName = "Tiet 1";
        public Sprite contentImage;
        public GameObject modelPrefab;
    }

    [Serializable]
    public class ChapterItem
    {
        public string chapterName = "Bai 10";
        public LessonItem[] lessons = Array.Empty<LessonItem>();
    }

    [Serializable]
    private struct SelectionColors
    {
        public Color activeBackground;
        public Color inactiveBackground;
        public Color activeText;
        public Color inactiveText;
    }

    [Header("Catalog Data")]
    [SerializeField] private ChapterItem[] chapters = Array.Empty<ChapterItem>();
    [SerializeField] private bool forceStandardNavbarLabels = true;
    [SerializeField] private int firstChapterNumber = 10;

    [Header("Left Navbar - Chapter")]
    [SerializeField] private Transform navbarTreeRoot;
    [SerializeField] private Button chapterButtonTemplate;

    [Header("Left Navbar - Lesson (Tree Child)")]
    [SerializeField] private Button lessonButtonTemplate;
    [SerializeField] private bool onlyOneChapterExpanded = true;
    [SerializeField] private bool autoSelectFirstLessonOnChapterClick = true;
    [SerializeField] private float lessonIndentLeft = 24f;
    [SerializeField] private float fallbackChapterButtonHeight = 42f;
    [SerializeField] private float fallbackLessonButtonHeight = 34f;

    [Header("Main Content (Right)")]
    [SerializeField] private TMP_Text chapterTitleText;
    [SerializeField] private TMP_Text lessonTitleText;
    [SerializeField] private Image lessonContentImage;
    [SerializeField] private TMP_Text emptyContentLabel;

    [Header("Center Classroom - 3D Model")]
    [SerializeField] private Transform modelSpawnRoot;
    [SerializeField] private Vector3 modelSpawnOffset = Vector3.zero;
    [SerializeField] private bool destroyOldModelOnSwitch = true;

    [Header("Color Management - Chapter Active")]
    [SerializeField] private SelectionColors chapterColors = new SelectionColors
    {
        activeBackground = new Color(0.14f, 0.35f, 0.72f, 0.95f),
        inactiveBackground = new Color(0.12f, 0.14f, 0.24f, 0.4f),
        activeText = Color.white,
        inactiveText = new Color(0.82f, 0.86f, 0.95f, 1f),
    };

    [Header("Color Management - Lesson Active")]
    [SerializeField] private SelectionColors lessonColors = new SelectionColors
    {
        activeBackground = new Color(0.09f, 0.53f, 0.58f, 0.95f),
        inactiveBackground = new Color(0.1f, 0.16f, 0.2f, 0.4f),
        activeText = Color.white,
        inactiveText = new Color(0.82f, 0.86f, 0.95f, 1f),
    };

    private readonly List<ChapterNodeUI> _chapterNodes = new List<ChapterNodeUI>();
    private int _activeChapterIndex = -1;
    private int _activeLessonIndex = -1;
    private GameObject _activeModelInstance;

    private class ChapterNodeUI
    {
        public RectTransform nodeRoot;
        public Button chapterButton;
        public RectTransform lessonContainer;
        public List<Button> lessonButtons = new List<Button>();
        public bool expanded;
    }

    private void Start()
    {
        if (modelSpawnRoot == null) modelSpawnRoot = transform;
        if (!ValidateUIRefs()) return;

        if (forceStandardNavbarLabels)
            NormalizeNavbarLabels();

        EnsureTreeRootLayout();
        ValidateInteractionSetup();
        BuildChapterButtons();
        SelectFirstAvailable();
    }

    public void SelectChapter(int chapterIndex)
    {
        if (!IsChapterValid(chapterIndex)) return;
        _activeChapterIndex = chapterIndex;
        _activeLessonIndex = -1;
        for (int i = 0; i < _chapterNodes.Count; i++)
            _chapterNodes[i].expanded = i == chapterIndex;

        if (autoSelectFirstLessonOnChapterClick)
        {
            LessonItem[] lessons = chapters[chapterIndex].lessons;
            if (lessons != null && lessons.Length > 0)
                SelectLesson(chapterIndex, 0);
            else
                RefreshTreeHighlights();
        }
        else
        {
            RefreshTreeHighlights();
        }
    }

    public void SelectLesson(int chapterIndex, int lessonIndex)
    {
        if (!IsChapterValid(chapterIndex)) return;
        LessonItem[] lessons = chapters[chapterIndex].lessons;
        if (lessons == null || lessons.Length == 0 || lessonIndex < 0 || lessonIndex >= lessons.Length) return;

        _activeChapterIndex = chapterIndex;
        _activeLessonIndex = lessonIndex;

        if (chapterIndex >= 0 && chapterIndex < _chapterNodes.Count)
            _chapterNodes[chapterIndex].expanded = true;

        if (onlyOneChapterExpanded)
        {
            for (int i = 0; i < _chapterNodes.Count; i++)
            {
                if (i != chapterIndex) _chapterNodes[i].expanded = false;
            }
        }

        RefreshTreeHighlights();
        PresentLesson(chapters[chapterIndex], lessons[lessonIndex]);
    }

    private bool ValidateUIRefs()
    {
        if (navbarTreeRoot == null || chapterButtonTemplate == null)
        {
            Debug.LogError("ClassroomLessonMenuController: Missing chapter button root/template.");
            return false;
        }

        if (lessonButtonTemplate == null)
        {
            Debug.LogError("ClassroomLessonMenuController: Missing lesson button template.");
            return false;
        }

        chapterButtonTemplate.gameObject.SetActive(false);
        lessonButtonTemplate.gameObject.SetActive(false);
        return true;
    }

    private void ValidateInteractionSetup()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemGo = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            eventSystemGo.AddComponent<InputSystemUIInputModule>();
#else
            eventSystemGo.AddComponent<StandaloneInputModule>();
#endif
            Debug.LogWarning("ClassroomLessonMenuController: EventSystem was missing, created automatically.");
        }

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogWarning("ClassroomLessonMenuController: Controller is not under a Canvas.");
            return;
        }

        if (parentCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            parentCanvas.gameObject.AddComponent<GraphicRaycaster>();
            Debug.LogWarning("ClassroomLessonMenuController: GraphicRaycaster was missing, added automatically.");
        }
    }

    private void EnsureTreeRootLayout()
    {
        if (navbarTreeRoot == null) return;

        VerticalLayoutGroup rootLayout = navbarTreeRoot.GetComponent<VerticalLayoutGroup>();
        if (rootLayout == null) rootLayout = navbarTreeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        rootLayout.childAlignment = TextAnchor.UpperLeft;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = false;
        rootLayout.spacing = 6f;
        rootLayout.padding = new RectOffset(0, 0, 0, 0);

        ContentSizeFitter rootFitter = navbarTreeRoot.GetComponent<ContentSizeFitter>();
        if (rootFitter == null) rootFitter = navbarTreeRoot.gameObject.AddComponent<ContentSizeFitter>();
        rootFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        rootFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void BuildChapterButtons()
    {
        ClearChapterNodes();
        CleanupOrphanRuntimeTreeNodes();
        if (chapters == null) return;

        for (int i = 0; i < chapters.Length; i++)
        {
            int index = i;
            ChapterNodeUI node = CreateChapterNode(chapters[i], index);
            _chapterNodes.Add(node);
        }
        RefreshTreeHighlights();
    }

    private void CleanupOrphanRuntimeTreeNodes()
    {
        if (navbarTreeRoot == null) return;

        var toRemove = new List<GameObject>();
        for (int i = 0; i < navbarTreeRoot.childCount; i++)
        {
            Transform child = navbarTreeRoot.GetChild(i);
            if (child == null) continue;

            if (chapterButtonTemplate != null && child == chapterButtonTemplate.transform) continue;
            if (lessonButtonTemplate != null && child == lessonButtonTemplate.transform) continue;
            if (child.name.EndsWith("_Node", StringComparison.Ordinal)) toRemove.Add(child.gameObject);
        }

        for (int i = 0; i < toRemove.Count; i++)
            Destroy(toRemove[i]);
    }

    private void SelectFirstAvailable()
    {
        if (chapters == null || chapters.Length == 0)
        {
            SetMainContentEmpty("Chua co du lieu bai hoc.");
            return;
        }

        for (int i = 0; i < chapters.Length; i++)
        {
            if (chapters[i].lessons != null && chapters[i].lessons.Length > 0)
            {
                SelectChapter(i);
                return;
            }
        }

        SetMainContentEmpty("Tat ca chuong hien tai chua co tiet hoc.");
    }

    private void PresentLesson(ChapterItem chapter, LessonItem lesson)
    {
        if (chapterTitleText != null) chapterTitleText.text = chapter.chapterName;
        if (lessonTitleText != null) lessonTitleText.text = lesson.lessonName;

        if (lessonContentImage != null)
        {
            bool hasImage = lesson.contentImage != null;
            lessonContentImage.sprite = lesson.contentImage;
            lessonContentImage.enabled = hasImage;
            lessonContentImage.preserveAspect = true;
            if (emptyContentLabel != null) emptyContentLabel.gameObject.SetActive(!hasImage);
        }

        if (emptyContentLabel != null && lesson.contentImage == null)
            emptyContentLabel.text = "Tiet nay chua gan anh noi dung.";

        SpawnLessonModel(lesson.modelPrefab);
    }

    private void SpawnLessonModel(GameObject prefab)
    {
        if (destroyOldModelOnSwitch && _activeModelInstance != null)
        {
            Destroy(_activeModelInstance);
            _activeModelInstance = null;
        }

        if (prefab == null) return;

        Vector3 spawnPos = modelSpawnRoot.position + modelSpawnOffset;
        _activeModelInstance = Instantiate(prefab, spawnPos, modelSpawnRoot.rotation);
        _activeModelInstance.transform.SetParent(modelSpawnRoot, true);
        _activeModelInstance.name = prefab.name + "_Runtime";
    }

    private void RefreshChapterHighlights()
    {
        for (int i = 0; i < _chapterNodes.Count; i++)
        {
            bool active = i == _activeChapterIndex;
            ApplyButtonVisual(_chapterNodes[i].chapterButton, active, chapterColors);
        }
    }

    private void RefreshLessonHighlights()
    {
        if (_activeChapterIndex < 0 || _activeChapterIndex >= _chapterNodes.Count) return;
        ChapterNodeUI activeNode = _chapterNodes[_activeChapterIndex];
        for (int i = 0; i < activeNode.lessonButtons.Count; i++)
        {
            bool active = i == _activeLessonIndex;
            ApplyButtonVisual(activeNode.lessonButtons[i], active, lessonColors);
        }
    }

    private void RefreshTreeHighlights()
    {
        RefreshChapterHighlights();
        for (int chapterIndex = 0; chapterIndex < _chapterNodes.Count; chapterIndex++)
        {
            ChapterNodeUI node = _chapterNodes[chapterIndex];
            if (node.lessonContainer != null)
                node.lessonContainer.gameObject.SetActive(node.expanded);

            for (int lessonIndex = 0; lessonIndex < node.lessonButtons.Count; lessonIndex++)
            {
                bool active = chapterIndex == _activeChapterIndex && lessonIndex == _activeLessonIndex;
                ApplyButtonVisual(node.lessonButtons[lessonIndex], active, lessonColors);
            }

            if (node.nodeRoot != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(node.nodeRoot);
        }

        if (navbarTreeRoot is RectTransform treeRootRect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(treeRootRect);
    }

    private static void SetButtonLabel(Button button, string label)
    {
        TMP_Text tmp = button.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null) tmp.text = string.IsNullOrWhiteSpace(label) ? "Tiet" : label;
    }

    private static void ApplyButtonVisual(Button button, bool active, SelectionColors colors)
    {
        Image background = button.targetGraphic as Image;
        if (background == null) background = button.GetComponent<Image>();
        if (background != null)
            background.color = active ? colors.activeBackground : colors.inactiveBackground;

        TMP_Text tmp = button.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
            tmp.color = active ? colors.activeText : colors.inactiveText;
    }

    private void SetMainContentEmpty(string message)
    {
        if (chapterTitleText != null) chapterTitleText.text = "No chapter";
        if (lessonTitleText != null) lessonTitleText.text = "No lesson";
        if (lessonContentImage != null)
        {
            lessonContentImage.sprite = null;
            lessonContentImage.enabled = false;
        }
        if (emptyContentLabel != null)
        {
            emptyContentLabel.text = message;
            emptyContentLabel.gameObject.SetActive(true);
        }
    }

    private bool IsChapterValid(int idx) => chapters != null && idx >= 0 && idx < chapters.Length;

    private ChapterNodeUI CreateChapterNode(ChapterItem chapter, int chapterIndex)
    {
        GameObject nodeGO = new GameObject($"{chapter.chapterName}_Node", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        RectTransform nodeRect = nodeGO.GetComponent<RectTransform>();
        nodeRect.SetParent(navbarTreeRoot, false);
        nodeRect.localScale = Vector3.one;

        VerticalLayoutGroup nodeLayout = nodeGO.GetComponent<VerticalLayoutGroup>();
        nodeLayout.childControlWidth = true;
        nodeLayout.childControlHeight = true;
        nodeLayout.childForceExpandHeight = false;
        nodeLayout.childForceExpandWidth = true;
        nodeLayout.spacing = 4f;
        nodeLayout.padding = new RectOffset(0, 0, 0, 6);

        ContentSizeFitter nodeFitter = nodeGO.GetComponent<ContentSizeFitter>();
        nodeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        nodeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Button chapterButton = Instantiate(chapterButtonTemplate, nodeRect);
        PrepareButtonForLayout(chapterButton, chapterButtonTemplate, fallbackChapterButtonHeight);
        chapterButton.gameObject.SetActive(true);
        chapterButton.onClick.RemoveAllListeners();
        chapterButton.onClick.AddListener(() => SelectChapter(chapterIndex));
        SetButtonLabel(chapterButton, chapter.chapterName);

        GameObject lessonContainerGO = new GameObject($"{chapter.chapterName}_Lessons", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(LayoutElement));
        RectTransform lessonContainerRect = lessonContainerGO.GetComponent<RectTransform>();
        lessonContainerRect.SetParent(nodeRect, false);
        lessonContainerRect.localScale = Vector3.one;

        VerticalLayoutGroup lessonLayout = lessonContainerGO.GetComponent<VerticalLayoutGroup>();
        lessonLayout.childControlWidth = true;
        lessonLayout.childControlHeight = true;
        lessonLayout.childForceExpandHeight = false;
        lessonLayout.childForceExpandWidth = true;
        lessonLayout.spacing = 2f;
        lessonLayout.padding = new RectOffset(Mathf.RoundToInt(lessonIndentLeft), 0, 0, 0);

        ContentSizeFitter lessonFitter = lessonContainerGO.GetComponent<ContentSizeFitter>();
        lessonFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        lessonFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        LayoutElement lessonLayoutElement = lessonContainerGO.GetComponent<LayoutElement>();
        lessonLayoutElement.minHeight = 0f;

        ChapterNodeUI node = new ChapterNodeUI
        {
            nodeRoot = nodeRect,
            chapterButton = chapterButton,
            lessonContainer = lessonContainerRect,
            expanded = false
        };

        LessonItem[] lessons = chapter.lessons ?? Array.Empty<LessonItem>();
        for (int lessonIndex = 0; lessonIndex < lessons.Length; lessonIndex++)
        {
            int captureLessonIndex = lessonIndex;
            Button lessonButton = Instantiate(lessonButtonTemplate, lessonContainerRect);
            PrepareButtonForLayout(lessonButton, lessonButtonTemplate, fallbackLessonButtonHeight);
            lessonButton.gameObject.SetActive(true);
            lessonButton.onClick.RemoveAllListeners();
            lessonButton.onClick.AddListener(() => SelectLesson(chapterIndex, captureLessonIndex));
            SetButtonLabel(lessonButton, lessons[lessonIndex].lessonName);
            node.lessonButtons.Add(lessonButton);
        }

        return node;
    }

    private void ClearChapterNodes()
    {
        for (int i = 0; i < _chapterNodes.Count; i++)
        {
            if (_chapterNodes[i].nodeRoot != null)
                Destroy(_chapterNodes[i].nodeRoot.gameObject);
        }
        _chapterNodes.Clear();
    }

    private static void PrepareButtonForLayout(Button instance, Button template, float fallbackHeight)
    {
        if (instance == null) return;

        RectTransform instanceRect = instance.GetComponent<RectTransform>();
        RectTransform templateRect = template != null ? template.GetComponent<RectTransform>() : null;
        float preferredHeight = fallbackHeight;
        if (templateRect != null && templateRect.rect.height > 1f)
            preferredHeight = templateRect.rect.height;

        LayoutElement layout = instance.GetComponent<LayoutElement>();
        if (layout == null) layout = instance.gameObject.AddComponent<LayoutElement>();
        layout.ignoreLayout = false;
        layout.minHeight = preferredHeight;
        layout.preferredHeight = preferredHeight;
        layout.flexibleHeight = 0f;

        if (instanceRect != null)
        {
            instanceRect.anchorMin = new Vector2(0f, 0.5f);
            instanceRect.anchorMax = new Vector2(1f, 0.5f);
            instanceRect.pivot = new Vector2(0.5f, 0.5f);
            instanceRect.sizeDelta = new Vector2(0f, preferredHeight);
            instanceRect.anchoredPosition = Vector2.zero;
            instanceRect.localScale = Vector3.one;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Populate Bai10-Bai14 Data")]
    private void AutoPopulateDataFromFolders()
    {
        string[] chapterIds = { "Bai10", "Bai11", "Bai12", "Bai13", "Bai14" };
        var result = new List<ChapterItem>();

        foreach (string chapterId in chapterIds)
        {
            string imageFolder = $"Assets/ImageContent/{chapterId}";
            string prefabFolder = $"Assets/Prefab/{chapterId}";
            List<Sprite> images = LoadAssetsByFolder<Sprite>(imageFolder);
            List<GameObject> prefabs = LoadAssetsByFolder<GameObject>(prefabFolder);
            int lessonCount = Mathf.Max(images.Count, prefabs.Count);

            if (lessonCount == 0) continue;

            var lessons = new LessonItem[lessonCount];
            for (int i = 0; i < lessonCount; i++)
            {
                lessons[i] = new LessonItem
                {
                    lessonName = $"Tiet {i + 1}",
                    contentImage = i < images.Count ? images[i] : null,
                    modelPrefab = i < prefabs.Count ? prefabs[i] : null
                };
            }

            result.Add(new ChapterItem
            {
                chapterName = chapterId.Replace("Bai", "Bai "),
                lessons = lessons
            });
        }

        chapters = result.ToArray();
        NormalizeNavbarLabels();
        EditorUtility.SetDirty(this);
        Debug.Log($"ClassroomLessonMenuController: Auto-populated {chapters.Length} chapters.");
    }

    [ContextMenu("Normalize Navbar Labels")]
    private void NormalizeNavbarLabels()
    {
        if (chapters == null) return;

        for (int chapterIndex = 0; chapterIndex < chapters.Length; chapterIndex++)
        {
            ChapterItem chapter = chapters[chapterIndex];
            if (chapter == null) continue;

            chapter.chapterName = $"Bài {firstChapterNumber + chapterIndex}";

            if (chapter.lessons == null) continue;
            for (int lessonIndex = 0; lessonIndex < chapter.lessons.Length; lessonIndex++)
            {
                if (chapter.lessons[lessonIndex] == null) continue;
                chapter.lessons[lessonIndex].lessonName = $"Tiết {lessonIndex + 1}";
            }
        }
    }

    private static List<T> LoadAssetsByFolder<T>(string folderPath) where T : UnityEngine.Object
    {
        var items = new List<T>();
        if (!AssetDatabase.IsValidFolder(folderPath)) return items;

        string filter = $"t:{typeof(T).Name}";
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });
        Array.Sort(guids, (a, b) =>
        {
            string pathA = AssetDatabase.GUIDToAssetPath(a);
            string pathB = AssetDatabase.GUIDToAssetPath(b);
            return EditorUtility.NaturalCompare(pathA, pathB);
        });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) items.Add(asset);
        }

        return items;
    }
#endif
}
