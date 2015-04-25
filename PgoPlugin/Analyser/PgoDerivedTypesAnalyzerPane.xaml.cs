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
using PgoPlugin.UIHelper;

namespace PgoPlugin.Analyser
{

    public class PgoDerivedTypesAnalyzerPaneBase : BaseUserControl<DerivedTypesPresenter>
    {
        public PgoDerivedTypesAnalyzerPaneBase()
        {

        }
    }
    /// <summary>
    /// Interaction logic for PgoDerivedTypesAnalyzerPane.xaml
    /// </summary>
    public partial class PgoDerivedTypesAnalyzerPane : PgoDerivedTypesAnalyzerPaneBase
    {
        public PgoDerivedTypesAnalyzerPane()
        {
            InitializeComponent();
        }
    }
}
