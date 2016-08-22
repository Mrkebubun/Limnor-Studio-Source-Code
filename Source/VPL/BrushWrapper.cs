using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VPL
{
    /// <summary>
    /// for Brush to be used as a Property
    /// </summary>
    public class BrushWrapper
    {
        private Brush _brush;
        private bool _gradient;
        private Color _color;
        public BrushWrapper()
        {
            _brush = Brushes.Black;
        }
        public BrushWrapper(Brush brush)
        {
            _brush = brush;
        }
        public BrushWrapper(Color color, bool gradient)
        {
            _gradient = gradient;
            _color = color;
            _brush = Brushes.Black;
        }
    }
}
