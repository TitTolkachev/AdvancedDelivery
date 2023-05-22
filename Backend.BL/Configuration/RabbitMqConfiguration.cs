namespace Notifications.BL.Configuration;

public class RabbitMqConfiguration
{
    public RabbitMqConfiguration(string hostName, string username, string password)
    {
        HostName = hostName;
        Username = username;
        Password = password;
    }

    public string HostName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}