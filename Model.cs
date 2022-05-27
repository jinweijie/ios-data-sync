namespace IosDataSync;
public class UploadModel
{
    public string Data { get; set; }
}

public class ReminderModel
{
    public string Name { get; set; }
    public List<ReminderItemModel> Items { get; set; } = new List<ReminderItemModel>();
}


public class ReminderItemModel
{
    public bool Completed { get; set; }
    public string Subject { get; set; }
    public DateTimeOffset DueDate { get; set; }
}

