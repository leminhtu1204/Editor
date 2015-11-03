using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    public class Chapter : PropertyChangedBase
    {
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

        private int index;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                this.NotifyOfPropertyChange(() => this.Index);
            }
        }
    }
}
