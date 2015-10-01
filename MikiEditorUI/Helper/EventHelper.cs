using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MikiEditorUI
{
    public static class EventHelper
    {
        public static List<MouseButtonEventHandler> delegates = new List<MouseButtonEventHandler>();

        /// <summary>
        /// The remove all events.
        /// </summary>
        public static void RemoveAllEvents(AdornerLayer alayer)
        {
            foreach (MouseButtonEventHandler eh in delegates)
            {
                alayer.PreviewMouseLeftButtonUp -= eh;
            }

            delegates.Clear();
        }
    }
}
