using Hangfire;

namespace DiscordRPG.Client;

public class ContainerJobActivator : JobActivator
{
    private readonly IServiceProvider provider;

    public ContainerJobActivator(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public override object ActivateJob(Type jobType)
    {
        return provider.GetService(jobType);
    }
}