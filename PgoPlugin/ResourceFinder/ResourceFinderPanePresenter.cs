using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PgoPlugin.Debugger;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using ICSharpCode.ILSpy;

namespace PgoPlugin.ResourceFinder
{
    public class ResourceFinderPanePresenter : BasePresenter
    {
        ListCollectionView _resourcesView;
        ObservableCollection<object> _resources;

        public ResourceFinderPanePresenter ()
        {
            _resources = new ObservableCollection<object>();
            _resourcesView = new ListCollectionView(_resources);
            this._resourcesView.Filter = OnFiltered;
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

            this._resources.Clear();
            foreach (var item in resourceModules)
            {
                foreach (var resource in item.Resources)
                {
                    var resourceItem = new ResourceItem { AssemblyName=item.Assembly.Name.Name, ResourceType = resource.ResourceType.ToString(), ResourceName = resource.Name, Resource = resource };
                    this._resources.Add(resourceItem);                    
                }
            }
        }
        
        public ObservableCollection<object> Resources { get { return _resources; } }

        public ListCollectionView ResourcesView { get { return _resourcesView; } }

        string _searchTerm;
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    UpdateFilteredItems();
                }
            }
        }
    
        private void UpdateFilteredItems()
        {
            this._resourcesView.Refresh();
        }

        private bool OnFiltered(object value)
        {
            if (string.IsNullOrWhiteSpace(this._searchTerm))
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
