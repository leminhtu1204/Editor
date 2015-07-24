namespace MikiEditorUI.ViewModel
{
    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            OnActive();
        }
        public ShellViewModel(Comic comic)
        {
            this.currentComic = comic;
            this.currentComic.Chapters = chapters;
            OnActive();
        }

        private Comic currentComic;

        public Comic CurrentComic
        {
            get
            {
                return currentComic;
            }
            set
            {
                currentComic = value;
                this.NotifyOfPropertyChange(()=> this.CurrentComic);
            }
        }

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

            currentComic.Chapters = new BindableCollection<Chapter>();
            var chapter1 = new Chapter { Title = "Chapter", Comic = currentComic };
            chapter1.Pages = new BindableCollection<Page>
                                     {
                                         new Page
                                             {
                                                 ImgPath = "Image/blankimage.jpeg",
                                                 Chapter = chapter1
                                             },
                                              new Page
                                             {
                                                 ImgPath = "Image/blankimage.jpeg",
                                                 Chapter = chapter1
                                             }
                                     };

            var chapter2 = new Chapter { Title = "Chapter", Comic = currentComic };
            chapter2.Pages = new BindableCollection<Page>
                                     {
                                         new Page
                                             {
                                                 ImgPath = "Image/blankimage.jpeg",
                                                 Chapter = chapter2
                                             }
                                     };

            currentComic.Chapters.Add(chapter1);
            currentComic.Chapters.Add(chapter2);

        }

        public void AddNewPage()
        {
            if (!hasCurrentChapter())
            {
                return;
            }

            var currentPageIndex = this.getNewPageIndex();
            var Page = new Page() { ImgPath = "Image/blankimage.jpeg", Chapter = currentChapter };
            CurrentChapter.Pages.Insert(currentPageIndex-1, Page);
        }

        public void AddNewChapter()
        {
            currentComic.Chapters.Add(new Chapter() { Title = "Chapter ", Comic = comic, Pages = new BindableCollection<Page>() });
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

        private int getNewPageIndex()
        {
            if (currentPage == null)
            {
                return 1;
            }

            return this.currentPage.PageIndex + 1;
        }

        public void ExportComic()
        {
            var help = new Helper();

            help.ExportCompressFile(currentComic, currentComic.WorkSpace);
        }
    }
}
