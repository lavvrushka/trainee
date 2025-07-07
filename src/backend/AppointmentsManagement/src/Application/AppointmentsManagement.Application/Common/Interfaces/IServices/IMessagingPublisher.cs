namespace AppointmentsManagement.Application.Common.Interfaces.IServices;

public interface IMessagingPublisher
{
    public Task PublishAsync<T>(string routingKey, T message);
}
