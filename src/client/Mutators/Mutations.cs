using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Olav.Sanity.Client.Mutators
{
    public class Mutations
    {
        [JsonProperty(PropertyName = "Mutations")]
        private readonly List<object> _mutations;


        public Mutations() => _mutations = new List<object>();

        public Mutations AddCreate(ISanityType obj)
        {
            _mutations.Add(new CreateMutator{Create = obj});
            return this;
        }
        public Mutations AddCreateOrReplace(ISanityType obj)
        {
            _mutations.Add(new CreateOrReplaceMutator{CreateOrReplace = obj});
            return this;
        }

        public Mutations AddCreateIfNotExists(ISanityType obj)
        {
            _mutations.Add(new CreateIfNotExistsMutator{CreateIfNotExists = obj});
            return this;
        }

        public Mutations AddDelete(IHaveId obj)
        {
            _mutations.Add(new DeleteMutator{Delete = obj});
            return this;
        }

        public Mutations AddPatch(object obj)
        {
            _mutations.Add(new PatchMutator{Patch = obj});
            return this;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(
                        this,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            NullValueHandling = NullValueHandling.Ignore
                        }
                    );
        }
    }
}
