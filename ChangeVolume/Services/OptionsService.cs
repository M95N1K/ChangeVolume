using ChangeVolume.Models;
using ChangeVolume.Services.Interfaces;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace ChangeVolume.Services
{
    class OptionsService : IOptionsService
    {
        private string fileName = "config.cfg";
        public MOptions Options { get; set; }
        public void SaveOptions()
        {
            var serialize = JsonSerializer.Serialize(Options);
            var optionFile = File.Create(fileName);
            byte[] bytes = Encoding.UTF8.GetBytes(serialize);
            optionFile.Write(bytes);
            optionFile.Close();
        }

        public void LoadOptions()
        {
            if(!File.Exists(fileName))
            {
                Options = new MOptions();
                SaveOptions();
            }
            var serialize = File.ReadAllText(fileName);
            Options = JsonSerializer.Deserialize<MOptions>(serialize);
        }

        public OptionsService()
        {
            LoadOptions();
        }
    }
}
