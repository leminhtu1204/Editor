namespace MikiEditorUI.ViewModel
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Caliburn.Micro;

    using BusinessObject;

    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using Screen = Caliburn.Micro.Screen;

    public class NewComicModel : Screen
    {
        private Comic comic;

        private Thread thread;

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

        public NewComicModel(Comic _comic)
        {
            this.comic = _comic;
            this.thread = thread;
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
            this.IsLoading = true;
            var helper = new Helper();
            if (comic.Title == null)
            {
                MessageBox.Show("Please input the author information");
                return;
            }

            if (comic.WorkSpace == null)
            {
                MessageBox.Show("Please input the work space information");
                return;
            }

            helper.ExportCompressFile(comic, comic.WorkSpace);

            this.IsLoading = false;

            this.TryClose();
        }
    }
}
