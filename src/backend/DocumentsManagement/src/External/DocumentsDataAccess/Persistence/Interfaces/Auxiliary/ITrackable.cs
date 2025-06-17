namespace DocumentsDataAccess.Persistence.Interfaces.Auxiliary;

public interface ITrackable
{
    DateTime LastRetrievedAt { get; set; }
}
