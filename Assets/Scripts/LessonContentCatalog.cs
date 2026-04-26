public static class LessonContentCatalog
{
    public struct LessonEntry
    {
        public string Label;
        public string Title;
        public string Description;

        public LessonEntry(string label, string title, string description)
        {
            Label = label;
            Title = title;
            Description = description;
        }
    }

    public const string LessonTitle = "Bài 13: Hai mặt phẳng song song";

    
    
    public static readonly LessonEntry[] Lessons =
    {
        new LessonEntry(
            "1) Lesson 1",
            "Lesson 1",
            Module1LectureContent + "\n\n" + Module1InteractiveContent),
        new LessonEntry(
            "2) Lesson 2.1",
            "Lesson 2.1 - Giao tuyến (Mô phỏng 3D)",
            "Lesson 2.1 - Giao tuyến hai mặt phẳng (Mô phỏng 3D)\n\n" +
            "Mục tiêu:\n" +
            "• Trực quan giao tuyến của hai mặt phẳng trong không gian.\n" +
            "• Quan sát sự thay đổi vị trí/định hướng và kết luận về đường giao tuyến.\n\n" +
            "Nội dung mô phỏng:\n" +
            Module2LectureContent),
        new LessonEntry(
            "3) Lesson 2.2",
            "Lesson 2.2 - Giao tuyến",
            "Lesson 2.2 - Giao tuyến hai mặt phẳng (Tương tác)\n\n" +
            "Mục tiêu:\n" +
            "• Người học trực tiếp xoay mặt phẳng để làm thay đổi giao tuyến.\n" +
            "• So sánh các trạng thái giao tuyến khi góc nghiêng thay đổi.\n\n" +
            "Nội dung thực hành:\n" +
            Module2PracticeContent),
        new LessonEntry(
            "4) Lesson 3",
            "Lesson 3 - Tỷ số đồng nhất",
            "Bài giảng tỷ số đồng nhất:\n\n" + Module3LectureContent + "\n\n" + Module3PracticeContent),
        new LessonEntry(
            "5) Lesson 4",
            "Lesson 4 - Hình lăng trụ",
            "Bài giảng hình lăng trụ:\n\n" + Module4LectureContent)
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
        string[] labels = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) labels[i] = Lessons[i].Label;
        return labels;
    }

    private static string[] BuildTitles()
    {
        string[] titles = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) titles[i] = Lessons[i].Title;
        return titles;
    }

    private static string[] BuildDescriptions()
    {
        string[] descriptions = new string[Lessons.Length];
        for (int i = 0; i < Lessons.Length; i++) descriptions[i] = Lessons[i].Description;
        return descriptions;
    }

    public const string SidebarModules =
        "Danh sách module\n" +
        "1) Điều kiện để hai mặt phẳng song song\n" +
        "2) Tính chất của hai mặt phẳng song song\n" +
        "3) Định lí Thalès trong không gian\n" +
        "4) Hình lăng trụ và hình hộp";

    public const string Module1Title = "Module 1: Điều kiện để hai mặt phẳng song song";
    public const string Module1LectureContent =
        "Phân đoạn giảng (Lý thuyết)\n\n" +
        "Nội dung định lý:\n" +
        "\"Nếu mặt phẳng α chứa hai đường thẳng cắt nhau, và hai đường này cùng song song với mặt phẳng β,\n" +
        "thì α và β song song với nhau.\"\n\n" +
        "Trình diễn trong không gian trung tâm:\n" +
        "• Hai mặt phẳng đồ họa α và β xuất hiện, lơ lửng song song.\n" +
        "• Trên mặt phẳng α hiển thị hai đường thẳng a và b cắt nhau.\n" +
        "• Người học quan sát điều kiện để kết luận α // β.";
    public const string Module1InteractiveContent =
        "Trạng thái hệ thống:\n" +
        "• Đang chạy mô phỏng...\n\n" +
        "Mục tiêu nhận thức:\n" +
        "• Xác định đúng vai trò của α, β, a, b trong định lý.\n" +
        "• Kết luận được điều kiện đủ để hai mặt phẳng song song.";

    public const string Module2Title = "Module 2: Tính chất của hai mặt phẳng song song";
    public const string Module2LectureContent =
        "Phân đoạn giảng (Lý thuyết)\n\n" +
        "Tính chất:\n" +
        "\"Nếu một mặt phẳng cắt hai mặt phẳng song song,\n" +
        "thì hai giao tuyến của chúng song song với nhau.\"\n\n" +
        "Trình diễn:\n" +
        "• Một mặt phẳng cắt thứ ba đi xuyên qua α và β.\n" +
        "• Hai giao tuyến được highlight màu vàng và hiển thị ký hiệu a // b.";
    public const string Module2PracticeContent =
        "Phân đoạn thực hành (Tương tác vật lý)\n\n" +
        "Nhiệm vụ:\n" +
        "• Sử dụng bản lề để thay đổi góc nghiêng của mặt phẳng cắt.\n" +
        "• Quan sát hai giao tuyến màu vàng trượt theo bề mặt.\n" +
        "• Theo dõi đồng hồ đo khoảng cách ảo (Data Tracker).\n\n" +
        "Phản hồi mong đợi:\n" +
        "• Xác minh thành công: Giao tuyến luôn song song.";

    public const string Module3Title = "Module 3: Định lí Thalès trong không gian";
    public const string Module3LectureContent =
        "Phân đoạn giảng (Lý thuyết)\n\n" +
        "Thiết lập không gian:\n" +
        "• Ba mặt phẳng song song cách đều nhau xuất hiện.\n" +
        "• Một cát tuyến xuyên qua ba mặt phẳng, tạo các điểm A, B, C.\n\n" +
        "Công thức định lý:\n" +
        "AB/A'B' = BC/B'C' = AC/A'C'\n\n" +
        "Ý nghĩa:\n" +
        "• Các tỉ số đoạn thẳng tương ứng trên hai cát tuyến luôn bằng nhau.";
    public const string Module3PracticeContent =
        "Phân đoạn thực hành (Tương tác vật lý)\n\n" +
        "Nhiệm vụ:\n" +
        "• Dùng cát tuyến thứ hai đâm xuyên ba mặt phẳng tại vị trí/góc bất kỳ.\n" +
        "• Hệ thống gắn điểm mới A', B', C' và cập nhật dữ liệu real-time.\n\n" +
        "Phản hồi mong đợi:\n" +
        "• Giá trị tử và mẫu thay đổi liên tục.\n" +
        "• Kết quả ba phép chia luôn đồng nhất (ví dụ cùng bằng 0.5).";

    public const string Module4Title = "Module 4: Hình lăng trụ và hình hộp";
    public const string Module4LectureContent =
        "Phân đoạn giảng (Hình lăng trụ)\n\n" +
        "• Hệ thống dựng một khối lăng trụ từ hai mặt phẳng α (trên) và β (dưới).\n" +
        "• Các cạnh bên song song; mặt bên hiển thị vật liệu kính mờ.\n" +
        "• Nội dung: Các mặt bên của hình lăng trụ là hình bình hành.\n\n" +
        "Phân đoạn giảng chuyển tiếp (Hình hộp)\n\n" +
        "• Hình lăng trụ có đáy là hình bình hành gọi là hình hộp.\n" +
        "• Bốn đường chéo không gian xuất hiện và cắt nhau tại một điểm tâm.";
    public const string Module4PracticeContent =
        "Phân đoạn thực hành 1 (Đo lường mặt bên)\n\n" +
        "• Dùng Caliper Tool để đo cạnh dọc/cạnh ngang của một mặt bên.\n" +
        "• Xác minh: Mặt bên là hình bình hành.\n\n" +
        "Phân đoạn thực hành 2 (Giao điểm đường chéo)\n\n" +
        "• Kéo biến dạng khối hộp, hệ thống vẫn giữ các mặt đối diện song song.\n" +
        "• Theo dõi tọa độ trung điểm 4 đường chéo luôn trùng nhau theo thời gian thực.\n" +
        "• Hoàn thành toàn bộ bài học khi xác nhận được tâm đối xứng.";
}
