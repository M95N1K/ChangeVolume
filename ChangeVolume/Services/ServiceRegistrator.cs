using ChangeVolume.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using HotKeyLibrary;
using SetAppVolume;

namespace ChangeVolume.Services
{
    static class ServiceRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
           .AddTransient<IOptionsService, OptionsService>()
           //.AddTransient<IUserDialog, UserDialog>()
           .AddTransient<IHotKeys, HotKeys>()
           .AddTransient<IAppVolume, AppVolume>()
        ;
    }
}
