namespace OfficesManagement.Core.DTOs.Requests;

public record LocationRequest(
    string? Address,
    string? City,
    string? State,
    string? Country);