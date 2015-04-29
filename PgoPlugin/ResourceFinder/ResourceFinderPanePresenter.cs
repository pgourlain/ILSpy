using System;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using ICSharpCode.ILSpy;
using PgoPlugin.UIHelper;

namespace PgoPlugin.ResourceFinder
{
    public class ResourceFinderPanePresenter : FilteredPresenter<ResourceItem>
    {
        public ResourceFinderPanePresenter ()
        {
        }

        protected internal override void ViewReady()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.CurrentAssemblyListChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Instance_CurrentAssemblyListChanged);
                UpdateList();
            } 
        }

        void Instance_CurrentAssemblyListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateList();
        }

        protected internal override void ViewClose()
        {
        }

        private void UpdateList()
        {
            var assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
            var definitions = assemblies.Where(x => x.IsLoaded).Where(x => x.AssemblyDefinition != null).Select(x => x.AssemblyDefinition);
            var resourceModules = definitions.SelectMany(x => x.Modules);

            this.Models.Clear();
            foreach (var item in resourceModules)
            {
                foreach (var resource in item.Resources)
                {
                    var resourceItem = new ResourceItem { AssemblyName=item.Assembly.Name.Name, ResourceType = resource.ResourceType.ToString(), ResourceName = resource.Name, Resource = resource };
                    this.Models.Add(resourceItem);                    
                }
            }
        }

        protected override bool OnFiltered(object value)
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
                return true;
            var model = value as ResourceItem;
            if (model != null)
            {
                return model.ResourceName.IndexOf(this.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }
    }
}
