using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // chỉ để sẵn nếu cần
using SEP490_SU25_G86_API.Models;

public class JobExpiryWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<JobExpiryWorker> _logger;
    public JobExpiryWorker(IServiceProvider sp, ILogger<JobExpiryWorker> logger)
        => (_sp, _logger) = (sp, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SEP490_G86_CvMatchContext>();

                // LOG để xác nhận đúng DB
                var conn = db.Database.GetDbConnection();
                _logger.LogInformation("JobExpiryWorker tick {Utc}. DB={Db}; Server={Server}",
                    DateTime.UtcNow, conn.Database, conn.DataSource);
                _logger.LogInformation("ConnString={Conn}", conn.ConnectionString);
                // ĐÚNG câu lệnh bạn chạy tay
                var affected = await db.Database.ExecuteSqlRawAsync(@"
UPDATE JobPosts
SET Status = 'CLOSED',
    UpdatedDate = SYSUTCDATETIME()
WHERE Status = 'OPEN'
  AND EndDate IS NOT NULL
  AND EndDate < CAST(SYSUTCDATETIME() AS date);",
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Auto-closed {Count} jobs.", affected);

                // (Optional) đếm xem còn bao nhiêu OPEN quá hạn
                var remain = await db.JobPosts
                    .CountAsync(j => j.Status == "OPEN"
                                  && j.EndDate != null
                                  && j.EndDate < DateTime.UtcNow.Date, stoppingToken);
                _logger.LogInformation("Remaining OPEN & expired (date): {Remain}", remain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Expiry worker failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
