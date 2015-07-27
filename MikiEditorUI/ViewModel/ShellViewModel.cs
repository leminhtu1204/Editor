using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace MikiEditorUI.ViewModel
{
    using Caliburn.Micro;

    using BusinessObject;

    using Microsoft.Win32;

    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            OnActive();
        }

        private Helper helper = new Helper();

        Page page = new Page();

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

        private string totalPage;

        public string TotalPage
        {
            get
            {
                return totalPage;
            }

            set
            {
                totalPage = value;
                this.NotifyOfPropertyChange(() => this.TotalPage);
            }
        }

        private void OnActive()
        {
            comic = new Comic { Chapters = Chapters };
            Chapters = new BindableCollection<Chapter>();
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            Chapters.Add(chapter1);
            AutoSave();
        }

        public void AddNewPage()
        {
            if (!HasCurrentChapter())
            {
                return;
            }

            var lastIndex = currentChapter.Pages.Count;

            page = new Page() { ImgPath = string.Empty, Index = ++lastIndex };

            CurrentChapter.Pages.Add(page);

            TotalPage = "Total " + lastIndex;
        }

        public void AddNewChapter()
        {
            var lastIndex = Chapters.Count;

            var chapter = new Chapter
            {
                Title = "Chapter",
                Pages = new BindableCollection<Page>(),
                Index = ++lastIndex
            };

            Chapters.Add(chapter);
        }

        public void NewWorkSpace()
        {
            this.comic.Chapters = Chapters;

            this.windowManager = new WindowManager();

            var newComicModel = new NewComicModel(this.Comic);

            windowManager.ShowDialog(newComicModel);

        }

        public void LoadImage()
        {
            if (!this.HasCurrentPage())
            {
                return;
            }

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

        private bool HasCurrentPage()
        {
            return currentPage != null;
        }

        private bool HasCurrentChapter()
        {
            return currentChapter != null;
        }

        public void RemoveChapter()
        {
            int i;

            var currentIndex = currentChapter.Index;

            Chapters.Remove(currentChapter);

            for (i = currentIndex - 1; i < Chapters.Count; i++)
            {
                Chapters[i].Index -= 1;
            }
        }

        public void RemovePage()
        {
            int i;

            var currentIndex = currentPage.Index;

            currentChapter.Pages.Remove(CurrentPage);

            for (i = currentIndex - 1; i < currentChapter.Pages.Count; i++)
            {
                currentChapter.Pages[i].Index -= 1;
            }
        }

        public void InsertChapter()
        {
            if (!HasCurrentChapter())
            {
                return;
            }

            int i;

            var currentIndex = currentChapter.Index;

            if (currentIndex == Chapters.Count)
            {
                AddNewChapter();
                return;
            }

            var chapter = new Chapter
            {
                Title = "Chapter",
                Pages = new BindableCollection<Page>(),
                Index = currentIndex + 1
            };

            Chapters.Insert(currentIndex, chapter);

            for (i = currentIndex + 1; i < Chapters.Count; i++)
            {
                Chapters[i].Index = i + 1;
            }
        }

        public void InsertPage()
        {
            if (!this.HasCurrentPage())
            {
                return;
            }

            int i;

            var currentIndex = currentPage.Index;

            if (currentIndex == currentChapter.Pages.Count)
            {
                AddNewPage();
                return;
            }

            page = new Page { ImgPath = string.Empty, Index = currentIndex + 1 };

            currentChapter.Pages.Insert(currentIndex, page);

            for (i = currentIndex + 1; i < currentChapter.Pages.Count; i++)
            {
                currentChapter.Pages[i].Index = i + 1;
            }
        }

        private void WriteTempFile()
        {
            while (true)
            {
                Thread.Sleep(10000);
                helper.ConvertJson(comic, AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("dMMyyyy"), "tmp");
            }
        }

        private void AutoSave()
        {
            var thread = new Thread(WriteTempFile);

            thread.Start();
        }

        public void SaveComic()
        {
            helper.ConvertJson(comic, AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("dMMyyyy"), "tmp");
        }

        public void OpenComic()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a comic template";
            op.Filter = "All templte comic |*.tmp";
            op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (op.ShowDialog() == true)
            {

            }
        }

        public void NewComic()
        {
            comic = null;
            comic = new Comic { Chapters = Chapters };
            Chapters = new BindableCollection<Chapter>();
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            Chapters.Add(chapter1);
            AutoSave();
        }
    }
}
