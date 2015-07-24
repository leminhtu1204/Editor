namespace MikiEditorUI.ViewModel
{
    using System.Windows.Forms;

    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

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

        public void SaveComic()
        {
            var result = comic;
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
            Helper helper = new Helper();
            helper.ExportCompressFile(comic, comic.WorkSpace);
        }

    }
}
