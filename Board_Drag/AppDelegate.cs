using Foundation;
using UIKit;

namespace Board_Drag
{

    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        Board_DragViewController viewController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            viewController = new Board_DragViewController();
            window.RootViewController = viewController;
            window.MakeKeyAndVisible();

            return true;
        }

    }
}

