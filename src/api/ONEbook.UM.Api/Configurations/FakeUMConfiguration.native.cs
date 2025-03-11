namespace ONEbook.UM.Api.Configurations;

public class FakeUMConfiguration
{
    public const string FakeUM = "FakeUM";

    public bool Enable { get; set; } = false;
    public int Period { get; set; } = 300;
    public int[] MinutesCheck { get; set; } = [15, 45];
}