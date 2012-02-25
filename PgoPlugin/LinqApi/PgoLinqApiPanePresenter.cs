using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PgoPlugin.Debugger;
using System.Reflection;
using System.Collections.ObjectModel;
using ICSharpCode.ILSpy;
using Mono.Cecil;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Diagnostics;

namespace PgoPlugin.LinqApi
{
    public class PgoLinqApiPanePresenter : BasePresenter
    {
        ListCollectionView _linqApisView;
        ObservableCollection<object> _linqApis;
        public PgoLinqApiPanePresenter()
        {            
            _linqApis = new ObservableCollection<object>();
            _linqApisView = new ListCollectionView(_linqApis);
            //_linqApisView.GroupDescriptions.Add(new PropertyGroupDescription("ExtendedType"));
            _linqApisView.Filter = OnFiltered;
            this._selectedSearchKind = "ExtendedType";
        }

        protected internal override void ViewReady()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.CurrentAssemblyListChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Instance_CurrentAssemblyListChanged);
                UpdateList();
            } 
        }

        private void UpdateList()
        {
            var assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
            var definitions = assemblies.Where(x => x.IsLoaded).Where(x => x.AssemblyDefinition != null).Select(x => x.AssemblyDefinition);
            var enumerableType = definitions.SelectMany(x => x.Modules).SelectMany(x => x.GetTypes()).Where(x => x.IsPublic && x.IsSealed);

            var extensionMethods = enumerableType.SelectMany(x => x.Methods).Where(x => x.IsPublic && x.IsStatic && x.HasCustomAttributes && x.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute"));
            var methods = from m in extensionMethods
                          let isExtension = m.HasCustomAttributes && m.CustomAttributes.Any(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute")
                          select new LinqApiModel
                          {
                              Method = m,
                              ReturnType = m.ReturnType.FormatTypeName(),
                              MethodName = m.Name + 
                                (m.HasGenericParameters ? "<" + m.GenericParameters.Select(t => t.Name).CommaSeparated() + ">" : ""),
                              Parameters = "(" + (isExtension ? "this " : "") +
                                    m.Parameters.Select(p => p.ParameterType.FormatTypeName() + " " + p.Name) .CommaSeparated() 
                                    + ")",
                              ExtendedType = isExtension ? m.Parameters.Select(p => p.ParameterType.FormatTypeName()).First() : ""
                          };

            this.LinqApis.Clear();
            foreach (var item in methods)
            {
                this.LinqApis.Add(item);
            }
        }

        void Instance_CurrentAssemblyListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateList();
        }

        protected internal override void ViewClose()
        {
        }

        public ObservableCollection<object> LinqApis { get { return _linqApis; } }

        public ListCollectionView LinqApisView { get { return _linqApisView; } }

        public IEnumerable<string> SearchKinds
        {
            get
            {
                yield return "ExtendedType";
                yield return "FullDefinition";
                yield return "AssemblyName";
            }
        }
        
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

        string _selectedSearchKind;
        public string SelectedSearchKind
        {
            get
            {
                return _selectedSearchKind;
            }
            set
            {
                if (value != _selectedSearchKind)
                {
                    _selectedSearchKind = value;
                    UpdateFilteredItems();
                }
            }
        }

        private void UpdateFilteredItems()
        {
            this._linqApisView.Refresh();
        }

        private bool OnFiltered(object value)
        {
            if (string.IsNullOrWhiteSpace(this._searchTerm))
                return true;
            var model = value as LinqApiModel;
            if (model != null)
            {
                switch(this.SelectedSearchKind)
                {
                    case "ExtendedType":
                        return model.ExtendedType.IndexOf(this.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                    case "Name":
                        return model.MethodName.IndexOf(this.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                    case "AssemblyName":
                        return model.AssemblyName.IndexOf(this.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                    default:
                        return model.FullDefinition.IndexOf(this.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            return false;
        }

        #region linq apis from msdn "http://msdn.microsoft.com/en-us/vstudio/hh749018"
        /*
        private static void LinqAnalysis(Assembly assembly)
        {
            var enumerableType = assembly.GetExportedTypes()
                .Single(t => t.Name == "Enumerable");
            var methods = from m in enumerableType.GetMethods(
                                BindingFlags.Public | BindingFlags.Static)
                          let isExtension = m.GetCustomAttributes(
                              typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                              .Any()
                          select new
                          {
                              returnParm = m.ReturnType.FormatTypeName(),
                              name = m.Name +
                              (m.GetGenericArguments().Any() ?
                                  "<" + m.GetGenericArguments()
                                      .Select(t => t.Name)
                                      .CommaSeparated() + ">"
                                      : ""),
                              parameters = "(" +
                              (isExtension ? "this " : "") +
                              m.GetParameters()
                                  .Select(p => p.ParameterType.FormatTypeName()
                                      + " " + p.Name)
                                  .CommaSeparated()
                                  + ")"
                          };
            foreach (var m in methods)
                Console.WriteLine("{0} {1}{2}", m.returnParm, m.name, m.parameters);
        }
        */
        /* optimized
var methods = from m in enumerableType.GetMethods( 
                        BindingFlags.Public | BindingFlags.Static) 
                    let isExtension = m.GetCustomAttributes( 
                        typeof(ExtensionAttribute), false) 
                        .Any() 
                    orderby m.Name, m.GetParameters().Count() 
                    select new 
                    { 
                        returnParm = m.ReturnType.FormatTypeName(), 
                        name = m.Name +  
                        (m.GetGenericArguments().Any() ? 
                            "<" + m.GetGenericArguments() 
                                .Select(t => t.Name) 
                                .CommaSeparated() + ">"  
                                : ""), 
                        parameters = "(" +  
                        (isExtension ? "this " : "") + 
                        m.GetParameters() 
                            .Select(p => p.ParameterType.FormatTypeName() 
                                + " " + p.Name) 
                            .CommaSeparated() 
                            + ")" 
                    };
         * */

        #endregion

    }

    public static class LINQHelpers
    {
        public static string FormatTypeName(this Type genericType)
        {
            if (!genericType.IsGenericType)
                return genericType.Name;
            var parmTypeName = genericType.Name.Remove(genericType.Name
                .IndexOf('`'));
            var genericTypes = (from g in genericType.GetGenericArguments()
                                select g.FormatTypeName())
                                .CommaSeparated();
            return string.Format("{0}<{1}>", parmTypeName, genericTypes);
        }

        public static string FormatTypeName(this TypeReference genericType)
        {
            if (!genericType.IsGenericInstance)
                return genericType.Name;
            var gt = genericType as GenericInstanceType;

            var parmTypeName = genericType.Name.Remove(genericType.Name
                .IndexOf('`'));
            var genericTypes = (from g in gt.GenericArguments
                                select g.FormatTypeName())
                                .CommaSeparated();
            return string.Format("{0}<{1}>", parmTypeName, genericTypes);
        }
        public static string CommaSeparated(this IEnumerable<string> items)
        {
            return items.Any()
                ? items.Aggregate((partialResult, item) =>
                    string.Format("{0}, {1}", partialResult, item))
                : "";
        }
    }
}
