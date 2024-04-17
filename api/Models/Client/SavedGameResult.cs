namespace api.models.client;

public class SavedGameResult
{
    public BoardDisplay? BoardDisplay { get; set; } = null;
    public bool IsPlayerWhite { get; set; }
    public bool IsTwoPlayer { get; set; }
}
