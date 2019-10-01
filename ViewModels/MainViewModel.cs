using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Viewer.Commands;
using Viewer.Models;
using Viewer.Services;
using static Viewer.Enums.Enums.TopNodeType;

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
        private int? _confirmedSelectedYear;
        private Enums.Enums.TopNodeType _topNodeType;

        public ObservableCollection<Node> ProtocolNodes
        {
            get => _protocolNodes;
            set
            {
                _protocolNodes = value;
                OnPropertyChanged("ProtocolNodes");
            }
        }

        public ObservableCollection<int> Years
        {
            get => _years;
            set
            {
                _years = value;
                OnPropertyChanged("Years");
            }
        }

        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged("SelectedYear");
            }
        }

        public RelayCommand LoginToTopCommand
        {
            get
            {
                return _loginToTopCommand ??
                       (_loginToTopCommand = new RelayCommand(obj =>
                       {
                           _topNodeType = Login;
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
                           _topNodeType = SmObjectType;
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
                           _confirmedSelectedYear = SelectedYear; 
                           GetProtocolNodes();
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
                    var filepath = $@"F:\test\Protocol{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.xlsx";
                    ExportService.Export(filepath, ProtocolNodes);
                }));
            }
        }

        public MainViewModel()
        {
            GetProtocolNodes();
            Years = DataService.GetAllYears();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void GetProtocolNodes()
        {
            ProtocolNodes = GroupService.Group(_topNodeType, _confirmedSelectedYear);
        }
    }
}
