using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public class ServicesInitializer
{
    private string _environmentName;

    public ServicesInitializer(string environmentName)
    {
        _environmentName = environmentName;
    }

    public async Task Initialize()
    {
        InitializationOptions options = new InitializationOptions();
        if (!string.IsNullOrEmpty(_environmentName))
        {
            options.SetEnvironmentName(_environmentName);
        }

        await UnityServices.InitializeAsync(options);
    }
}