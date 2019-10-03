using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Viewer.Commands;
using Viewer.Enums.TreeEnums;
using Viewer.Models;
using Viewer.Services;


namespace Viewer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Node> _protocolNodes;
        private RelayCommand _loginToTopCommand;
        private RelayCommand _smObjectToTopCommand;
        private RelayCommand _confirmYearCommand;
        private RelayCommand _cancelYearCommand;
        private RelayCommand _exportCommand;
        private ObservableCollection<int> _years;
        private int? _selectedYear;
        private string _selectedYearText;
        private Visibility _isYearExceptionVisibility;
        private int? _confirmedSelectedYear;
        private TopNodeEnum _topNodeEnum;
        private Visibility _isWaitingVisibility;

        public ObservableCollection<Node> ProtocolNodes
        {
            get => _protocolNodes;
            set
            {
                _protocolNodes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Years
        {
            get => _years;
            set
            {
                _years = value;
                OnPropertyChanged();
            }
        }

        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                IsYearExceptionVisibility = Visibility.Collapsed;
                OnPropertyChanged();
            }
        }

        public string SelectedYearText
        {
            get => _selectedYearText;
            set
            {
                _selectedYearText = value;

                if (string.IsNullOrEmpty(_selectedYearText))
                {
                    IsYearExceptionVisibility = Visibility.Collapsed;
                }

                OnPropertyChanged();
            }
        }

        public Visibility IsYearExceptionVisibility
        {
            get => _isYearExceptionVisibility;
            set
            {
                _isYearExceptionVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsWaitingVisibility
        {
            get => _isWaitingVisibility;
            set
            {
                _isWaitingVisibility = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoginToTopCommand
        {
            get
            {
                return _loginToTopCommand ??
                       (_loginToTopCommand = new RelayCommand(obj =>
                       {
                           _topNodeEnum = TopNodeEnum.Login;
                           GetProtocolNodes();
                       }));
            }
        }

        public RelayCommand SmObjectToTopCommand
        {
            get
            {
                return _smObjectToTopCommand ??
                       (_smObjectToTopCommand = new RelayCommand(obj =>
                       {
                           _topNodeEnum = TopNodeEnum.SmObjectType;
                           GetProtocolNodes();
                       }));
            }
        }

        public RelayCommand ConfirmYearCommand
        {
            get
            {
                return _confirmYearCommand ??
                       (_confirmYearCommand = new RelayCommand(obj =>
                       {
                           if (IsYearExceptionVisibility == Visibility.Visible)
                           {
                               return;
                           }

                           int? year;
                           try
                           {
                               year = Convert.ToInt32(SelectedYearText);
                           }
                           catch (FormatException)
                           {
                               year = null;
                           }

                           if (string.IsNullOrEmpty(SelectedYearText) || year != null && Years.Contains((int)year))
                           {
                               _confirmedSelectedYear = SelectedYear;
                               GetProtocolNodes();
                           }
                           else
                           {
                               IsYearExceptionVisibility = Visibility.Visible;
                           }
                       }));
            }
        }

        public RelayCommand CancelYearCommand
        {
            get
            {
                return _cancelYearCommand ?? (_cancelYearCommand = new RelayCommand(obj =>
                {
                    SelectedYear = null;
                    SelectedYearText = null;
                    _confirmedSelectedYear = null;
                    GetProtocolNodes();
                }));
            }
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(obj =>
                {
                    IsWaitingVisibility = Visibility.Visible;
                    var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var filepath = $@"{dialog.FileName}\Protocol{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.xlsx";
                        ExportService.Export(filepath, ProtocolNodes);
                    }
                    IsWaitingVisibility = Visibility.Hidden;
                }));
            }
        }

        public MainViewModel()
        {
            GetProtocolNodes();
            Years = DataService.GetAllYears();
            IsYearExceptionVisibility = Visibility.Collapsed;
            IsWaitingVisibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void GetProtocolNodes()
        {
            ProtocolNodes = GroupService.Group(_topNodeEnum, _confirmedSelectedYear);
        }
    }
}
