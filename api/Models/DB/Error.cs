using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models.db;

public class Error
{
    [BsonId]
    public ObjectId ErrorID { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public Error? InnerException { get; set; }

    public Error(Exception ex)
    {
        this.Message = ex.Message;
        this.StackTrace = ex.StackTrace;
        this.Source = ex.Source;

        if (ex.InnerException != null)
        {
            this.InnerException = new Error(ex);
        }
    }
}
