namespace MikiEditorUI.ViewModel
{
    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

    using Microsoft.Win32;

    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            OnActive();
        }

        private WindowManager windowManager;

        private Chapter currentChapter;

        public Chapter CurrentChapter
        {
            get
            {
                return currentChapter;
            }

            set
            {
                currentChapter = value;
                this.NotifyOfPropertyChange(() => this.CurrentChapter);
            }
        }

        private Page currentPage;

        public Page CurrentPage
        {
            get
            {
                return currentPage;
            }

            set
            {
                currentPage = value;
                this.NotifyOfPropertyChange(() => this.CurrentPage);
            }
        }

        private Comic comic;

        public Comic Comic
        {
            get
            {
                return comic;
            }

            set
            {
                comic = value;
                NotifyOfPropertyChange(() => Comic);
            }
        }

        private BindableCollection<Chapter> chapters;

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

        private void OnActive()
        {
            this.windowManager = new WindowManager();
            comic = new Comic();
            comic.Chapters = new BindableCollection<Chapter>();
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>()};
            comic.Chapters.Add(chapter1);
        }

        public void AddNewPage()
        {
            if (!hasCurrentChapter())
            {
                return;
            }

            var Page = new Page() { ImgPath = "Image/blankimage.jpeg"};
            CurrentChapter.Pages.Add(Page);
        }

        public void AddNewChapter()
        {
            var chapter = new Chapter() { Title = "Chapter", Pages = new BindableCollection<Page>() };
            comic.Chapters.Add(chapter);
        }

        public void NewWorkSpace()
        {
            var newComicModel = new NewComicModel(this.Comic);

            windowManager.ShowDialog(newComicModel);

        }

        public void LoadImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                CurrentPage.ImgPath = op.FileName;
            }
        }

        private bool hasCurrentChapter()
        {
            return currentChapter != null;
        }
    }
}
