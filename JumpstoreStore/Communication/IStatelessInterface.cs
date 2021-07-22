using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatelessInterface : IService // This is a service fabric reference
    {
        //Have to be asynchroinous so a task
        Task<string> GetServiceDetails();
    }
}
