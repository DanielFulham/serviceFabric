using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatefulInterface : IService // This is a service fabric thing
    {
        //Have to be asynchroinous so a task
        Task<string> GetServiceDetails();
    }
}
