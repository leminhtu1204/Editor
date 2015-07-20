using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MikiEditor.BusinessObject
{
    class Comic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public IList<Chapter> Chapters { get; set; } 
    }
}
