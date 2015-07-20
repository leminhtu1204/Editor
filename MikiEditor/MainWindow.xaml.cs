using System.Collections.Generic;
using System.IO;
using System.Windows;
using MikiEditor.BusinessObject;

namespace MikiEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("test");
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            var page1 = new Page {Caption = "test", ImgPath = @"C:\image\1.jpg", PageIndex = 1};
            var page2 = new Page { Caption = "test1", ImgPath = @"C:\image\2.jpg", PageIndex = 2 };

            var listPage = new List<Page> {page1, page2};

            var chapter1 = new Chapter {Title = "Chapter1", Pages = listPage, Index = 1};

            var listChapter = new List<Chapter> {chapter1};

            var comic = new Comic
            {
                Author = "Le Minh Tu",
                Chapters = listChapter,
                Description = "1 nua ranma",
                Id = 1,
                Title = "1_nua_ranma"
            };

            var path = @"C:\" + comic.Title;

            CreateFolder(comic, path); // create parent folder

            foreach (var chap in comic.Chapters)
            {
                CreateFolder(comic, path + @"\" + chap.Title); // create sub folder

                foreach (var page in chap.Pages)
                {
                    CopyFiles(page.ImgPath, path + @"\" + chap.Title + @"\" + page.PageIndex + ".jpg");
                }
            }

            ConvertJson(comic, path); // create meta data

        }

        private void ConvertJson(Comic comic, string path)
        {
           var test = Newtonsoft.Json.JsonConvert.SerializeObject(comic);
           System.IO.File.WriteAllText(path + @"\meta.manga", test);
        }

        private void CreateFolder(Comic comic, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void CopyFiles(string sourcePath, string destinationPath)
        {
            System.IO.File.Copy(sourcePath, destinationPath);
        }

    }
}
