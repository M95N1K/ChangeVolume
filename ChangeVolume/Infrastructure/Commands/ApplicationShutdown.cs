using ChangeVolume.Infrastructure.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeVolume.Infrastructure.Commands
{
    class ApplicationShutdown : Command
    {
        protected override void Execute(object p)
        {
            App.Current.Shutdown();
        }
    }
}
