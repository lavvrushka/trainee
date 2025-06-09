using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.DTOs;
using ProfilesManagement.Domain.Models;

namespace ProfilesManagement.Application.UseCases.Patient
{
    public record CreatePatientRequest(
      string FirstName,
      string LastName,
      string MiddleName,
      Guid AccountId,
      Guid? ImageId
      ) : IRequest<Guid>;

    public class CreatePatientHandler : IRequestHandler<CreatePatientRequest, Guid>
    {
        private readonly IPatientRepository _repository;

        public CreatePatientHandler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreatePatientRequest request, CancellationToken cancellationToken)
        {
            var patient = request.MapToPatient();
            //ToDo: добавить пикчу здесь через сервис для файлов 
            await _repository.AddAsync(patient);

            return patient.Id;
        }
    }
}
