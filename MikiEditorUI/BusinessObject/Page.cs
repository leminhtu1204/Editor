using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace MikiEditorUI.BusinessObject
{
    public class Page : PropertyChangedBase
    {
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

        public Chapter Chapter { get; set; }

        public int PageIndex
        {
            get
            {
                if (Chapter.Pages == null || Chapter.Pages.Count == 0)
                {
                    return 1;
                }

                return Chapter.Pages.IndexOf(this) + 1;
            }
        }
    }
}
