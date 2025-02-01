using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChessApi.Models.DB;

public class Error
{
    [BsonId]
    public ObjectId ErrorID { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public Error? InnerException { get; set; }

    public Error(Exception ex, int? depth = null)
    {
        this.Message = ex.Message;
        this.StackTrace = ex.StackTrace;
        this.Source = ex.Source;

        if (depth < 3 && ex.InnerException is not null)
        {
            this.InnerException = new Error(ex, (depth ?? 0) + 1);
        }
    }
}
