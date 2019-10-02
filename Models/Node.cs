using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Viewer.Services;

namespace Viewer.Models
{
    public class Node: INotifyPropertyChanged
    {
        private string _name;
        private string _nodePath;
        private ObservableCollection<Node> _nodes;
        private NodeStyle _nodeStyle;
        private FontStyle _nodeFontStyle;
        private FontWeight _nodeFontWeight;

        public Node()
        {
            NodeStyle = StyleGetter.Get();
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string NodePath
        {
            get => _nodePath;
            set
            {
                _nodePath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Node> Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value; 
                OnPropertyChanged();
            }
        }

        public NodeStyle NodeStyle {
            get => _nodeStyle;
            set
            {
                _nodeStyle = value;
                NodeFontStyle = _nodeStyle.FontStyle;
                NodeFontWeight = _nodeStyle.FontWeight;
                OnPropertyChanged();
            }
        }

        public FontStyle NodeFontStyle
        {
            get => _nodeFontStyle;
            private set
            {
                _nodeFontStyle = value;
                OnPropertyChanged();
            }
        }

        public FontWeight NodeFontWeight
        {
            get => _nodeFontWeight;
            private set
            {
                _nodeFontWeight = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
