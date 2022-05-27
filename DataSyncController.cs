using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace IosDataSync;

[ApiController]
[Route("/")]
public class DataSyncController : ControllerBase
{
    private readonly Settings _settings;
    private readonly ILogger<DataSyncController> _logger;

    public DataSyncController(Settings settings, ILogger<DataSyncController> logger)
    {
        _settings = settings;
        _logger = logger;
    }
    
    [HttpPost("reminder/{name}")]
    public IActionResult UploadReminder([FromRoute]string name, [FromBody]UploadModel uploadModel)
    {
        if (!CheckKey()) return Unauthorized();

        var lineItems = uploadModel.Data.Split('\n');
        var reminder = new ReminderModel {Name = name};
        foreach (var lineItem in lineItems)
        {
            if(string.IsNullOrWhiteSpace(lineItem)) continue;
            
            var reminderItem = new ReminderItemModel();
            _logger.LogInformation("lineItem:" + lineItem);
            var tokens = lineItem.Split('|');
            
            if (tokens.Length > 0)
                reminderItem.Subject = tokens[0];

            if (tokens.Length > 1 && !string.IsNullOrWhiteSpace(tokens[1]))
                if(DateTimeOffset.TryParse(tokens[1], out var due)) reminderItem.DueDate = due;

            if (tokens.Length > 2 && !string.IsNullOrWhiteSpace(tokens[2]))
                reminderItem.Priority = tokens[2];

            if (tokens.Length > 3 && !string.IsNullOrWhiteSpace(tokens[3]))
                reminderItem.Flag = tokens[3];
            
            reminder.Items.Add(reminderItem);
        }

        using (var db = new LiteDatabase(_settings.DbFile))
        {
            var col = db.GetCollection<ReminderModel>("reminders");
            // delete the existing one
            col.DeleteMany(_ => _.Name == reminder.Name);

            col.Insert(reminder);
        }

        _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(reminder));
        
        return Ok();
    }

    [HttpGet("reminder/{name}")]
    public IActionResult Reminder([FromRoute] string name)
    {
        if (!CheckKey()) return Unauthorized();

        using var db = new LiteDatabase(_settings.DbFile);
        var col = db.GetCollection<ReminderModel>("reminders");
        var reminder = col.Find(_ => _.Name == name).FirstOrDefault();

        if (reminder == null) return NotFound();

        return Ok(reminder);
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok("healthy");
    }

    private bool CheckKey()
    {
        return Request.Headers["key"] == _settings.Key;
    }
}