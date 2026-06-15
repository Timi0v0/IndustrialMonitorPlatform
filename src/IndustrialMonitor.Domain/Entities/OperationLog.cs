namespace IndustrialMonitor.Domain.Entities;

public class OperationLog
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string OperationContent { get; set; } = string.Empty;
    public DateTime OperationTime { get; set; }
    public string Result { get; set; } = string.Empty;
}
