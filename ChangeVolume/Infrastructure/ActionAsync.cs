using System.Threading.Tasks;

namespace ChangeVolume.Infrastructure
{
    internal delegate Task ActionAsync();

    internal delegate Task ActionAsync<in T>(T parameter);
}
