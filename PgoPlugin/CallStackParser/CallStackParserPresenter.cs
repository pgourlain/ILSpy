using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.ILSpy;
using PgoPlugin.UIHelper;

namespace PgoPlugin.CallStackParser
{
    public class CallStackParserPresenter : FilteredPresenter<CallStackParserModel>
    {
        public CallStackParserPresenter()
        {
            this.Parsing = Visibility.Collapsed;
        }

        protected internal override void ViewClose()
        {
        }

        protected internal override void ViewReady()
        {
        }

        string _ExceptionStackTrace;
        public string ExceptionStackTrace
        {
            get { return _ExceptionStackTrace; } set
            {
                _ExceptionStackTrace = value;
                this.DoNotifyPropertyChanged("ExceptionStackTrace");
                this.DoNotifyPropertyChanged("CanParse");
            }
        }

        public bool CanParse
        {
            get
            {
                return !string.IsNullOrEmpty(this._ExceptionStackTrace);
            }
        }

        private Visibility parsing;

        public Visibility Parsing
        {
            get { return parsing; }
            set { parsing = value; this.DoNotifyPropertyChanged("Parsing"); }
        }

        public void Parse()
        {
            //string stack = "System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments) +192";
            //var estp = new ExceptionStackTraceParser(stack);
            //estp.Resolve(MainWindow.Instance.CurrentAssemblyList.GetAssemblies());
            this.Models.Clear();
            Parsing = Visibility.Visible;
            Task.Factory.StartNew(() =>
            {
                var estp = new ExceptionStackTraceParser(this.ExceptionStackTrace);
                var result = estp.Resolve(MainWindow.Instance.CurrentAssemblyList.GetAssemblies());
                UiInvoke(() => { Models.AddRange(result); });
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    UiInvoke(() => { Parsing = Visibility.Collapsed; MessageBox.Show("send your stacktrace to Pierrick", "Parsing exception"); });
                }
                else
                {
                    UiInvoke(() => { Parsing = Visibility.Collapsed; this.UpdateFilteredItems(); });
                }
            });
        }

        public bool TryJumpTo(CallStackParserModel selectedItem)
        {
            var x = selectedItem as CallStackParserModelMethod;
            if (x != null)
            {
                if (x.Definition != null)
                {
                    MainWindow.Instance.JumpToReference(x.Definition);
                    return true;
                }
            }
            return false;
        }
    }
}
