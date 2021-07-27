using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Communication;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;

namespace CommunicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunicationController : ControllerBase
    {
        [HttpGet]
        [Route("stateless")]
        public async Task<string> StatelessGet()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new System.Uri("fabric:/JumpstoreStore/CustomerAnalytics"));

            // This is why the method should be async 
            var serviceName = await statelessProxy.GetServiceDetails(); // Should return service name

            return serviceName;
        }
        
        [HttpGet]
        [Route("stateful")]
        public async Task<string> StatefulGet(
            [FromQuery] int productId)
        {
            var partitionId = productId % 3;

            // Wont pick up the region here as the settings to specify the partition name does not work
            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri("fabric:/JumpstoreStore/ProductCatalogue"), //, //
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(partitionId)); // Should pass a proxy

            // This is why the method should be async 
            var serviceName = await statefulProxy.GetServiceDetails(); // Should return service name and partition id

            return serviceName;
        }

        
    }
}
