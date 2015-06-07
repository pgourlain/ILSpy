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
using Mono.Cecil;
using PgoPlugin.UIHelper;

namespace PgoPlugin.ReferencesView
{
    /// <summary>
    /// Interaction logic for ReferencesView.xaml
    /// </summary>
    public partial class ReferencesViewBase : BaseUserControl<ReferencesViewPresenter>
    {
        public ReferencesViewBase()
        {
        }
    }

    public partial class ReferencesView : ReferencesViewBase
    {
        public ReferencesView()
        {
            InitializeComponent();
        }

        protected override string WindowTile
        {
            get
            {
                return "References";
            }
        }

        protected override void SetParameter(object parameter)
        {
            this.Presenter.UpdateReferences(parameter as MemberReference);
        }

        void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && lvExtentions.HasItems)
            {
                e.Handled = true;
                lvExtentions.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                lvExtentions.SelectedIndex = 0;
            }
        }
    }
}
