using DeliveryBackend.Configurations;
using DeliveryBackend.Data;
using Quartz;

namespace DeliveryBackend.Jobs;

public class DeleteInvalidTokensJob : IJob
{
    private readonly ApplicationDbContext _context;

    public DeleteInvalidTokensJob(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task Execute(IJobExecutionContext context)
    {

        var invalidTokens = _context.Tokens;
        foreach (var invalidToken in invalidTokens.Where(invalidToken => 
                     invalidToken.ExpiredDate + TimeSpan.FromMinutes(JwtConfigurations.Lifetime) < DateTime.UtcNow))
        {
            invalidTokens.Remove(invalidToken);
        }
        _context.SaveChanges();

        return Task.FromResult(true);
    }
}