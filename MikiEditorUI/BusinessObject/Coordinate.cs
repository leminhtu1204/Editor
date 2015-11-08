namespace MikiEditorUI.BusinessObject
{
    using System.Windows;

    /// <summary>
    /// The coordinate.
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Gets or sets the top left.
        /// </summary>
        public FramePoint TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the top right.
        /// </summary>
        public FramePoint TopRight { get; set; }

        /// <summary>
        /// Gets or sets the bottom left.
        /// </summary>
        public FramePoint BottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the bottom right.
        /// </summary>
        public FramePoint BottomRight { get; set; }

    }
}
