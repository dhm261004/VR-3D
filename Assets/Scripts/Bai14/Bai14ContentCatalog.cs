public static class Bai14ContentCatalog
{
    public struct LessonEntry
    {
        public string Label, Title, Description;
        public LessonEntry(string label, string title, string desc)
        { Label = label; Title = title; Description = desc; }
    }

    public const string LessonTitle = "Bài 14: Phép chiếu song song";

    public static readonly LessonEntry[] Lessons =
    {
        new LessonEntry(
            "1) Module 1",
            "Khái niệm phép chiếu song song",
            "Tia sáng mặt trời là phương chiếu Δ.\n" +
            "Sàn nhà là mặt phẳng chiếu α.\n\n" +
            "Bóng A′B′C′D′ in xuống sàn chính là\n" +
            "hình chiếu song song của khung cửa ABCD.\n\n" +
            "[Space] để xem lại — [R] để reset."
        ),
        new LessonEntry(
            "2) Module 2",
            "Tính chất của phép chiếu",
            "Phép chiếu song song:\n" +
            "✔ Biến 3 điểm thẳng hàng → thẳng hàng\n" +
            "✔ Biến 2 đường // → song song\n" +
            "✔ Bảo toàn tỉ số độ dài cùng đường thẳng\n" +
            "✘ KHÔNG bảo toàn độ dài đoạn thẳng\n\n" +
            "[Space] → bước tiếp — [R] → reset."
        ),
        new LessonEntry(
            "3) Module 3",
            "Hình biểu diễn của hình không gian",
            "Quy tắc vẽ hình biểu diễn:\n" +
            "• Hình tròn → Elip\n" +
            "• Hình vuông → Hình bình hành\n" +
            "• Góc vuông không được bảo toàn\n\n" +
            "[Space] → bước tiếp — [R] → reset."
        ),
    };

    public static readonly string[] LessonLabels = BuildLabels();
    public static int LessonCount => Lessons.Length;

    public static LessonEntry GetLesson(int i) =>
        (i >= 0 && i < Lessons.Length) ? Lessons[i] : default;

    private static string[] BuildLabels()
    {
        var r = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) r[i] = Lessons[i].Label;
        return r;
    }
}
