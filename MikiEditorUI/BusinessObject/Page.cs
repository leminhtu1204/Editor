using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    using System.Drawing;
    using System.IO;

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

        public int ImgWidth
        {
            get
            {
                return this.GetWidth(imgPath);
            }
        }

        public int ImgHeight
        {
            get
            {
                return this.GetHeight(imgPath);
            }
        }

        private int GetWidth(string imgPath)
        {
            if (imgPath != null)
            {
                try
                {
                    Image temp = System.Drawing.Image.FromFile(imgPath);
                    return temp.Width;
                }
                catch (Exception)
                {

                    return 0;
                }
               
            }
            return 0;
        }

        private int GetHeight(string imgPath)
        {
            if (imgPath != null)
            {
                try
                {
                    Image temp = System.Drawing.Image.FromFile(imgPath);
                    return temp.Height;
                }
                catch (Exception)
                {

                    return 0;
                }
               
            }
            return 0;
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
