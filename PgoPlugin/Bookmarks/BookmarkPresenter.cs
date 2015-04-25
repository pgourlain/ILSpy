using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using PgoPlugin.UIHelper;

namespace PgoPlugin.Bookmarks
{
    public class BookmarkPresenter : FilteredPresenter<BookmarkModel>
    {

        public BookmarkPresenter()
        {
            this.BookmarkFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ILSpy", "pgoplugin.bookmarks.txt");
        }

        protected internal override void ViewReady()
        {
        }

        protected internal override void ViewClose()
        {
        }

        internal void LoadBookmarks()
        {
            if (File.Exists(BookmarkFileName))
            {
                foreach (var line in File.ReadLines(BookmarkFileName))
                {
                    var m = BookmarkModel.FromPath(line);
                    if (m != null)
                        this.Models.Add(m);
                }
            }            
        }

        internal void SaveBookmarks()
        {
            try
            {
                if (!File.Exists(BookmarkFileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(BookmarkFileName));
                }

                File.WriteAllLines(BookmarkFileName, this.Models.Select(x => x.FullDefinition));
            }
            catch (IOException)
            {
                //if IOException -> bookmarks will be lost
            }
        }

        string BookmarkFileName { get; set; }

        ICommand _command;
        public ICommand RemoveCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new RelayCommand(Execute, CanExecute);
                }
                return _command;
            }
        }

        private void Execute(object o)
        {
            this.Models.Remove(o as BookmarkModel);
        }

        private bool CanExecute(object o)
        {
            return true;
        }

        protected override bool OnFiltered(object value)
        {
            if (!base.OnFiltered(value))
            {
                var model = value as BookmarkModel;
                if (model != null && !string.IsNullOrWhiteSpace(model.FullDefinition))
                {
                    return model.FullDefinition.IndexOf(this.SearchTerm) >= 0;
                }
                return false;
            }
            else return true;
        }

    }
}
