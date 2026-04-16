using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonUIWorldPanel : MonoBehaviour
{
    [System.Serializable]
    private class ModuleTitleItem
    {
        public TMP_Text label;
        public Image background;
        public Image leftIndicator;
    }

    [Header("Header")]
    [SerializeField] private TMP_Text lessonTitleText;
    [SerializeField] private TMP_Text moduleTitleText;

    [Header("Main Content")]
    [SerializeField] private TMP_Text contentText;

    [Header("Navigation Buttons")]
    [SerializeField] private Button previousModuleButton;
    [SerializeField] private Button nextModuleButton;

    [Header("Module Titles")]
    [SerializeField] private ModuleTitleItem[] moduleTitles = new ModuleTitleItem[4];
    [SerializeField] private Color moduleActiveBackgroundColor = new Color(0.13f, 0.28f, 0.52f, 0.95f);
    [SerializeField] private Color moduleInactiveBackgroundColor = new Color(0.12f, 0.14f, 0.24f, 0.35f);
    [SerializeField] private Color moduleActiveTextColor = Color.white;
    [SerializeField] private Color moduleInactiveTextColor = new Color(0.8f, 0.84f, 0.95f, 1f);
    [SerializeField] private Color moduleActiveIndicatorColor = new Color(0.22f, 0.71f, 1f, 1f);
    [SerializeField] private Color moduleInactiveIndicatorColor = new Color(0.22f, 0.71f, 1f, 0f);
    [SerializeField] private float activeFontSize = 31f;
    [SerializeField] private float inactiveFontSize = 27f;

    public void SetHeader(string lessonTitle, string moduleTitle)
    {
        if (lessonTitleText != null) lessonTitleText.text = lessonTitle;
        if (moduleTitleText != null) moduleTitleText.text = moduleTitle;
    }

    public void SetContent(string content)
    {
        if (contentText != null) contentText.text = content;
    }

    public void SetModuleTitleLabels(string[] labels)
    {
        if (labels == null) labels = new string[0];
        for (int i = 0; i < moduleTitles.Length; i++)
        {
            ModuleTitleItem item = moduleTitles[i];
            if (item == null || item.label == null) continue;

            bool hasData = i < labels.Length;
            item.label.text = hasData ? labels[i] : string.Empty;
            item.label.gameObject.SetActive(hasData);

            if (item.background != null) item.background.gameObject.SetActive(hasData);
            if (item.leftIndicator != null) item.leftIndicator.gameObject.SetActive(hasData);
        }

        if (labels.Length > moduleTitles.Length)
            Debug.LogWarning($"LessonUIWorldPanel: Only {moduleTitles.Length} title slots assigned, but catalog has {labels.Length} lessons.");
    }

    public void SetActiveModule(int activeIndex)
    {
        for (int i = 0; i < moduleTitles.Length; i++)
        {
            ModuleTitleItem item = moduleTitles[i];
            if (item == null || item.label == null) continue;

            bool isVisible = item.label.gameObject.activeSelf;
            bool isActive = isVisible && i == activeIndex;
            item.label.color = isActive ? moduleActiveTextColor : moduleInactiveTextColor;
            item.label.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
            item.label.fontSize = isActive ? activeFontSize : inactiveFontSize;

            if (item.background != null)
                item.background.color = isActive ? moduleActiveBackgroundColor : moduleInactiveBackgroundColor;

            if (item.leftIndicator != null)
                item.leftIndicator.color = isActive ? moduleActiveIndicatorColor : moduleInactiveIndicatorColor;
        }
    }

    public void SetNavigationState(bool canGoPrevious, bool canGoNext)
    {
        if (previousModuleButton != null)
            previousModuleButton.interactable = canGoPrevious;

        if (nextModuleButton != null)
            nextModuleButton.interactable = canGoNext;
    }

    public void BindNavigationButtons(
        UnityEngine.Events.UnityAction onPreviousModule,
        UnityEngine.Events.UnityAction onNextModule)
    {
        if (previousModuleButton != null)
        {
            previousModuleButton.onClick.RemoveAllListeners();
            previousModuleButton.onClick.AddListener(onPreviousModule);
        }

        if (nextModuleButton != null)
        {
            nextModuleButton.onClick.RemoveAllListeners();
            nextModuleButton.onClick.AddListener(onNextModule);
        }
    }
}
