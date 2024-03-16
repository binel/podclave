using Podclave.Cli.Tasks;

namespace Podclave.Cli.Handlers;

public interface IHandler
{
    Task Handle(WorkTask t);
}