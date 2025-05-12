namespace OfficesManagement.BuisnessLogic.DTOs.Requests;

public record FilterOfficesRequest(
     string? Address,
     string? City,
     string? Country,
     string? IsActive
 );
