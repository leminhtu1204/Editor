namespace MikiEditorUI.ViewModel
{
    using Caliburn.Micro;
    using MikiEditor.BusinessObject;

    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            OnActive();
        }

        private BindableCollection<Chapter> chapters;

        private BindableCollection<Page> pages;

        public BindableCollection<Chapter> Chapters
        {
            get
            {
                return chapters;
            }

            set
            {
                chapters = value;
                this.NotifyOfPropertyChange(() => this.Chapters);
            }
        }

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

        private void OnActive()
        {
            if (Chapters == null)
            {
                Chapters = new BindableCollection<Chapter>();
            }

            if (Pages == null)
            {
                Pages = new BindableCollection<Page>();
            }
        }
    }
}
