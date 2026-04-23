public static class Bai12ContentCatalog
{
    public struct LessonEntry
    {
        public string Label;
        public string Title;
        public string Description;
        public LessonEntry(string label, string title, string description)
        {
            Label = label; Title = title; Description = description;
        }
    }

    public const string LessonTitle = "Bài 12: Đường thẳng song song với mặt phẳng";

    public static readonly LessonEntry[] Lessons =
    {
        new LessonEntry(
            "1) Module 1",
            "Module 1 — Ba vị trí tương đối",
            "Ba vị trí tương đối của đường thẳng và mặt phẳng\n\n" +
            "Định nghĩa:\n" +
            "• d // (α): đường thẳng và mặt phẳng không có điểm chung.\n" +
            "• d ∩ (α) = {M}: cắt nhau tại đúng một điểm M.\n" +
            "• d ⊂ (α): đường thẳng nằm trong mặt phẳng.\n\n" +
            "Quan sát ba hoạt ảnh lần lượt trong không gian.\n" +
            "Nhấn [Space] để bắt đầu — nhấn [R] để xem lại."
        ),
        new LessonEntry(
            "2) Module 2",
            "Module 2 — Điều kiện nhận biết song song",
            "Điều kiện nhận biết đường thẳng song song mặt phẳng:\n\n" +
            "\"Nếu đường thẳng a không nằm trong (P) và song song với\n" +
            "một đường thẳng b nằm trong (P) thì a song song với (P).\"\n\n" +
            "Điều kiện:\n" +
            "① a không nằm trong (P)\n" +
            "② b ⊂ (P)\n" +
            "③ a // b\n" +
            "→ Kết luận: a // (P)\n\n" +
            "Nhấn [Space] để xem animation từng bước — [R] để reset."
        ),
        new LessonEntry(
            "3) Module 3",
            "Module 3 — Tính chất song song",
            "Tính chất của đường thẳng song song với mặt phẳng:\n\n" +
            "\"Cho đường thẳng a song song với mặt phẳng (P).\n" +
            "Nếu mặt phẳng (Q) chứa a và cắt (P) theo giao tuyến b\n" +
            "thì a song song với b.\"\n\n" +
            "Sơ đồ:\n" +
            "• Giả thiết: a // (P), a ⊂ (Q), (Q) ∩ (P) = b\n" +
            "• Kết luận: a // b\n\n" +
            "Nhấn [Space] để xem animation — [R] để reset."
        ),
    };

    public static readonly string[] LessonLabels = BuildLabels();
    public static readonly string[] LessonTitles = BuildTitles();
    public static readonly string[] LessonDescriptions = BuildDescriptions();
    public static int LessonCount => Lessons.Length;

    public static LessonEntry GetLesson(int index)
    {
        if (index < 0 || index >= Lessons.Length) return default;
        return Lessons[index];
    }

    private static string[] BuildLabels()
    {
        var r = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) r[i] = Lessons[i].Label;
        return r;
    }
    private static string[] BuildTitles()
    {
        var r = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) r[i] = Lessons[i].Title;
        return r;
    }
    private static string[] BuildDescriptions()
    {
        var r = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) r[i] = Lessons[i].Description;
        return r;
    }
}
