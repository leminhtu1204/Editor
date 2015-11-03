using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Windows.Point;

namespace MikiEditorUI.ViewModel
{
    using Caliburn.Micro;

    using BusinessObject;

    using Microsoft.Win32;

    using Path = System.IO.Path;

    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            OnActive();
        }

        private int scale = 4;

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

        private void InitComic()
        {
            comic = new Comic { Chapters = new BindableCollection<Chapter>() };
            var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
            this.comic.Chapters.Add(chapter1);
        }

        private void OnActive()
        {
            InitComic();
            AutoSave();
        }

        public void AddNewPage()
        {
            if (!HasCurrentChapter())
            {
                return;
            }

            var lastIndex = currentChapter.Pages.Count;

            var op = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png"
            };

            if (op.ShowDialog() == true)
            {
                foreach (var file in op.FileNames)
                {
                    page = new Page() { ImgPath = string.Empty, Index = ++lastIndex };
                    page.ImgPath = file;
                    CurrentChapter.Pages.Add(page);
                    NotifyOfPropertyChange(() => TotalPage);
                }
            }
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
            var folderDialog = new FolderBrowserDialog { SelectedPath = "C:\\" };
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                comic.WorkSpace = folderDialog.SelectedPath;

                this.windowManager = new WindowManager();

                var newComicModel = new NewComicModel(this.Comic, true);

                windowManager.ShowDialog(newComicModel);
            }
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

            this.comic.Chapters.Remove(currentChapter);

            for (i = currentIndex - 1; i < this.comic.Chapters.Count; i++)
            {
                this.comic.Chapters[i].Index -= 1;
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

        public void InsertAbovePage()
        {
            if (!this.HasCurrentPage())
            {
                return;
            }

            int i;

            var currentIndex = currentPage.Index;

            page = new Page { ImgPath = string.Empty, Index = currentIndex };

            currentChapter.Pages.Insert(currentIndex - 1, page);

            for (i = currentIndex; i < currentChapter.Pages.Count; i++)
            {
                currentChapter.Pages[i].Index = i + 1;
            }
        }

        public void InsertAboveChapter()
        {
            if (!this.HasCurrentChapter())
            {
                return;
            }

            int i;

            var currentIndex = currentChapter.Index;

            var chapter = new Chapter
            {
                Title = "Chapter",
                Pages = new BindableCollection<Page>(),
                Index = currentIndex
            };

            comic.Chapters.Insert(currentIndex - 1, chapter);

            for (i = currentIndex; i < comic.Chapters.Count; i++)
            {
                comic.Chapters[i].Index = i + 1;
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
                helper.ConvertJson(comic, AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("dMMyyyy"), "tmp");
            }
        }

        private void AutoSave()
        {
            foreach (string sFile in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.tmp"))
            {
                if (Path.GetFileName(sFile) != DateTime.Now.ToString("dMMyyyy") + ".tmp")
                {
                    System.IO.File.Delete(sFile);
                }
            }

            thread = new Thread(WriteTempFile) { IsBackground = true, Priority = ThreadPriority.Lowest };

            thread.Start();
        }

        public void SaveComic()
        {
            var dlg = new SaveFileDialog
                          {
                              FileName = "Comic",
                              DefaultExt = ".tmp",
                              Filter = "Manga save file (.tmp)|*.tmp"
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
            op.Filter = "All templte comic (.tmp) |*.tmp";
            op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (op.ShowDialog() == true)
            {
                var comicConvert = helper.ConvertToObjectFromJson(op.FileName);
                this.Comic = comicConvert;
            }
        }

        public void OpenProject()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a comic template";
            op.Filter = "All templte comic (.manga) |*.manga";
            op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (op.ShowDialog() == true)
            {
                var comicConvert = helper.ConvertToObjectFromJson(op.FileName);

                var path = Directory.GetParent(op.FileName);

                this.Comic = comicConvert;

                foreach (var c in this.Comic.Chapters)
                {
                    foreach (var p in c.Pages)
                    {
                        p.ImgPath = path + @"\" + p.ImgPath.Replace(@"/", @"\");
                    }
                }
            }
        }

        public void SelectedChapterChanged()
        {
            this.NotifyOfPropertyChange(() => TotalPage);
        }

        public void NewComic()
        {
            if (MessageBox.Show("Do you want to add new Comic ?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Comic = new Comic { Chapters = new BindableCollection<Chapter>() };
                var chapter1 = new Chapter { Title = "Chapter", Pages = new BindableCollection<Page>(), Index = 1 };
                this.comic.Chapters.Add(chapter1);
            }
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

        public void EditComic()
        {
            this.windowManager = new WindowManager();

            var newComicModel = new NewComicModel(this.Comic, false);

            windowManager.ShowDialog(newComicModel);
        }

        public void MovePageUp()
        {
            if (!this.HasCurrentPage())
            {
                return;
            }

            if (currentPage.Index == 1)
            {
                return;
            }

            MovePage(true);
        }

        public void MovePageDown()
        {
            if (!this.HasCurrentPage())
            {
                return;
            }

            if (currentPage.Index == currentChapter.Pages.Count)
            {
                return;
            }

            MovePage(false);
        }

        public void MoveChapterUp()
        {
            if (!this.HasCurrentChapter())
            {
                return;
            }

            if (currentChapter.Index == 1)
            {
                return;
            }

            MoveChapter(true);
        }

        public void MoveChapterDown()
        {
            if (!this.HasCurrentChapter())
            {
                return;
            }

            if (currentChapter.Index == comic.Chapters.Count)
            {
                return;
            }

            MoveChapter(false);
        }

        private void MovePage(bool isUp)
        {
            var currentListIndex = currentPage.Index - 1;

            var nextIndex = isUp ? currentListIndex - 1 : currentListIndex + 1;

            Move(currentChapter.Pages, currentListIndex, nextIndex);
        }

        private void MoveChapter(bool isUp)
        {
            var currentListIndex = currentChapter.Index - 1;

            var nexIndex = isUp ? currentListIndex - 1 : currentListIndex + 1;

            Move(comic.Chapters, currentListIndex, nexIndex);
        }

        public static void Move(BindableCollection<Page> list, int oldIndex, int newIndex)
        {
            var nIndex = list[newIndex];
            var upperIndex = list[newIndex].Index;
            var lowerIndex = list[oldIndex].Index;
            list[newIndex] = list[oldIndex];
            list[oldIndex] = nIndex;
            list[newIndex].Index = upperIndex;
            list[oldIndex].Index = lowerIndex;
        }

        public static void Move(BindableCollection<Chapter> list, int oldIndex, int newIndex)
        {
            var nIndex = list[newIndex];
            var upperIndex = list[newIndex].Index;
            var lowerIndex = list[oldIndex].Index;
            list[newIndex] = list[oldIndex];
            list[oldIndex] = nIndex;
            list[newIndex].Index = upperIndex;
            list[oldIndex].Index = lowerIndex;
        }

        public void AddOrUpdateFrame(string id, Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, int index)
        {
            var frame = this.currentPage.Frames.FirstOrDefault(x => x.Id.ToString() == id);
            if (frame == null)
            {
                frame = new Frame
                            {
                                Id = id,
                                Index = index,
                                Coordinates =
                                    new Coordinate
                                        {
                                            TopLeft = ToOriginal(topLeft, scale),
                                            TopRight = ToOriginal(topRight, scale),
                                            BottomLeft = ToOriginal(bottomLeft, scale),
                                            BottomRight = ToOriginal(bottomRight, scale)
                                        }
                            };

                this.currentPage.Frames.Add(frame);
            }
            else
            {
                frame.Coordinates.TopLeft = ToOriginal(topLeft, scale);
                frame.Coordinates.TopRight = ToOriginal(topRight, scale);
                frame.Coordinates.BottomLeft = ToOriginal(bottomLeft, scale);
                frame.Coordinates.BottomRight = ToOriginal(bottomRight, scale);
            }
        }

        private Point ToOriginal(Point point, int scaleInput)
        {
            double x = point.X * scaleInput;
            double y = point.Y * scaleInput;
            return new Point(x, y);
        }

        public void RemoveFrame(string id)
        {
            var frame = this.currentPage.Frames.FirstOrDefault(x => x.Id.ToString() == id);
            if (frame != null)
            {
                this.currentPage.Frames.Remove(frame);
            }
        }

        public void ZoomIn()
        {
            
            if (CurrentPage != null)
            {
                if (CurrentPage.Zoom == 1)
                {
                    return;
                }
                CurrentPage.Zoom = CurrentPage.Zoom - 1;
            }
           
        }

        public void ZoomOut()
        {
            if (CurrentPage != null)
            {
                CurrentPage.Zoom = CurrentPage.Zoom + 1;
            }
        }
    }
}
