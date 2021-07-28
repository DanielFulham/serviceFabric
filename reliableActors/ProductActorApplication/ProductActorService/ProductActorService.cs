using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ProductActorService.Interfaces;
using Contracts;

namespace ProductActorService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class ProductActorService : Actor, IProductActorService, IRemindable //, IProductActorService
    {
        //So this could be Everton V United
        private string ProductStateName = "ProductState";

        private IActorTimer _actorTimer;

        /// <summary>
        /// Initializes a new instance of ProductActorService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ProductActorService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {

        }

        public async Task AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            await this.StateManager.AddOrUpdateStateAsync(ProductStateName, product, (k, v) => product, cancellationToken);

            await this.StateManager.SaveStateAsync(cancellationToken);
        }

        public async Task<Product> GetProductAsync(CancellationToken cancellationToken)
        {
            var product = await this.StateManager.GetStateAsync<Product>(ProductStateName, cancellationToken);

            return product;
        }

        protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"{actorMethodContext.MethodName} has finished");

            return base.OnPostActorMethodAsync(actorMethodContext);
        }
        protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            ActorEventSource.Current.ActorMessage(this, $"{actorMethodContext.MethodName} will start soon");

            return base.OnPreActorMethodAsync(actorMethodContext);
        }

        protected override Task OnDeactivateAsync()
        {
            //if (_actorTimer != null)
            //{
            //    UnregisterTimer(_actorTimer);
            //}

            ActorEventSource.Current.ActorMessage(this, "Actor deactivated.");

            return base.OnDeactivateAsync();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            this.RegisterReminderAsync("TaskReminder", null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));

            //_actorTimer = RegisterTimer(
            //    DoWork,
            //    null,
            //    TimeSpan.FromSeconds(10),
            //    TimeSpan.FromSeconds(150));

            //ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        private async Task DoWork(object work)
        {
            // Below is an example of how these functions can be detrimental to halt the actor performing tasks using the time.
            await Task.Delay(TimeSpan.FromSeconds(100));

            ActorEventSource.Current.ActorMessage(this, $"Actor is doing work");

            // return Task.CompletedTask;
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == "TaskReminder")
            {
                ActorEventSource.Current.ActorMessage(this, $"Reminder is doing work");
            }
        }

        ///// <summary>
        ///// TODO: Replace with your own actor method.
        ///// </summary>
        ///// <returns></returns>
        //Task<int> IProductActorService.GetCountAsync(CancellationToken cancellationToken)
        //{
        //    return this.StateManager.GetStateAsync<int>("count", cancellationToken);
        //}

        ///// <summary>
        ///// TODO: Replace with your own actor method.
        ///// </summary>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //Task IProductActorService.SetCountAsync(int count, CancellationToken cancellationToken)
        //{
        //    // Requests are not guaranteed to be processed in order nor at most once.
        //    // The update function here verifies that the incoming count is greater than the current count to preserve order.
        //    return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
        //}
    }
}
