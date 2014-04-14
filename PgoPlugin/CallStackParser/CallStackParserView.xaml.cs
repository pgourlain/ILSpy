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

namespace PgoPlugin.CallStackParser
{
    public class CallStackParserViewBase : BaseUserControl<CallStackParserPresenter>
    {
        public CallStackParserViewBase()
        {

        }
    }

    /// <summary>
    /// Interaction logic for CallStackParserView.xaml
    /// </summary>
    public partial class CallStackParserView : CallStackParserViewBase
    {
        public CallStackParserView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Presenter.
            this.Presenter.Parse();
;       }

        protected override void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            base.UserControl_Loaded(sender, e);
            this.Presenter.ExceptionStackTrace = @"[RuntimeBinderException: 'WebApplication2.Controllers.HomeController' does not contain a definition for 'DoSomething']
   CallSite.Target(Closure , CallSite , Object ) +135
   System.Dynamic.UpdateDelegates.UpdateAndExecuteVoid1(CallSite site, T0 arg0) +432
   WebApplication2.Controllers.HomeController.About() in C:\Users\genius\Documents\Visual Studio 2013\Projects\WebApplication2\WebApplication2\Controllers\HomeController.cs:21
   lambda_method(Closure , ControllerBase , Object[] ) +62
   System.Web.Mvc.ActionMethodDispatcher.Execute(ControllerBase controller, Object[] parameters) +14
   System.Web.Mvc.ReflectedActionDescriptor.Execute(ControllerContext controllerContext, IDictionary`2 parameters) +156
   System.Web.Mvc.ControllerActionInvoker.InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary`2 parameters) +27
   System.Web.Mvc.Async.AsyncControllerActionInvoker.<BeginInvokeSynchronousActionMethod>b__36(IAsyncResult asyncResult, ActionInvocation innerInvokeState) +22
   System.Web.Mvc.Async.WrappedAsyncResult`2.CallEndDelegate(IAsyncResult asyncResult) +29
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +49
   System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeActionMethod(IAsyncResult asyncResult) +32
   System.Web.Mvc.Async.AsyncInvocationWithFilters.<InvokeActionMethodFilterAsynchronouslyRecursive>b__3c() +50
   System.Web.Mvc.Async.<>c__DisplayClass45.<InvokeActionMethodFilterAsynchronouslyRecursive>b__3e() +225
   System.Web.Mvc.Async.<>c__DisplayClass30.<BeginInvokeActionMethodWithFilters>b__2f(IAsyncResult asyncResult) +10
   System.Web.Mvc.Async.WrappedAsyncResult`1.CallEndDelegate(IAsyncResult asyncResult) +10
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +49
   System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeActionMethodWithFilters(IAsyncResult asyncResult) +34
   System.Web.Mvc.Async.<>c__DisplayClass28.<BeginInvokeAction>b__19() +26
   System.Web.Mvc.Async.<>c__DisplayClass1e.<BeginInvokeAction>b__1b(IAsyncResult asyncResult) +100
   System.Web.Mvc.Async.WrappedAsyncResult`1.CallEndDelegate(IAsyncResult asyncResult) +10
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +49
   System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeAction(IAsyncResult asyncResult) +27
   System.Web.Mvc.Controller.<BeginExecuteCore>b__1d(IAsyncResult asyncResult, ExecuteCoreState innerState) +13
   System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +36
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +54
   System.Web.Mvc.Controller.EndExecuteCore(IAsyncResult asyncResult) +39
   System.Web.Mvc.Controller.<BeginExecute>b__15(IAsyncResult asyncResult, Controller controller) +12
   System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +28
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +54
   System.Web.Mvc.Controller.EndExecute(IAsyncResult asyncResult) +29
   System.Web.Mvc.Controller.System.Web.Mvc.Async.IAsyncController.EndExecute(IAsyncResult asyncResult) +10
   System.Web.Mvc.MvcHandler.<BeginProcessRequest>b__4(IAsyncResult asyncResult, ProcessRequestState innerState) +21
   System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +36
   System.Web.Mvc.Async.WrappedAsyncResultBase`1.End() +54
   System.Web.Mvc.MvcHandler.EndProcessRequest(IAsyncResult asyncResult) +31
   System.Web.Mvc.MvcHandler.System.Web.IHttpAsyncHandler.EndProcessRequest(IAsyncResult result) +9
   System.Web.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute() +9690164
   System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously) +155
";
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lvCallStack.SelectedItem as CallStackParserModel;
            if (selectedItem != null)
            {
                if (selectedItem is CallStackParserModelHeader)
                {
                    MessageBox.Show("this line cannot be found, through assemblies");
                    return;
                }
                if (!this.Presenter.TryJumpTo(selectedItem))
                {
                    MessageBox.Show("this item is not found, try to load assembly where it defined");
                }
            }
            
        }
    }
}
