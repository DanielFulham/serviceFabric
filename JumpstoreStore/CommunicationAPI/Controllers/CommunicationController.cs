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
                new Uri("fabric:/JumpstoreStore/CustomerAnalytics"));

            // This is why the method should be async 
            var serviceName = await statelessProxy.GetServiceDetails(); // Should return service name

            return serviceName;
        }

        [HttpPost]
        [Route("addproduct")]
        public async Task AddProduct(
            [FromBody] Product product)
        {
            var partitionId = product.Id % 3; // % 3 to proxy in

            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri("fabric:/JumpstoreStore/ProductCatalogue"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(partitionId)); // Should pass a proxy

            // Add product
            await statefulProxy.AddProduct(product);
        }

        [HttpPost]
        [Route("addtoqueue")]
        public async Task AddToQueue(
            [FromQuery] int partitionId,
            [FromBody] Product product)
        {
            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri("fabric:/JumpstoreStore/ProductCatalogue"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(partitionId)); // Should pass a proxy

            // Add product
            await statefulProxy.AddToQueue(product);
        }

        [HttpGet]
        [Route("getfromqueue")]
        public async Task<Product> GetFromQueue(
            [FromQuery] int partitionId)
        {
            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri("fabric:/JumpstoreStore/ProductCatalogue"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(partitionId));

            var product = await statefulProxy.GetFromQueue();

            return product;
        }

        [HttpGet]
        [Route("getproduct")]
        public async Task<Product> GetProduct(
            [FromQuery] int productId)
        {
            var partitionId = productId % 3;

            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri("fabric:/JumpstoreStore/ProductCatalogue"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(partitionId));

            var product = await statefulProxy.GetProductById(productId);

            return product;
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
