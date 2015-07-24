using System.Linq;

using Microsoft.Win32;

namespace MikiEditorUI.ViewModel
{
    using System;
    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
            var chapter1 = new Chapter { Title = "Chapter", Comic = comic };
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

            comic.Chapters.Add(chapter1);

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
            comic.Chapters.Add(new Chapter() { Title = "Chapter ", Comic = comic, Pages = new BindableCollection<Page>() });
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

        private int getNewPageIndex()
        {
            if (currentPage == null)
            {
                return 1;
            }

            return this.currentPage.PageIndex + 1;
        }
    }
}
