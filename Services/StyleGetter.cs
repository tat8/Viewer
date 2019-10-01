using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Viewer.Models;

namespace Viewer.Services
{
    public static class StyleGetter
    {
        private static List<NodeStyle> _nodeStyles = new List<NodeStyle>
        {
            new NodeStyle{FontWeight = FontWeights.SemiBold, FontStyle = FontStyles.Normal},
            new NodeStyle{FontWeight = FontWeights.SemiBold, FontStyle = FontStyles.Italic},
            new NodeStyle{FontWeight = FontWeights.Normal, FontStyle = FontStyles.Italic},
            new NodeStyle{FontWeight = FontWeights.Normal, FontStyle = FontStyles.Normal}
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"> (нумерация с нуля) </param>
        /// <returns></returns>
        public static NodeStyle Get(int level)
        {
            return level >= _nodeStyles.Count ? GetDefault() : _nodeStyles[level];
        }

        public static NodeStyle GetDefault()
        {
            return _nodeStyles[_nodeStyles.Count - 1];
        }
    }
}
