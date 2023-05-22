namespace Backend.Common.Interfaces;

public interface IProducerService
{
    public void SendMessage<T>(T message);
}