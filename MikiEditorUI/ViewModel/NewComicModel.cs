namespace MikiEditorUI.ViewModel
{
    using System.Windows.Forms;

    using Caliburn.Micro;

    using MikiEditorUI.BusinessObject;

    public class NewComicModel : PropertyChangedBase
    {
        private WindowManager windowManager;

        private Comic comic = new Comic();

        public NewComicModel()
        {
            
        }

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
            LoadMainForm();
        }

        public void OpenSaveWorkSpace()
        {
            var folderDialog = new FolderBrowserDialog { SelectedPath = "C:\\" };
            DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                comic.WorkSpace = folderDialog.SelectedPath;

        }

        public void LoadMainForm()
        {
            windowManager = new WindowManager();

            var shellViewModel = new ShellViewModel(this.Comic);

            windowManager.ShowDialog(shellViewModel);

        }
    }
}
