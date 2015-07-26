using System.Linq;
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

        private BindableCollection<Chapter> pages;

        public BindableCollection<Chapter> Pages
        {
            get
            {
                return pages;
            }

            set
            {
                chapters = value;
                this.NotifyOfPropertyChange(() => this.Pages);
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
            this.windowManager = new WindowManager();
            Chapters = new BindableCollection<Chapter>();
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            Chapters.Add(chapter1);
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
            comic = new Comic { Chapters = Chapters };

            var newComicModel = new NewComicModel(this.Comic);

            windowManager.ShowDialog(newComicModel);

        }

        public void SaveWorkSpace()
        {

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
            if (!HasCurrentChapter())
            {
                return;
            }

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
            if (!HasCurrentPage())
            {
                return;
            }

            int i;

            var currentIndex = currentPage.Index;

            currentChapter.Pages.Remove(CurrentPage);

            for (i = currentIndex - 1; i < currentChapter.Pages.Count; i++)
            {
                currentChapter.Pages[i].Index -= 1;
            }

            TotalPage = "Total " + currentChapter.Pages.Count;
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

            Chapters.Insert(chapter.Index - 1, chapter);

            for (i = currentIndex; i < Chapters.Count; i++)
            {
                Chapters[i].Index = i + 1;
            }
        }

        public void InsertPage()
        {
            if (currentPage == null)
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

            currentChapter.Pages.Insert(page.Index - 1, page);

            for (i = currentIndex; i < currentChapter.Pages.Count; i++)
            {
                currentChapter.Pages[i].Index = i + 1;
            }

            TotalPage = "Total " + currentChapter.Pages.Count;
        }
    }
}
