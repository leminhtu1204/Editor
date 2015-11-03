using System;
using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    using System.Drawing;

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

        private int zoom = 4;
        public int Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
                this.NotifyOfPropertyChange(() => this.Zoom);
                this.NotifyOfPropertyChange(() => this.ImgHeight);
                this.NotifyOfPropertyChange(() => this.ImgWidth);
            }
        }

        public int ImgWidth
        {
            get
            {
                return this.GetWidth(imgPath) / Zoom;
            }
        }


        public int ImgHeight
        {
            get
            {
                return this.GetHeight(imgPath) / Zoom;
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
