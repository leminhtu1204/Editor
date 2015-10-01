using System;

namespace MikiEditorUI.BusinessObject
{
    using System.Collections.Generic;

    /// <summary>
    /// The frame.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public Coordinate Coordinates { get; set; }
    }
}
