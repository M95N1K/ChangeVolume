using Microsoft.Extensions.DependencyInjection;

namespace ChangeVolume.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
