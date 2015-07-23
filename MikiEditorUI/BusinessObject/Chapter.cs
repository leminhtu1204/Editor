using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    public class Chapter : PropertyChangedBase
    {
        public int Index { get; set; }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                this.NotifyOfPropertyChange(() => this.Title);
            }
        }

        private BindableCollection<Page> pages;
        public BindableCollection<Page> Pages
        {
            get
            {
                return pages;
            }

            set
            {
                pages = value;
                this.NotifyOfPropertyChange(() => this.Pages);
            }
        }
    }
}
