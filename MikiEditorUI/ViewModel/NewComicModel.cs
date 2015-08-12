using Caliburn.Micro;

namespace MikiEditorUI.ViewModel
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using BusinessObject;

    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using Screen = Caliburn.Micro.Screen;

    public class NewComicModel : Screen
    {
        private Comic comic;

        private bool isExport;

        private bool isLoading;

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }

            set
            {
                isLoading = value;
                this.NotifyOfPropertyChange(() => OpacityBackgroundLoading);
                this.NotifyOfPropertyChange(() => Visibility);
            }
        }

        public float OpacityBackgroundLoading
        {
            get
            {
                return IsLoading ? (float)0.5 : 1;
            }
        }

        public string Visibility
        {
            get
            {
                return IsLoading ? "Visible" : "Hidden";
            }
        }

        public NewComicModel(Comic _comic, bool isExport)
        {
            this.comic = _comic;
            this.isExport = isExport;
        }

        public Comic Comic
        {
            get
            {
                return comic;
            }
            set
            {
                comic = value;
                this.NotifyOfPropertyChange(() => this.Comic);
            }
        }

        public void OpenSaveWorkSpace()
        {
            var folderDialog = new FolderBrowserDialog { SelectedPath = "C:\\" };
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                comic.WorkSpace = folderDialog.SelectedPath;

        }

        public void LoadCoverPath()
        {
            var op = new OpenFileDialog();
            op.Title = "Select a comic cover";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (op.ShowDialog() == true)
            {
                this.Comic.CoverPath = op.FileName;
            }
        }

        public void ExportComic()
        {
            var helper = new Helper();

            if (comic.Title == null)
            {
                MessageBox.Show("Please input the author information");
                return;
            }

            if (comic.CoverPath == null || !File.Exists(comic.CoverPath))
            {
                MessageBox.Show("Please choose a cover file");
                return;
            }

            this.IsLoading = true;

            if (!isExport)
            {
                MessageBox.Show("Save successfully", "Save Comic Informaiton", MessageBoxButtons.OK);

                this.TryClose();

                return;
            }

            helper.ExportCompressFile(comic, comic.WorkSpace);

            this.IsLoading = false;

            MessageBox.Show("Exported successfully", "Exported", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            comic.Chapters.Clear();

            NotifyOfPropertyChange(() => Comic);

            this.TryClose();
        }

        public void CancelComic()
        {
            this.TryClose();
        }
    }
}
