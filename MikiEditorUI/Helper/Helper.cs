namespace MikiEditorUI
{
    using System;
    using System.IO;
    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;
    using BusinessObject;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    class Helper
    {
        public bool ExportCompressFile(Comic comic, string originalPath)
        {
            try
            {
                string subPath = string.Empty;

                var imageExtension = string.Empty;

                string path = originalPath + @"\" + comic.Title;

                this.CreateFolder(path); // create comic folder
                
                foreach (var chapter in comic.Chapters)
                {
                    subPath = path + @"\data" + @"\" + chapter.Title + chapter.Index;
                    this.CreateFolder(subPath); // create chapter folder

                    foreach (var page in chapter.Pages)
                    {
                        imageExtension = Path.GetExtension(page.ImgPath);
                        CopyFiles(page.ImgPath, subPath + @"\" + page.Index + imageExtension);
                    }
                }

                this.CopyFiles(comic.CoverPath, path + @"\" + "cover" + Path.GetExtension(comic.CoverPath)); // create cover

                ConvertJson(comic, AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("dMMyyyy"), "tmp");

                ConvertJson(ResetImagePath(CloneComic(comic)), path, "meta", "manga"); // convert meta data file and save to comic folder

                if (!Directory.Exists(originalPath))
                {
                    Directory.CreateDirectory(originalPath);
                }

                FileStream fsOut = File.Create(path + ".zip");

                var zipStream = new ZipOutputStream(fsOut);

                int folderOffset = path.Length  + (originalPath.EndsWith("\\") ? 0 : 1);

                CompressFolder(path, zipStream, folderOffset);

                zipStream.IsStreamOwner = true;

                zipStream.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        public void ConvertJson(Comic comic, string path,string fileName = null, string extension = null)
        {
            path = (path.EndsWith(@"\")) ? path.Substring(0, path.Length - 1) : path;

            string fullPath = (fileName != null && extension != null) ? path + @"\" + fileName + "." + extension : path;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var serializeObject = JsonConvert.SerializeObject(comic, Formatting.Indented, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            File.WriteAllText(fullPath, serializeObject);
        }

        public Comic ConvertToObjectFromJson(string path)
        {
            var comic = JsonConvert.DeserializeObject<Comic>(File.ReadAllText(path));
            return comic;
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

            if (files.Length == 0 && Directory.GetDirectories(path).Length == 0)
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

                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                var buffer = new byte[4096];

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

        private Comic ResetImagePath(Comic comic)
        {
            var coverExtension = Path.GetExtension(comic.CoverPath);
            comic.CoverPath = @"cover" + coverExtension;

            foreach (var chapter in comic.Chapters)
            {
                foreach (var page in chapter.Pages)
                {
                    string extension = Path.GetExtension(page.ImgPath);

                    page.ImgPath = @"data/" + chapter.Title + chapter.Index + @"/" + page.Index + extension;
                }
            }

            return comic;
        }

        private Comic CloneComic(Comic comic)
        {
            var serializeObject = JsonConvert.SerializeObject(comic, Formatting.Indented, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return JsonConvert.DeserializeObject<Comic>(serializeObject);
        }
    }
}
