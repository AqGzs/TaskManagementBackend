using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;

public class TaskReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskReminderService> _logger;

    public TaskReminderService(IServiceProvider serviceProvider, ILogger<TaskReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Checking for tasks nearing deadline...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var tasks = await dbContext.TaskItems
                    .Where(t => !t.IsComplete && t.DueDate <= DateTime.Now.AddMinutes(5))
                    .ToListAsync();

                foreach (var task in tasks)
                {
                    var user = await dbContext.Users.FindAsync(task.UserId);
                    if (user != null)
                    {
                        _logger.LogInformation($"Sending reminder for task '{task.Title}' to '{user.Email}'.");
                        await emailService.SendReminderEmail(user.Email, task);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
