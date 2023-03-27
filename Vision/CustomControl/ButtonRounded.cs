using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WpfApp1.CustomControl
{
    public class ButtonRounded : Button
    {
        public static readonly DependencyProperty RoundedValProperty =
        DependencyProperty.Register("RoundedVal", typeof(double), typeof(ButtonRounded), new PropertyMetadata(3.0));

        public double RoundedVal
        {
            get { return (double)GetValue(RoundedValProperty); }
            set { SetValue(RoundedValProperty, value); }
        }

    }
}
