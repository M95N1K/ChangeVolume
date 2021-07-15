using ChangeVolume.Infrastructure.Commands;
using ChangeVolume.Services.Interfaces;
using ChangeVolume.ViewModels.Base;
using HotKeyLibrary;
using SetAppVolume;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChangeVolume.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);

        private readonly IAppVolume _appVolume;
        private readonly IHotKeys _hotKeys;
        private readonly IOptionsService _options;

        string process { get => GetActiveWindowProcessName(); }
        int step = 10;

        #region TextProperty - Текстовые свойства для изменения локализации

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Change Volume - Options";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region Status : string - Статус

        /// <summary>Статус</summary>
        private string _Status = "Готов!";

        /// <summary>Статус</summary>
        public string Status { get => _Status; set => Set(ref _Status, value); }

        #endregion

        #region VolUPText: string - Текст кнопки VolUP
        private string _volUpText = "VolUp";
        public string VolUPText { get => _volUpText; set => Set(ref _volUpText, value); }
        #endregion

        #region MuteText: string = Текст кнопки Mute
        private string _muteText = "Mute";
        public string MuteText { get => _muteText; set => Set(ref _muteText, value); }
        #endregion

        #region VolDownText: string - Текст кнопки VolDown
        private string _volDownText = "VolDown";
        public string VolDownText { get => _volDownText; set => Set(ref _volDownText, value); }
        #endregion

        #region ExitText: string - Текст кнопки Exit
        private string _exitText = "Exit";
        public string ExitText { get => _exitText; set => Set(ref _exitText, value); }
        #endregion

        #region RunText: string - Текст кнопки вкл/выкл отслеживания горячих клавиш
        private string _runText = "RUN";
        public string RunText { get => _runText; set => Set(ref _runText, value); }
        #endregion

        #region HideToStartText: string - Текст скрывать или нет при старте
        private string _HideToStartText = "Скрывать при старте";
        /// <summary>Текст скрывать или нет при старте</summary>
        public string HideToStartText
        {
            get => _HideToStartText;
            set => Set(ref _HideToStartText, value);
        }
        #endregion


        #endregion

        #region Commands

        #region MinimizeWindowCommand
        public LambdaCommand MinimizeWindowCommand { get; }
        private void OnMinimizeWindowCommandExecute()
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        #endregion

        #region VolUpCommand
        public LambdaCommand VolUpCommand { get; }
        private void OnVolUpCommandExecuted()
        {
            _hotKeys.OnHotKeyString += _hotKeys_OnVolUpPresset;
            _hotKeys.OnHotKeyString -= _hotKeys_OnHotKeyPresset;
        }
        #endregion

        #region MuteCommand - Команда установки горячей клавиши для Mute
        public LambdaCommand MuteCommand { get; }
        private void OnMuteCommandExecuted()
        {
            _hotKeys.OnHotKeyString += _hotKeys_OnMutePresset;
            _hotKeys.OnHotKeyString -= _hotKeys_OnHotKeyPresset;
        }
        #endregion

        #region VolDownCommand - Команда установки горячей клавиши для VolDown
        /// <summary>Команда установки горячей клавиши для VolDown</summary>
        public LambdaCommand VolDownCommand { get; }
        private void OnVolDownCommandExecuted()
        {
            _hotKeys.OnHotKeyString += _hotKeys_OnVolDownPresset;
            _hotKeys.OnHotKeyString -= _hotKeys_OnHotKeyPresset;
        }
        #endregion

        #region StartStopCommand - Команда запуска / остановки отслеживания нажатия клавишь
        /// <summary>Команда запуска / остановки отслеживания нажатия клавишь</summary>
        public LambdaCommand StartStopCommand { get; }
        private void OnStartStopCommandExecuted()
        {
            if (_hotKeys.IsEnable)
                _hotKeys.StopHK();
            else
                _hotKeys.StartHK();
            StateWork = _hotKeys.IsEnable;
        }
        #endregion

        #endregion

        #region State - состояния объектов

        #region HideToStart: bool - Скрывать при старте или нет
        /// <summary>Скрывать при старте или нет</summary>
        public bool HideToStart
        {
            get => _options.Options.HideToStart;
            set
            {
                if (_options.Options.HideToStart != value)
                {
                    _options.Options.HideToStart = value;
                    _options.SaveOptions();
                    OnPropertyChanged(nameof(HideToStart));
                }
            }
        }
        #endregion

        #region VisabilityState: Visibility - Состояние видимости окна
        private Visibility _VisabilityState = default;
        /// <summary>Состояние видимости окна</summary>
        public Visibility VisabilityState
        {
            get => _VisabilityState;
            set => Set(ref _VisabilityState, value);
        }
        #endregion

        #region StateWork: bool - Запущено отслеживание или нет
        private bool _StateWork;
        /// <summary>Запущено отслеживание или нет</summary>
        public bool StateWork
        {
            get => _StateWork;
            set => Set(ref _StateWork, value);
        }
        #endregion

        #region VUHotKey: string - Горячая клавиша VolUp
        /// <summary>Горячая клавиша VolUp</summary>
        public string VUHotKey
        {
            get => _options.Options.HotKeySettings.VolUpKey;
            set 
            { 
                if(_options.Options.HotKeySettings.VolUpKey != value)
                {
                    _options.Options.HotKeySettings.VolUpKey = value;
                    _options.SaveOptions();
                    OnPropertyChanged(nameof(VUHotKey));
                }
            }
        }
        #endregion

        #region MuteHotKey: string - Горячая клавиша для Mute
        /// <summary>Горячая клавиша для Mute</summary>
        public string MuteHotKey
        {
            get => _options.Options.HotKeySettings.MuteKey;
            set
            {
                if (_options.Options.HotKeySettings.MuteKey != value)
                {
                    _options.Options.HotKeySettings.MuteKey = value;
                    _options.SaveOptions();
                    OnPropertyChanged(nameof(MuteHotKey));
                }
            }
        }
        #endregion

        #region VDHotKey: string - Горячая клавиша для VolDown
        /// <summary>Горячая клавиша для VolDown</summary>
        public string VDHotKey
        {
            get => _options.Options.HotKeySettings.VolDownKey;
            set
            {
                if (_options.Options.HotKeySettings.VolDownKey != value)
                {
                    _options.Options.HotKeySettings.VolDownKey = value;
                    _options.SaveOptions();
                    OnPropertyChanged(nameof(VDHotKey));
                }
            }
        }
        #endregion


        #endregion
        public MainWindowViewModel(IAppVolume appVolume, IHotKeys hotKeys,
            IOptionsService options)
        {
            _appVolume = appVolume;
            _hotKeys = hotKeys;
            _options = options;

            if (options.Options.HideToStart)
                VisabilityState = Visibility.Hidden;

            _hotKeys.StartHK();
            StateWork = _hotKeys.IsEnable;

            _hotKeys.OnHotKeyString += _hotKeys_OnHotKeyPresset;

            MinimizeWindowCommand = new LambdaCommand(OnMinimizeWindowCommandExecute);
            VolUpCommand = new LambdaCommand(OnVolUpCommandExecuted, () => StateWork);
            MuteCommand = new LambdaCommand(OnMuteCommandExecuted, () => StateWork);
            VolDownCommand = new LambdaCommand(OnVolDownCommandExecuted, () => StateWork);
            StartStopCommand = new LambdaCommand(OnStartStopCommandExecuted);
        }

        #region HotKeyPressetMetods

        string tmpKey = "";
        private void _hotKeys_OnHotKeyPresset(string obj)
        {
            try
            {
                if (obj.Equals(VUHotKey))
                {
                    Task.Run(() => _appVolume.SetAppVolume(process, _appVolume.GetAppVolume(process) + step)).Wait();
                    Status = ProcessVolumeInfo();
                }
                else if (obj.Equals(MuteHotKey))
                {
                    Task.Run(() => _appVolume.SetMuteApp(process, !_appVolume.GetMuteApp(process))).Wait();
                    Status = ProcessVolumeInfo();
                }
                else if (obj.Equals(VDHotKey))
                {
                    Task.Run(() => _appVolume.SetAppVolume(process, _appVolume.GetAppVolume(process) - step)).Wait();
                    Status = ProcessVolumeInfo();
                }
            }
            catch { }

        }

        private void _hotKeys_OnVolUpPresset(string p)
        {
            if (p == "-1" && !string.IsNullOrEmpty(tmpKey))
            {
                _hotKeys.OnHotKeyString -= _hotKeys_OnVolUpPresset;
                _hotKeys.OnHotKeyString += _hotKeys_OnHotKeyPresset;
                VUHotKey = tmpKey;
                tmpKey = "";
            }
            else if (p.Length > 2)
            {
                tmpKey = p;
            }

        }

        private void _hotKeys_OnMutePresset(string p)
        {
            if (p == "-1" && !string.IsNullOrEmpty(tmpKey))
            {
                _hotKeys.OnHotKeyString -= _hotKeys_OnMutePresset;
                _hotKeys.OnHotKeyString += _hotKeys_OnHotKeyPresset;
                MuteHotKey = tmpKey;
                tmpKey = "";
            }
            else if (p.Length > 2)
            {
                tmpKey = p;
            }

        }

        private void _hotKeys_OnVolDownPresset(string p)
        {
            if (p == "-1" && !string.IsNullOrEmpty(tmpKey))
            {
                _hotKeys.OnHotKeyString -= _hotKeys_OnVolDownPresset;
                _hotKeys.OnHotKeyString += _hotKeys_OnHotKeyPresset;
                VDHotKey = tmpKey;
                tmpKey = "";
            }
            else if (p.Length > 2)
            {
                tmpKey = p;
            }

        }

        #endregion

        private string GetActiveWindowProcessName()
        {
            IntPtr handl = GetForegroundWindow();
            int pid = 0;
            GetWindowThreadProcessId(handl, ref pid);
            Process p = Process.GetProcessById(pid);
            return p.ProcessName;
        }

        private string ProcessVolumeInfo()
        {
            int vol = Task.Run(() => _appVolume.GetAppVolume(process)).Result;
            bool mute = Task.Run(() => _appVolume.GetMuteApp(process)).Result;
            if (mute)
                return $"{process} - Muted";
            return $"{process} - Vol: {vol}";
        }

    }
}
