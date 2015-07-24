namespace MikiEditorUI
{
    using System;
    using System.IO;
    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;
    using MikiEditorUI.BusinessObject;

    class Helper
    {
        public bool ExportCompressFile(Comic comic, string originalPath)
        {
            try
            {
                string path = originalPath + comic.Title;

                this.CreateFolder(path); // create comic folder
                
                foreach (var chapter in comic.Chapters)
                {
                    this.CreateFolder(path + @"\" + chapter.Title + chapter.ChapterIndex); // create chapter folder

                    foreach (var page in chapter.Pages)
                    {
                        CopyFiles(page.ImgPath, path + @"\" + chapter.Title + @"\" + page.PageIndex + ".jpg");
                    }
                }

                ConvertJson(comic, path); // convert meta data file and save to comic folder

                if (!Directory.Exists(originalPath))
                {
                    Directory.CreateDirectory(originalPath);
                }

                FileStream fsOut = File.Create(originalPath + @"\" + comic.Title + ".magatana");

                ZipOutputStream zipStream = new ZipOutputStream(fsOut);

                CompressFolder(path, zipStream, 1);

                zipStream.IsStreamOwner = true;

                zipStream.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        private void ConvertJson(Comic comic, string path)
        {
            var serializeObject = Newtonsoft.Json.JsonConvert.SerializeObject(comic);
            File.WriteAllText(path + @"\meta.manga", serializeObject);
        }

        private void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CopyFiles(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath, true);
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
