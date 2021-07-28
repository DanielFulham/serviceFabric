using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using ProductActorService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        [HttpGet]
        public async Task<Product> GetProductById(
            [FromQuery] int id)
        {
            var actorId = new ActorId(id);

            var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"));
            //var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/applicationName/serviceName"));

            var product = await proxy.GetProductAsync(new CancellationToken());

            return product;
        }

        [HttpPost]
        public async Task AddProduct(
            [FromBody] Product product)
        {
            var actorId = new ActorId(product.Id);

            var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"));
            //var proxy = ActorProxy.Create<IProductActorService>(actorId, new Uri("fabric:/applicationName/serviceName"));

            await proxy.AddProductAsync(product, new CancellationToken());
        }

        [HttpDelete]
        public async Task DeleteActorById(
            [FromQuery] int id)
        {
            var actorId = new ActorId(id);

            // we'#re connecting to actor service, not the actor here

            var actorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/ProductActorApplication/ProductActorServiceActorService"),
                actorId);

            // This is an expensive operation. Maybe it could be done every so often (once a week).
            // or maybe some business logic against a threashold
            await actorServiceProxy.DeleteActorAsync(actorId, new CancellationToken());
        }

    }
}
