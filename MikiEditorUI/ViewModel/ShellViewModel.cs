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

        private Thread thread;

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

        public string TotalPage
        {
            get
            {
                if (this.HasCurrentChapter())
                {
                    return "Total " + currentChapter.Pages.Count;
                }

                return "Total 0";
            }
        }

        private void OnActive()
        {
            comic = new Comic { Chapters = new BindableCollection<Chapter>() };
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            this.comic.Chapters.Add(chapter1);
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
            NotifyOfPropertyChange(() => TotalPage);
        }

        public void AddNewChapter()
        {
            var lastIndex = this.comic.Chapters.Count;

            var chapter = new Chapter
            {
                Title = "Chapter",
                Pages = new BindableCollection<Page>(),
                Index = ++lastIndex
            };

            this.comic.Chapters.Add(chapter);
        }

        public void NewWorkSpace()
        {
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

            this.comic.Chapters.Remove(currentChapter);

            for (i = currentIndex - 1; i < this.comic.Chapters.Count; i++)
            {
                this.comic.Chapters[i].Index -= 1;
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
            NotifyOfPropertyChange(() => TotalPage);
        }

        public void InsertChapter()
        {
            if (!HasCurrentChapter())
            {
                return;
            }

            int i;

            var currentIndex = currentChapter.Index;
            if (currentIndex == this.comic.Chapters.Count)
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
            this.comic.Chapters.Insert(currentIndex, chapter);
            for (i = currentIndex + 1; i < this.comic.Chapters.Count; i++)
            {
                this.comic.Chapters[i].Index = i + 1;
            }
            NotifyOfPropertyChange(() => TotalPage);
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
                if (comic == null)
                {
                    thread.Abort();
                    break;
                }
                Thread.Sleep(10000);
                helper.ConvertJson(comic, AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("dMMyyyy"), "manga");
            }
        }

        private void AutoSave()
        {
            thread = new Thread(WriteTempFile) { IsBackground = true, Priority = ThreadPriority.Lowest};

            thread.Start();
        }

        public void SaveComic()
        {
            var dlg = new SaveFileDialog
                          {
                              FileName = "Comic",
                              DefaultExt = ".manga",
                              Filter = "Manga save file (.manga)|*.manga"
                          };

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filePath = dlg.FileName;

                helper.ConvertJson(comic, filePath);
            }
        }

        public void OpenComic()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a comic template";
            op.Filter = "All templte comic (.manga) |*.manga";
            op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (op.ShowDialog() == true)
            {
                var comicConvert = helper.ConvertToObjectFromJson(op.FileName);
                this.Comic = comicConvert;
            }
        }

        public void SelectedChapterChanged()
        {
            this.NotifyOfPropertyChange(() => TotalPage);
        }

        public void NewComic()
        {
            Comic = new Comic { Chapters = new BindableCollection<Chapter>() };
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            this.comic.Chapters.Add(chapter1);
        }

        public void ChangePageImage()
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
    }
}
