using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JsonApiDotNetCore.AtomicOperations;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Serialization.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiDotNetCore.Controllers
{
    /// <summary>
    /// The base class to derive atomic:operations controllers from. This class delegates all work to <see cref="BaseJsonApiOperationsController" /> but adds
    /// attributes for routing templates. If you want to provide routing templates yourself, you should derive from BaseJsonApiOperationsController directly.
    /// </summary>
    public abstract class JsonApiOperationsController : BaseJsonApiOperationsController
    {
        protected JsonApiOperationsController(IJsonApiOptions options, ILoggerFactory loggerFactory, IOperationsProcessor processor, IJsonApiRequest request,
            ITargetedFields targetedFields)
            : base(options, loggerFactory, processor, request, targetedFields)
        {
        }

        // TODO: FAKE IMPLEMENTATION
        protected JsonApiOperationsController(IOperationsProcessor operationsProcessor) : base(null, null, null, null, null)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<IActionResult> PatchAsync([FromBody] OperationsDocument doc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        [HttpPost]
        public override async Task<IActionResult> PostOperationsAsync([FromBody] IList<OperationContainer> operations, CancellationToken cancellationToken)
        {
            return await base.PostOperationsAsync(operations, cancellationToken);
        }
    }

    // TODO: FAKE IMPLEMENTATION
    public class OperationsDocument
    {
        public OperationsDocument() { }
        public OperationsDocument(List<Operation> operations)
        {
            Operations = operations;
        }

        [JsonProperty("operations")]
        public List<Operation> Operations { get; set; }
    }

    // TODO: FAKE IMPLEMENTATION
    public enum OperationCode
    {
        get = 1,
        add = 2,
        update = 3,
        remove = 4
    }

    public class ResourceReference : ResourceIdentifierObject
    {
        [JsonProperty("relationship")]
        public string Relationship { get; set; }
    }

    public class Params
    {
        public List<string> Include { get; set; }
        public List<string> Sort { get; set; }
        public Dictionary<string, object> Filter { get; set; }
        public string Page { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }


    // TODO: FAKE IMPLEMENTATION
    public class Operation
         {
             public OperationCode Op { get; set; }

             public ResourceReference Ref { get; set; }

             public Params Params { get; set; }

             public object Data
             {
                 get
                 {
                     if (DataIsList) return DataList;
                     return DataObject;
                 }
                 set => SetData(value);
             }

             private void SetData(object data)
             {
                 if (data is JArray jArray)
                 {
                     DataIsList = true;
                     DataList = jArray.ToObject<List<ResourceObject>>();
                 }
                 else if (data is List<ResourceObject> dataList)
                 {
                     DataIsList = true;
                     DataList = dataList;
                 }
                 else if (data is JObject jObject)
                 {
                     DataObject = jObject.ToObject<ResourceObject>();
                 }
                 else if (data is ResourceObject dataObject)
                 {
                     DataObject = dataObject;
                 }
             }

             [JsonIgnore]
             public bool DataIsList { get; private set; }

             [JsonIgnore]
             public List<ResourceObject> DataList { get; private set; }

             [JsonIgnore]
             public ResourceObject DataObject { get; private set; }

             public string GetResourceTypeName()
             {
                 if (Ref != null)
                     return Ref.Type?.ToString();

                 if (DataIsList)
                     return DataList[0].Type?.ToString();

                 return DataObject.Type?.ToString();
             }
         }
}
