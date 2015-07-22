using System.Collections.Generic;
using System.IO;
using System.Windows;
using MikiEditor.BusinessObject;
using ICSharpCode.SharpZipLib;      
namespace MikiEditor
{
    using System;
    using System.IO.Compression;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        delegate void ProgressDelegate(string sMessage);

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

            if (!Directory.Exists(@"C:\zip"))
            {
                Directory.CreateDirectory(@"C:\zip");
            }

            FileStream fsOut = File.Create(@"C:\zip" + @"\manga.zip");

            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            CompressFolder(path, zipStream, 1);

            zipStream.IsStreamOwner = true;

            zipStream.Close();
            

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
            System.IO.File.Copy(sourcePath, destinationPath, true);
        }

        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(path);

            if (files.Length == 0)
            {
                return;
            }

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

    }
}
