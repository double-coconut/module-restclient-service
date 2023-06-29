using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace RestClientService.Utils
{
    // Source of this settings https://stackoverflow.com/questions/18543482/is-there-a-way-to-ignore-get-only-properties-in-json-net-without-using-jsonignor
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            var newProps = props.Where(p => p.Writable && p.Readable).ToList();
            return newProps;
        }
    }
}