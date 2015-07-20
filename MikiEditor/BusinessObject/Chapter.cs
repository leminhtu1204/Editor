using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MikiEditor.BusinessObject
{
    class Chapter
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public IList<Page> Pages { get; set; } 
    }
}
