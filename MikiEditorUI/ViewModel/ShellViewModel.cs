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

        //private BindableCollection<Page> pages;

        //public BindableCollection<Page> Pages
        //{
        //    get
        //    {
        //        return pages;
        //    }

        //    set
        //    {
        //        pages = value;
        //        this.NotifyOfPropertyChange(() => this.Pages);
        //    }
        //}

        private void OnActive()
        {
            if (Chapters == null)
            {
                Chapters = new BindableCollection<Chapter>();
                var chapter1 = new Chapter
                {
                    Title = "Chapter1",
                    Pages =
                        new BindableCollection<Page>
                        {
                            new Page() {ImgPath = "Image/blankimage.jpeg"},
                            new Page() {ImgPath = "Image/blankimage.jpeg"},
                            new Page() {ImgPath = "Image/blankimage.jpeg"}
                        }
                };

                var chapter2 = new Chapter
                {
                    Title = "Chapter2",
                    Pages =
                        new BindableCollection<Page>
                        {
                            new Page() {ImgPath = "Image/blankimage.jpeg"},
                            new Page() {ImgPath = "Image/blankimage.jpeg"}
                        }
                };

                Chapters.Add(chapter1);
                Chapters.Add(chapter2);
            }
        }

        public void AddNewPage()
        {
            CurrentChapter.Pages.Add(new Page() { ImgPath = "Image/blankimage.jpeg" });
        }

        public void ChapterChange()
        {
           
        }
    }
}
