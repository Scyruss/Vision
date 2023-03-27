using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WPF.ImageEffects
{
    /// <summary>
    /// BrightContrastEffect
    /// </summary>
    public class ColorTintShader : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorTintShader), 0);

        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register(nameof(Red), typeof(double), typeof(ColorTintShader), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register(nameof(Green), typeof(double), typeof(ColorTintShader), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty BlueProperty =
    DependencyProperty.Register(nameof(Blue), typeof(double), typeof(ColorTintShader), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty ColorProperty =
DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorTintShader), new PropertyMetadata(OnCustomerChangedCallBack));

        private static void OnCustomerChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorTintShader brightness = d as ColorTintShader;
            if (brightness != null)
            {
                Color color = (Color)e.NewValue;
                brightness.Red = color.R;
                brightness.Blue= color.B;
                brightness.Green = color.G;
            }
        }

        public ColorTintShader()
        {
            //PixelShader = new PixelShader() { UriSource = new Uri("pack://application:,,,/Scripts/ImageEffects/Shaders/ImageShader.fxc") };
            PixelShader = new PixelShader() { UriSource = new Uri("pack://application:,,,/Scripts/ImageEffects/Shaders/ColorTintShader.fxc") };
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RedProperty);
            UpdateShaderValue(GreenProperty);
            UpdateShaderValue(BlueProperty);
        }

        public ColorTintShader(Color c)
        {
            SetValue(RedProperty, (double)c.R);
            SetValue(GreenProperty, (double)c.G);
            SetValue(BlueProperty, (double)c.B);
            //PixelShader = new PixelShader() { UriSource = new Uri("pack://application:,,,/Scripts/ImageEffects/Shaders/ImageShader.fxc") };
            PixelShader = new PixelShader() { UriSource = new Uri("pack://application:,,,/Scripts/ImageEffects/Shaders/ColorTintShader.fxc") };
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RedProperty);
            UpdateShaderValue(GreenProperty);
            UpdateShaderValue(BlueProperty);
        }

        /// <summary>
        /// Input
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }       

        /// <summary>
        /// Brightness
        /// </summary>
        public double Red
        {
            get { return (double)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        /// <summary>
        /// Contrast
        /// </summary>
        public double Green
        {
            get { return (double)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        public double Blue
        {
            get { return (double)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        public Color Color
        {
            get { return _Color; }
            set { Red = value.R;Green = value.G; Blue = value.B; _Color = value;}
        }
        private Color _Color;
    }
}
