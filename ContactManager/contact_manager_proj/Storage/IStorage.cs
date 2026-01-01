using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contacting
{
    public interface IStorage<T>
    {
        Task SaveAsync(IEnumerable<T> items);
        Task<List<T>> LoadAsync();
    }
}
