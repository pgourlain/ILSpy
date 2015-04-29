using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PgoPlugin.DecompilerViewExtensions
{
    /// <summary>
    /// Interaction logic for SmartTagUI.xaml
    /// </summary>
    public partial class SmartTagUI : UserControl
    {
        public SmartTagUI()
        {
            InitializeComponent();
            this.lightImage.Source = Images.Light;
            this.comboImage.Source = Images.ComboButton;
            this.MouseEnter += UIElement_MouseEnter;
            this.MouseLeave += UIElement_MouseLeave;
            this.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
        }

        private void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("comming soon");
        }

        private void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.comboImage.Visibility = Visibility.Collapsed;
        }

        private void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.comboImage.Visibility = Visibility.Visible;
        }
    }
}
