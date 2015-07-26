namespace MikiEditorUI.ViewModel
{
    using System.Windows.Forms;

    using Caliburn.Micro;

    using BusinessObject;

    public class NewComicModel : PropertyChangedBase
    {
        private Comic comic;

        public NewComicModel(Comic _comic)
        {
            this.comic = _comic;
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

        public void ExportComic()
        {
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
        }

    }
}
