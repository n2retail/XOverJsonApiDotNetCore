using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources.Annotations;

namespace JsonApiDotNetCore.Resources
{
    /// <summary>
    /// Provides a resource-centric extensibility point for executing custom code when something happens with a resource. The goal here is to reduce the need
    /// for overriding the service and repository layers.
    /// </summary>
    /// <typeparam name="TResource">
    /// The resource type.
    /// </typeparam>
    [PublicAPI]
    public class JsonApiResourceDefinition<TResource> : JsonApiResourceDefinition<TResource, int>, IResourceDefinition<TResource>
        where TResource : class, IIdentifiable<int>
    {
        public JsonApiResourceDefinition(IResourceGraph resourceGraph)
            : base(resourceGraph)
        {
        }
    }

    /// <inheritdoc />
    [PublicAPI]
    public class JsonApiResourceDefinition<TResource, TId> : IResourceDefinition<TResource, TId>
        where TResource : class, IIdentifiable<TId>
    {
        protected IResourceGraph ResourceGraph { get; }

        // TODO: FAKE IMPLEMENTATION
        protected virtual List<AttrAttribute> OutputAttrs()
        {
            throw new NotImplementedException();
        }

        // TODO: FAKE IMPLEMENTATION
        protected List<AttrAttribute> Remove(Expression<Func<TResource, dynamic>> filter, List<AttrAttribute> from = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides metadata for the resource type <typeparamref name="TResource" />.
        /// </summary>
        protected ResourceContext ResourceContext { get; }

        public JsonApiResourceDefinition(IResourceGraph resourceGraph)
        {
            ArgumentGuard.NotNull(resourceGraph, nameof(resourceGraph));

            ResourceGraph = resourceGraph;
            ResourceContext = resourceGraph.GetResourceContext<TResource>();
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<IncludeElementExpression> OnApplyIncludes(IReadOnlyCollection<IncludeElementExpression> existingIncludes)
        {
            return existingIncludes;
        }

        /// <inheritdoc />
        public virtual FilterExpression OnApplyFilter(FilterExpression existingFilter)
        {
            return existingFilter;
        }

        /// <inheritdoc />
        public virtual SortExpression OnApplySort(SortExpression existingSort)
        {
            return existingSort;
        }

        /// <summary>
        /// Creates a <see cref="SortExpression" /> from a lambda expression.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// var sort = CreateSortExpressionFromLambda(new PropertySortOrder
        /// {
        ///     (model => model.CreatedAt, ListSortDirection.Ascending),
        ///     (model => model.Password, ListSortDirection.Descending)
        /// });
        /// ]]></code>
        /// </example>
        protected SortExpression CreateSortExpressionFromLambda(PropertySortOrder keySelectors)
        {
            ArgumentGuard.NotNull(keySelectors, nameof(keySelectors));

            var sortElements = new List<SortElementExpression>();

            foreach ((Expression<Func<TResource, dynamic>> keySelector, ListSortDirection sortDirection) in keySelectors)
            {
                bool isAscending = sortDirection == ListSortDirection.Ascending;
                AttrAttribute attribute = ResourceGraph.GetAttributes(keySelector).Single();

                var sortElement = new SortElementExpression(new ResourceFieldChainExpression(attribute), isAscending);
                sortElements.Add(sortElement);
            }

            return new SortExpression(sortElements);
        }

        /// <inheritdoc />
        public virtual PaginationExpression OnApplyPagination(PaginationExpression existingPagination)
        {
            return existingPagination;
        }

        /// <inheritdoc />
        public virtual SparseFieldSetExpression OnApplySparseFieldSet(SparseFieldSetExpression existingSparseFieldSet)
        {
            return existingSparseFieldSet;
        }

        /// <inheritdoc />
        public virtual QueryStringParameterHandlers<TResource> OnRegisterQueryableHandlersForQueryStringParameters()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual IDictionary<string, object> GetMeta(TResource resource)
        {
            return null;
        }

        /// <inheritdoc />
        public virtual Task OnPrepareWriteAsync(TResource resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task<IIdentifiable> OnSetToOneRelationshipAsync(TResource leftResource, HasOneAttribute hasOneRelationship,
            IIdentifiable rightResourceId, OperationKind operationKind, CancellationToken cancellationToken)
        {
            return Task.FromResult(rightResourceId);
        }

        /// <inheritdoc />
        public virtual Task OnSetToManyRelationshipAsync(TResource leftResource, HasManyAttribute hasManyRelationship, ISet<IIdentifiable> rightResourceIds,
            OperationKind operationKind, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task OnAddToRelationshipAsync(TId leftResourceId, HasManyAttribute hasManyRelationship, ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task OnRemoveFromRelationshipAsync(TResource leftResource, HasManyAttribute hasManyRelationship, ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task OnWritingAsync(TResource resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task OnWriteSucceededAsync(TResource resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void OnDeserialize(TResource resource)
        {
        }

        /// <inheritdoc />
        public virtual void OnSerialize(TResource resource)
        {
        }

        /// <summary>
        /// This is an alias type intended to simplify the implementation's method signature. See <see cref="CreateSortExpressionFromLambda" /> for usage
        /// details.
        /// </summary>
        public sealed class PropertySortOrder : List<(Expression<Func<TResource, dynamic>> KeySelector, ListSortDirection SortDirection)>
        {
        }
    }
}
