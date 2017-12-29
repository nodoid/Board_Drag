using System;
using UIKit;

namespace Board_Drag
{
    public static class Theme
    {
        static Lazy<UIColor> box = new Lazy<UIColor>(() => UIColor.FromPatternImage(UIImage.FromFile("images/box.png")));

        public static UIColor Box => box.Value;
        public static int dragHeight { get; set; }

    }
}

