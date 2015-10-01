using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    public class Page : PropertyChangedBase
    {
        public Page()
        {
            frames = new BindableCollection<Frame>();
        }

        public string Caption { get; set; }

        private string imgPath;
        public string ImgPath
        {
            get
            {
                return imgPath;
            }

            set
            {
                imgPath = value;
                this.NotifyOfPropertyChange(() => this.ImgPath);
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                this.NotifyOfPropertyChange(() => this.Index);
            }
        }

        private BindableCollection<Frame> frames;
        public BindableCollection<Frame> Frames
        {
            get
            {
                return frames;
            }

            set
            {
                frames = value;
                this.NotifyOfPropertyChange(() => this.Frames);
            }
        }
    }
}
