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
using ICSharpCode.ILSpy.TextView;
using Mono.Cecil;

namespace PgoPlugin.DecompilerViewExtensions
{
    /// <summary>
    /// Interaction logic for SmartTagUI.xaml
    /// </summary>
    public partial class SmartTagUI : UserControl
    {
        public bool ShouldBeStayOpen
        {
            get
            {
                if (menuPopup.IsOpen)
                    return true;
                return false;
            }
        }

        public ReferenceSegment Segment { get; internal set; }

        public SmartTagUI()
        {
            InitializeComponent();
            this.lightImage.Source = Images.Light;
            this.togglePopup.Tag = Images.ComboButton;
            this.MouseEnter += UIElement_MouseEnter;
            this.MouseLeave += UIElement_MouseLeave;
            this.menuPopup.Closed += MenuPopup_Closed;
        }

        private void MenuPopup_Closed(object sender, EventArgs e)
        {
            //hide tooglePopup ?
        }

        private void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.menuPopup.IsOpen)
            {
                this.togglePopup.Visibility = Visibility.Collapsed;
            }
        }

        private void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.togglePopup.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.Segment == null || (this.Segment!= null && this.Segment.Reference == null))
            {
                MessageBox.Show("This text segment doesn't contains any references");
                this.menuPopup.IsOpen = false;
                return;
            }
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                SingletonPane<ReferencesView.ReferencesView>.Instance.Show(new KeyValuePair<string, object>(menuItem.Tag.ToString(), this.Segment.Reference));
            }
            this.menuPopup.IsOpen = false;
        }
    }
}
