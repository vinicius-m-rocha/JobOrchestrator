namespace JobOrchestrator.Infrastructure.Persistence;

public class DatabaseSettings
{
    public string DatabaseName { get; set; } = string.Empty;
    public string JobCollectionName { get; set; } = string.Empty;
}