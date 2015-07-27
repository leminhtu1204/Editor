using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MikiEditorUI.BusinessObject
{
    using Caliburn.Micro;

    public class Comic : PropertyChangedBase
    {
        private string title;
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                this.NotifyOfPropertyChange(() => this.Title);
            }
        }

        private string author;
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                author = value;
                this.NotifyOfPropertyChange(() => this.Author);
            }
        }

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                this.NotifyOfPropertyChange(() => this.Description);
            }
        }

        private BindableCollection<Chapter> chapters;

        public BindableCollection<Chapter> Chapters
        {
            get
            {
                return chapters;
            }
            set
            {
                chapters = value;
                this.NotifyOfPropertyChange(() => this.Chapters);
            }
        }

        private string workSpace;

        public string WorkSpace
        {
            get
            {
                return workSpace;
            }
            set
            {
                workSpace = value;
                this.NotifyOfPropertyChange(() => this.WorkSpace);
            }
        }
    }
}
