using System.Collections.Generic;
using System.Windows;
using Viewer.Models;

namespace Viewer.Services
{
    /// <summary>
    /// Returns style to display in a view
    /// </summary>
    public static class StyleGetter
    {
        private static readonly List<NodeStyle> NodeStyles = new List<NodeStyle>
        {
            new NodeStyle{FontWeight = FontWeights.SemiBold, FontStyle = FontStyles.Normal},
            new NodeStyle{FontWeight = FontWeights.SemiBold, FontStyle = FontStyles.Italic},
            new NodeStyle{FontWeight = FontWeights.Normal, FontStyle = FontStyles.Italic},
            new NodeStyle{FontWeight = FontWeights.Normal, FontStyle = FontStyles.Normal}
        };

        /// <summary>
        /// Get default style to a node
        /// </summary>
        /// <returns> style to dislay in a view </returns>
        public static NodeStyle Get()
        {
            return NodeStyles[NodeStyles.Count - 1];
        }

        /// <summary>
        /// Get style according to a level (depth of a tree)
        /// </summary>
        /// <param name="level"> desirable level style (count starts with 0) according to the depth of the tree </param>
        /// <returns> style to dislay in a view </returns>
        public static NodeStyle Get(int level)
        {
            return level >= NodeStyles.Count ? Get() : NodeStyles[level];
        }

    }
}
