namespace ONEbook.UM.Api.Configurations;

public class UnderMaintenanceApiConfiguration
{
    public const string UnderMaintenanceApi = "ONEbook.UM.Api";

    public string SecretKey { get; set; } = string.Empty;
    public bool EnableCheckUM { get; set; } = true;
    public bool EnableUnscheduledUM { get; set; }
    public string UMFrom { get; set; } = string.Empty;
    public string UMTo { get; set; } = string.Empty;
    public string[] UMServiceURL { get; set; } = [];
}