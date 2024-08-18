using Podclave.Core.Tasks;

namespace Podclave.Core.Handlers;

public interface IHandler
{
    Task Handle(WorkTask t);
}
