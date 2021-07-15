using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeVolume.Models
{
    public class MOptions
    {
        public bool HideToStart { get; set; }

        public MHotKeySettings HotKeySettings { get; set; }

        public MOptions()
        {
            HideToStart = false;
            HotKeySettings = new();
        }
    }
}
