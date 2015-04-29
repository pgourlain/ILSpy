using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PgoPlugin.UIHelper
{
    public abstract class FilteredPresenter<TModel> : BasePresenter
    {
        ListCollectionView _View;
        ObservableCollection<TModel> _model;

        public FilteredPresenter()
        {
            _model = new ObservableCollection<TModel>();
            _View = new ListCollectionView(_model);
            _View.Filter = OnFiltered;

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

        protected void UpdateFilteredItems()
        {
            this._View.Refresh();
        }

        protected virtual bool OnFiltered(object value)
        {
            if (string.IsNullOrWhiteSpace(this._searchTerm))
                return true;

            return false;
        }

        public ObservableCollection<TModel> Models { get { return this._model; } }

        public ListCollectionView ModelsView { get { return this._View; } }

        protected void RunAsync(Func<IEnumerable<TModel>> source)
        {
            RunAsync(source, results =>
            {
                this.Models.Clear();
                foreach (var item in results)
                {
                    this.Models.Add(item);
                }
            });
        }
    }
}
