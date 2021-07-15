using ChangeVolume.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ChangeVolume.Services.Interfaces
{
    internal interface IOptionsService
    {
        public MOptions Options { get; set; }

        public void SaveOptions();
        public void LoadOptions();
    }
}
