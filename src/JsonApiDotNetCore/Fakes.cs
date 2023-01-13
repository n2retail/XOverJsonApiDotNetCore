using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Internal.Query;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using JsonApiDotNetCore.Serialization.Objects;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace JsonApiDotNetCore.Services
{
    public interface IScopedServiceProvider : IServiceProvider { }

    public class ContextEntity
    {
        /// <summary>
        /// The exposed resource name
        /// </summary>
        public string EntityName {
            get;
            set; }

        /// <summary>
        /// The data model type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// The identity member type
        /// </summary>
        public Type IdentityType { get; set; }

        /// <summary>
        /// The concrete <see cref="ResourceDefinition{T}"/> type.
        /// We store this so that we don't need to re-compute the generic type.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Exposed resource attributes
        /// </summary>
        public List<AttrAttribute> Attributes { get; set; }

        /// <summary>
        /// Exposed resource relationships
        /// </summary>
        public List<RelationshipAttribute> Relationships { get; set; }

        /// <summary>
        /// Links to include in resource responses
        /// </summary>
        public LinkTypes Links { get; set; } = LinkTypes.All;
    }

    public interface IJsonApiContext
    {
        QuerySet QuerySet { get; set; }
        Options Options { get; set; }
        List<string> IncludedRelationships { get; set; }
        PageManager PageManager { get; set; }
        IResourceGraph ResourceGraph { get; set; }
        Dictionary<AttrAttribute, object> AttributesToUpdate { get; set; }
        IEnumerable<FakeRelationship> RelationshipsToUpdate { get; set; }
        ContextEntity RequestEntity { get; set; }
        IJsonApiContext ApplyContext<T>(object controller);
        void BeginOperation();
    }

    public interface IControllerContext
    {
        Type ControllerType { get; set; }
        ContextEntity RequestEntity { get; set; }
        TAttribute GetControllerAttribute<TAttribute>() where TAttribute : Attribute;
    }

    public interface FakeAttr
    {
        FakeKey Key { get; set; }
        string Value { get; set; }
    }

    public interface FakeKey
    {
        void SetValue(object oldItem, string attrValue);
    }

    public interface FakeResourceGraph
    {
        object GetPublicAttributeName<T>(string errPropertyName);
    }

    // public interface FakeContextEntity
    // {
    //     List<AttrAttribute> Attributes { get; set; }
    //     List<RelationshipAttribute> Relationships { get; set; }
    // }

    public interface FakeRelationship
    {
        string InternalRelationshipName { get; set; }
        FakeKey Key { get; set; }
        string Value { get; set; }
    }



    public interface PageManager
    {
        int TotalRecords { get; set; }
        bool IsPaginated { get; set; }
        int CurrentPage { get; set; }
        int PageSize { get; set; }
    }

    public interface Options
    {
        bool AllowClientGeneratedIds { get; set; }
    }

    public interface QuerySet
    {
        public List<FilterQuery> Filters { get; set; }
        public PageQuery PageQuery { get; set; }
        public List<SortQuery> SortParameters { get; set; }
        public List<string> IncludedRelationships { get; set; }
        public List<string> Fields { get; set; }
    }

    public class PageQuery
    {
        public int PageSize { get; set; }
        public int PageOffset { get; set; } = 1;
    }
}

namespace JsonApiDotNetCore.Serialization
{
    public interface IJsonApiDeSerializer
    {
        object Deserialize(string requestBody);
        TEntity Deserialize<TEntity>(string requestBody);
        object DeserializeRelationship(string requestBody);
        List<TEntity> DeserializeList<TEntity>(string requestBody);
        object DocumentToObject(ResourceObject data, List<ResourceObject> included = null);
    }
}


namespace JsonApiDotNetCore.Controllers
{
    public class JsonApiControllerMixin
    {
        protected IActionResult Forbidden()
        {
            throw new NotImplementedException();
        }

        protected IActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        protected IActionResult Ok(object stuff)
        {
            throw new NotImplementedException();
        }

        protected IActionResult Errors(object stuff)
        {
            throw new NotImplementedException();
        }



        protected IActionResult Error(Error error)
        {
            throw new NotImplementedException();
        }

        protected IActionResult Content(string blah)
        {
            throw new NotImplementedException();
        }

        protected IActionResult Errors(ErrorCollection errors)
        {
            throw new NotImplementedException();
        }
    }
}

namespace JsonApiDotNetCore.Services
{
    public interface IQueryAccessor
    {
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Gets the query value and throws a if it is not present.
        /// If the exception is not caught, the middleware will return an HTTP 422 response.
        /// </summary>
        /// <exception cref="JsonApiException" />
        T GetRequired<T>(string key);
    }
}

namespace JsonApiDotNetCore.Builders
{
    public interface IDocumentBuilder
    {
        ResourceObject GetData<T>(ContextEntity getContextEntity, T resource) where T : Identifiable<Guid>;
    }
}

namespace JsonApiDotNetCore.Internal.Query
{
    public static class QueryConstants {
        public const string FILTER = "filter";
        public const string SORT = "sort";
        public const string INCLUDE = "include";
        public const string PAGE = "page";
        public const string FIELDS = "fields";
        public const char OPEN_BRACKET = '[';
        public const char CLOSE_BRACKET = ']';
        public const char COMMA = ',';
        public const char COLON = ':';
        public const string COLON_STR = ":";
        public const char DOT = '.';

    }
    public enum SortDirection
    {
        Ascending = 1,
        Descending = 2
    }
    public enum FilterOperations
    {
        eq = 0,
        lt = 1,
        gt = 2,
        le = 3,
        ge = 4,
        like = 5,
        ne = 6,
        @in = 7, // prefix with @ to use keyword
        nin = 8,
        isnull = 9,
        isnotnull = 10
    }

    public abstract class BaseQuery
    {
        public BaseQuery(string attribute)
        {
            var properties = attribute.Split(QueryConstants.DOT);
            if(properties.Length > 1)
            {
                Relationship = properties[0];
                Attribute = properties[1];
            }
            else
                Attribute = properties[0];
        }

        public string Attribute { get; }
        public string Relationship { get; }
        public bool IsAttributeOfRelationship => Relationship != null;
    }

    public class SortQuery : BaseQuery
    {
        public SortQuery(SortDirection direction, string attribute)
            : base(attribute)
        {
            Direction = direction;
        }

        public string Attribute { get; set; }
        public SortDirection Direction { get; set; }
        public string Relationship { get; set; }
    }

    public class FilterQuery
    {
        public FilterQuery(string attribute, string operation, string value)
        {
            Attribute = attribute;
            Operation = operation;
            Value = value;
        }

        public string Attribute { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }
        public string Relationship { get; set; }
    }
}
