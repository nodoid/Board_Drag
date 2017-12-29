using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Board_Drag
{
    public partial class PopupView : UIViewController
    {
        Board_DragViewController bdv;
        UIImageView imageView;
        private UIPopoverController popOver;
        UIImagePickerController imagePicker;
        UIButton choosePhotoButton;
        bool uimage = false;
        string state;


        public PopupView() : base("PopupView", null)
        {
        }
        public PopupView(Board_DragViewController bd, string state) : base("PopupView", null)
        {
            this.bdv = bd;
            this.state = state;
        }
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("harshad POPVIEW");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            var popView = new UIView(new RectangleF(0, 0, 350, 300));
            popView.BackgroundColor = UIColor.FromRGB(223, 231, 255);

            var txtname = new UITextField(new RectangleF(10, 10, 330, 40));
            txtname.Layer.CornerRadius = 5.0f;
            txtname.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            txtname.Placeholder = "Name";
            txtname.BackgroundColor = UIColor.White;
            popView.AddSubview(txtname);

            var txtdetails = new UITextField(new RectangleF(10, 60, 330, 40));
            txtdetails.Layer.CornerRadius = 5.0f;
            txtdetails.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            txtdetails.Placeholder = "Details";
            txtdetails.BackgroundColor = UIColor.White;
            popView.AddSubview(txtdetails);

            choosePhotoButton = UIButton.FromType(UIButtonType.RoundedRect);
            choosePhotoButton.Frame = new RectangleF(110, 120, 130, 40);
            choosePhotoButton.SetTitle("ImagePicker", UIControlState.Normal);
            popView.AddSubview(choosePhotoButton);

            var btnok = UIButton.FromType(UIButtonType.RoundedRect);
            btnok.Frame = new RectangleF(55, 230, 100, 40);
            btnok.SetTitle("OK", UIControlState.Normal);
            popView.AddSubview(btnok);

            var btnclose = UIButton.FromType(UIButtonType.RoundedRect);
            btnclose.Frame = new RectangleF(185, 230, 100, 40);
            btnclose.SetTitle("Close", UIControlState.Normal);
            popView.AddSubview(btnclose);

            imageView = new UIImageView(new RectangleF(20, 120, 60, 50));
            imageView.Layer.BorderWidth = 2.0f;
            imageView.Layer.BorderColor = new CGColor(127, 127, 127);
            popView.AddSubview(imageView);

            choosePhotoButton.TouchUpInside += (s, e) =>
            {

                imagePicker = new UIImagePickerController();
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

                PickImage("png", "1", UIImagePickerControllerSourceType.PhotoLibrary);
            };

            btnok.TouchUpInside += delegate
            {

                bdv.PopupViewOK(state, txtname.Text, txtdetails.Text, uimage);

            };

            btnclose.TouchUpInside += (sender, e) =>
            {
                //bdv.ClosePopup();
            };

            View.Add(popView);
        }

        private void PickImage(string fileExtension, string contextIdValue, UIImagePickerControllerSourceType sourceType)
        {
            imagePicker = new UIImagePickerController
            {
                SourceType = sourceType,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };
            imagePicker.FinishedPickingMedia += (sender, e) =>
            {
                Console.WriteLine("Harshad");

                bool isImage = false;
                switch (e.Info[UIImagePickerController.MediaType].ToString())
                {
                    case "public.image":
                        Console.WriteLine("Image selected");
                        isImage = true;
                        break;

                    case "public.video":
                        Console.WriteLine("Video selected");
                        break;
                }

                Console.Write("Reference URL: [" + UIImagePickerController.ReferenceUrl + "]");

                // get common info (shared between images and video)
                NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
                if (referenceURL != null)
                    Console.WriteLine(referenceURL.ToString());

                // if it was an image, get the other image info
                if (isImage)
                {

                    // get the original image
                    UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;

                    if (originalImage != null)
                    {
                        // do something with the image
                        Console.WriteLine("got the original image");
                        imageView.Image = originalImage;

                        var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string jpgFilename = System.IO.Path.Combine(documentsDirectory, "Photo.png");
                        NSData imgData = originalImage.AsJPEG();
                        NSError err = null;
                        if (imgData.Save(jpgFilename, false, out err))
                        {
                            Console.WriteLine("saved as " + jpgFilename);
                        }
                        else
                        {
                            Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
                        }

                        uimage = true;

                        popOver.Dismiss(true);

                    }

                    // get the edited image
                    UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                    if (editedImage != null)
                    {
                        // do something with the image
                        Console.WriteLine("got the edited image");
                        imageView.Image = editedImage;
                    }

                    //- get the image metadata
                    NSDictionary imageMetadata = e.Info[UIImagePickerController.MediaMetadata] as NSDictionary;
                    if (imageMetadata != null)
                    {
                        // do something with the metadata
                        Console.WriteLine("got image metadata");
                    }

                }
                // if it's a video
                else
                {
                    // get video url
                    NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                    if (mediaURL != null)
                    {
                        //
                        Console.WriteLine(mediaURL.ToString());
                    }
                }
            };
            imagePicker.Canceled += (sender, evt) =>
            {
                imagePicker.DismissViewController(true, null);
                imagePicker.Dispose();
            };

            if (popOver == null || popOver.ContentViewController == null)
            {
                popOver = new UIPopoverController(imagePicker);
            }
            if (popOver.PopoverVisible)
            {
                popOver.Dismiss(true);
                imagePicker.Dispose();
                popOver.Dispose();
                return;
            }
            try
            {
                popOver.PresentFromRect(new RectangleF(120, 10, 10, 10), choosePhotoButton, UIPopoverArrowDirection.Left, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}

