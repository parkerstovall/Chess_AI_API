namespace api.models.client;

public class SavedGameResult
{
    public required BoardDisplay BoardDisplay { get; set; }
    public required bool IsPlayerWhite { get; set; }
    public required bool IsTwoPlayer { get; set; }
}
