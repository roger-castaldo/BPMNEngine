using BPMNEngine.Interfaces.Tasks;

namespace BPMNEngine.Interfaces.Extensions
{
    public interface ITaskExtensionElementElement : IExtensionElement
    {
        ValueTask<bool> ExecuteTaskExtensionAsync(ITask task);
    }
}
