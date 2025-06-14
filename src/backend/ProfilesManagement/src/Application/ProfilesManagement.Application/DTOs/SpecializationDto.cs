using ProfilesManagement.Application.UseCases.SpecializationUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.DTOs;

public class SpecializationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
public static class SpecializationMapper
{
    public static SpecializationDto MapSpecializationToDto(this Specialization specialization)
    {
        var dto = new SpecializationDto();
        dto.Id = specialization.Id;
        dto.Name = specialization.Name;
        dto.Description = specialization.Description;

        return dto;
    }

    public static Specialization MapSpecializationDtoToEntity(this CreateSpecializationRequest request)
    {
        var entity = new Specialization();
        entity.Id = Guid.NewGuid();
        entity.Name = request.Name;
        entity.Description = request.Description;

        return entity;
    }

    public static void MapSpecializationDtoToEntity(this UpdateSpecializationRequest request, Specialization entity)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
    }
}