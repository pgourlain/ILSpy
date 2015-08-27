using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
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
                return "References (beta)";
            }
        }

        protected override string GetWindowTitle(object[] parameters)
        {
            if (parameters != null)
            {
                var mr = parameters[1] as MemberReference;
                if (mr != null)
                {
                    return WindowTile + ": " + mr.FullName;
                }
            }
            return base.GetWindowTitle(parameters);
        }

        protected override void SetParameters(object[] args)
        {
            this.Presenter.UpdateReferences(args[0].ToString(), args[1] as MemberReference);
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

        private void graph_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var document = this.Presenter.CreateDgml();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".dgml";
            sfd.Filter = "Dgml Documents (*.dgml)|*.dgml";
            if (sfd.ShowDialog() == true)
            {
                document.Save(sfd.FileName);
            }
        }
    }
}
