using Microsoft.Win32;

namespace MikiEditorUI.ViewModel
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

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

        public void AddNewChapter()
        {
            var title = chapters.Count + 1;
            Chapters.Add(new Chapter() { Title = "Chapter " + title, Pages = new BindableCollection<Page>() });
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
    }
}
