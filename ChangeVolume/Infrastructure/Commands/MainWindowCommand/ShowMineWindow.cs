using ChangeVolume.Infrastructure.Commands.Base;
using System.Windows;

namespace ChangeVolume.Infrastructure.Commands.MainWindowCommand
{
    internal class ShowMineWindow : Command
    {
        protected override bool CanExecute(object p) => Application.Current.MainWindow != null;
        protected override void Execute(object p)
        {
            Window w = Application.Current.MainWindow;

            w.Visibility = Visibility.Visible;
            w.IsEnabled = true;
        }
    }
}
