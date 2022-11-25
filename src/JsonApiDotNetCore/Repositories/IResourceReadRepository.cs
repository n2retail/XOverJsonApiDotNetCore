using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Internal.Query;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources;

namespace JsonApiDotNetCore.Repositories
{
    /// <inheritdoc />
    public interface IResourceReadRepository<TResource> : IResourceReadRepository<TResource, int>
        where TResource : class, IIdentifiable<int>
    {
    }

    /// <summary>
    /// Groups read operations.
    /// </summary>
    /// <typeparam name="TResource">
    /// The resource type.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The resource identifier type.
    /// </typeparam>
    [PublicAPI]
    public interface IResourceReadRepository<TResource, in TId>
        where TResource : class, IIdentifiable<TId>
    {
        /// <summary>
        /// Executes a read query using the specified constraints and returns the collection of matching resources.
        /// </summary>
        Task<IReadOnlyCollection<TResource>> GetAsync(QueryLayer layer, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a read query using the specified top-level filter and returns the top-level count of matching resources.
        /// </summary>
        Task<int> CountAsync(FilterExpression topFilter, CancellationToken cancellationToken);

        // TODO: FAKE IMPLEMENTATION
        IQueryable<TResource> Filter(IQueryable<TResource> entities, FilterQuery filterQuery);

        // TODO: FAKE IMPLEMENTATION
        Task<IEnumerable<TResource>> PageAsync(IQueryable<TResource> entities, int pageSize,
            int pageNumber);

        // TODO: FAKE IMPLEMENTATION
        Task<int> CountAsync(IQueryable<TResource> entities);

        // TODO: FAKE IMPLEMENTATION
        IQueryable<TResource> Get();

        // TODO: FAKE IMPLEMENTATION
        Task<TResource> GetAsync(TId id);

        // TODO: FAKE IMPLEMENTATION
        Task<IEnumerable<TResource>> GetAsync();

        // TODO: FAKE IMPLEMENTATION
        IQueryable<TResource> Include(IQueryable<TResource> entities, string relationshipName);

        // TODO: FAKE IMPLEMENTATION
        Task<TResource> GetAndIncludeAsync(TId id, string relationshipName);

        // TODO: FAKE IMPLEMENTATION
        IQueryable<TResource> Sort(IQueryable<TResource> entities, List<SortQuery> sortQueries);


    }
}
