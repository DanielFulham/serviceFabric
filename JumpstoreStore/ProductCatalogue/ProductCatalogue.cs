using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Communication;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace ProductCatalogue
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalogue : StatefulService, IStatefulInterface
    {
        public ProductCatalogue(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<Product> GetProductById(int id)
        {
            var stateManager = this.StateManager;

            // We want to get the reliable dictionary now
            // When we retrieve this we can write or read from it going forward
            var productDict = await stateManager.GetOrAddAsync<IReliableDictionary<int, Product>>("productdict");

            using (var tx = stateManager.CreateTransaction())
            {
               var product = await productDict.TryGetValueAsync(tx, id);

                return product.Value;
            }

            //Assuming it has a value, can be handled better
            throw new Exception();
        }

        public async Task AddProduct(Product product)
        {
            var stateManager = this.StateManager;

            // We want to get the reliable dictionary now
            // When we retrieve this we can write or read from it going forward
            var productDict = await stateManager.GetOrAddAsync<IReliableDictionary<int, Product>>("productdict");

            using(var tx = stateManager.CreateTransaction())
            {
                // Adds new entry to dictionary in stateManager
                //await productDict.AddAsync(tx, product.Id, product);

                // We do this even if product exists it just updates it instead of throwing an exception (AddAsync above)
                await productDict.AddOrUpdateAsync(tx, product.Id, product, (k, v) => v);

                // Commits transaction
                await tx.CommitAsync();
            }
        }

        public async Task<string> GetServiceDetails() // Has to be async
        {
            var serviceName = this.Context.ServiceName.ToString();

            var partitionId = this.Context.PartitionId.ToString();

            return $"{serviceName} ::: {partitionId}";
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
            //return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
