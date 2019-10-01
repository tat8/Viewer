using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            NodeStyle = StyleGetter.GetDefault();
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string NodePath
        {
            get => _nodePath;
            set
            {
                _nodePath = value;
                OnPropertyChanged("NodePath");
            }
        }

        public ObservableCollection<Node> Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value; 
                OnPropertyChanged("Nodes");
            }
        }

        public NodeStyle NodeStyle {
            get => _nodeStyle;
            set
            {
                _nodeStyle = value;
                NodeFontStyle = _nodeStyle.FontStyle;
                NodeFontWeight = _nodeStyle.FontWeight;
                OnPropertyChanged("NodeStyle");
            }
        }

        public FontStyle NodeFontStyle
        {
            get => _nodeFontStyle;
            private set
            {
                _nodeFontStyle = value;
                OnPropertyChanged("NodeFontStyle");
            }
        }

        public FontWeight NodeFontWeight
        {
            get => _nodeFontWeight;
            private set
            {
                _nodeFontWeight = value;
                OnPropertyChanged("NodeFontWeight");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
