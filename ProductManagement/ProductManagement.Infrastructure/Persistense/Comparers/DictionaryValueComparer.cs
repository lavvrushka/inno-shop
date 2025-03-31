using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ProductManagement.Infrastructure.Persistense.Comparers
{
    public class DictionaryValueComparer : ValueComparer<Dictionary<Guid, DateTime>>
    {
        public DictionaryValueComparer()
            : base(
                (c1, c2) => JsonConvert.SerializeObject(c1) == JsonConvert.SerializeObject(c2),
                c => c == null ? 0 : JsonConvert.SerializeObject(c).GetHashCode(),
                c => c == null ? new Dictionary<Guid, DateTime>() : JsonConvert.DeserializeObject<Dictionary<Guid, DateTime>>(JsonConvert.SerializeObject(c))
            )
        { }
    }
}
