using System;
using CoreGraphics;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;
using UIKit;
using System.Drawing;
using Foundation;

namespace Board_Drag
{
    public partial class Board_DragViewController : UIViewController
    {

        private static string db_file = "Board.db";
        string dbPath;
        UIButton todoButton, progressButton, doneButton, holdButton;
        UIView todoView, progressView, doneView, holdView, mainView;
        int y = 0;
        UIScrollView todoScroll, progressScroll, doneScroll, holdScroll;
        int cntTodo = 0, cntProgress = 0, cntDone = 0, cntHold = 0;
        string oldState = "";
        UIImageView trashView;
        UILabel lblmsg;
        UIView addTask;
        UIImagePickerController imagePicker;
        private UIPopoverController popOver;
        int photoId;
        UIImageView imgSave;
        bool dragFlag = false;
        int todoY = 0, progressY = 0, doneY = 0, holdY = 0;

        int[] arrY = new int[1000];


        public Board_DragViewController() : base("Board_DragViewController", null)
        {
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
            Console.WriteLine("Harshad PopView Test");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Create Db

            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);
            bool exists = File.Exists(dbPath);

            if (!exists)
                SqliteConnection.CreateFile(dbPath);
            var conn1 = new SqliteConnection("Data Source=" + dbPath);

            Console.WriteLine(dbPath);
            if (!exists)
            {
                CreateDatabase(conn1);
            }

            View.BackgroundColor = UIColor.FromRGB(219, 202, 185);

            var height = UIScreen.MainScreen.Bounds.Height;
            Console.WriteLine("Screen : " + height);

            //			UIScrollView mainScroll = new UIScrollView (new RectangleF (0, 45, 1010, 700));
            //			mainScroll.ContentSize = new SizeF (1110,1124);
            //			  
            mainView = new UIView(new RectangleF(5, 55, 1010, 700));
            mainView.BackgroundColor = UIColor.FromRGB(219, 202, 185);

            var titleView = new UIView(new RectangleF(25, 5, 250, 50));
            titleView.BackgroundColor = UIColor.Black;
            View.AddSubview(titleView);

            var titlename = new UILabel(new RectangleF(10, 5, 100, 40));
            titlename.Text = "Today";
            //titlename.Font = UIFont.FromName ("Helventica",20f);
            titlename.TextColor = UIColor.White;
            titlename.BackgroundColor = UIColor.Clear;
            titleView.AddSubview(titlename);

            trashView = new UIImageView(new RectangleF(575, 400, 250, 280));
            trashView.Image = UIImage.FromFile("images/trash.png");
            mainView.AddSubview(trashView);

            lblmsg = new UILabel(new RectangleF(480, 630, 150, 30));
            lblmsg.Text = "";
            lblmsg.Hidden = true;
            mainView.AddSubview(lblmsg);

            todoView = new UIView(new RectangleF(30, 10, 250, 280));
            todoView.BackgroundColor = UIColor.FromRGB(255, 253, 233);
            todoView.Layer.CornerRadius = 6.0f;

            progressView = new UIView(new RectangleF(305, 10, 250, 280));
            progressView.BackgroundColor = UIColor.FromRGB(255, 253, 233);
            progressView.Layer.CornerRadius = 6.0f;

            doneView = new UIView(new RectangleF(575, 10, 250, 280));
            doneView.BackgroundColor = UIColor.FromRGB(255, 253, 233);
            doneView.Layer.CornerRadius = 6.0f;

            holdView = new UIView(new RectangleF(30, 310, 250, 280));
            holdView.BackgroundColor = UIColor.FromRGB(255, 253, 233);
            holdView.Layer.CornerRadius = 6.0f;

            var btntest = new UILabel(new RectangleF(15, 5, 240, 40));
            btntest.BackgroundColor = UIColor.Clear;
            btntest.Text = "TO DO";
            todoView.AddSubview(btntest);

            var imgTodo = new UIImageView(new RectangleF(220, 13, 18, 18));
            imgTodo.Image = UIImage.FromFile("images/down.png");
            todoView.AddSubview(imgTodo);

            var btnprogress = new UILabel(new RectangleF(15, 5, 240, 40));
            btnprogress.BackgroundColor = UIColor.Clear;
            btnprogress.Text = "IN PROGRESS";
            progressView.AddSubview(btnprogress);

            var imgProgress = new UIImageView(new RectangleF(220, 13, 18, 18));
            imgProgress.Image = UIImage.FromFile("images/down.png");
            progressView.AddSubview(imgProgress);

            var btndone = new UILabel(new RectangleF(15, 5, 240, 40));
            btndone.BackgroundColor = UIColor.Clear;
            btndone.Text = "DONE";
            doneView.AddSubview(btndone);

            var imgDone = new UIImageView(new RectangleF(220, 13, 18, 18));
            imgDone.Image = UIImage.FromFile("images/down.png");
            doneView.AddSubview(imgDone);

            var btnhold = new UILabel(new RectangleF(5, 5, 240, 40));
            btnhold.BackgroundColor = UIColor.Clear;
            btnhold.Text = "ON HOLD";
            holdView.AddSubview(btnhold);

            var imgHold = new UIImageView(new RectangleF(220, 13, 18, 18));
            imgHold.Image = UIImage.FromFile("images/down.png");
            holdView.AddSubview(imgHold);

            todoButton = new UIButton(UIButtonType.Custom);
            todoButton.Frame = new RectangleF(5, 235, 240, 40);
            todoButton.SetBackgroundImage(UIImage.FromFile("images/greybox.png"), UIControlState.Normal);
            todoButton.SetTitle(" +  Add a card...", UIControlState.Normal);
            todoButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            todoButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            todoButton.UserInteractionEnabled = true;
            todoView.AddSubview(todoButton);

            // LONG PRESS CLICK EVENTS

            //		UILongPressGestureRecognizer longpress = new UILongPressGestureRecognizer (); 
            //		longpress.MinimumPressDuration = 1.0; 
            //		todoButton.AddGestureRecognizer (longpress);  


            progressButton = new UIButton(UIButtonType.Custom);
            progressButton.Frame = new RectangleF(5, 235, 240, 40);
            progressButton.SetBackgroundImage(UIImage.FromFile("images/greybox.png"), UIControlState.Normal);
            progressButton.SetTitle(" +  Add a card...", UIControlState.Normal);
            progressButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            progressButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            progressButton.UserInteractionEnabled = true;
            progressView.AddSubview(progressButton);

            doneButton = new UIButton(UIButtonType.Custom);
            doneButton.Frame = new RectangleF(5, 235, 240, 40);
            doneButton.SetBackgroundImage(UIImage.FromFile("images/greybox.png"), UIControlState.Normal);
            doneButton.SetTitle(" +  Add a card...", UIControlState.Normal);
            doneButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            doneButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            doneButton.UserInteractionEnabled = true;
            doneView.AddSubview(doneButton);

            holdButton = new UIButton(UIButtonType.Custom);
            holdButton.Frame = new RectangleF(5, 235, 240, 40);
            holdButton.SetBackgroundImage(UIImage.FromFile("images/greybox.png"), UIControlState.Normal);
            holdButton.SetTitle(" +  Add a card...", UIControlState.Normal);
            holdButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            holdButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            holdButton.UserInteractionEnabled = true;
            holdView.AddSubview(holdButton);


            todoScroll = commanView("todo", todoView);
            progressScroll = commanView("progress", progressView);
            doneScroll = commanView("done", doneView);
            holdScroll = commanView("hold", holdView);


            todoButton.TouchUpInside += (sender, e) =>
            {

                //				popover = new UIPopoverController (new PopupView (this,"todo"));
                //				popover.PopoverContentSize = new SizeF (350f, 300f);
                //				popover.PresentFromRect (new RectangleF(230,10,10,10), todoButton, UIPopoverArrowDirection.Left, true);  
                AddTask("todo", todoScroll, todoButton, todoY);

            };

            progressButton.TouchUpInside += (sender, e) =>
            {
                AddTask("progress", progressScroll, progressButton, progressY);
            };

            doneButton.TouchUpInside += (sender, e) =>
            {
                AddTask("done", doneScroll, doneButton, doneY);
            };

            holdButton.TouchUpInside += (sender, e) =>
            {
                AddTask("hold", holdScroll, holdButton, holdY);
            };

            View.AddSubview(mainView);
            //	mainScroll.AddSubview (mainView);
            mainView.AddSubview(todoView);
            mainView.AddSubview(progressView);
            mainView.AddSubview(doneView);
            mainView.AddSubview(holdView);

        }

        public void AddTask(string state, UIScrollView scrollType, UIButton buttonType, int viewY)
        {
            Console.WriteLine("Clicked :" + viewY);

            buttonType.Hidden = true;

            addTask = new UIView(new RectangleF(0, viewY, 240, 110));
            addTask.BackgroundColor = UIColor.Clear;
            scrollType.ContentSize = new SizeF(245, viewY + 110);
            scrollType.AddSubview(addTask);

            var taskField = new UITextField(new RectangleF(5, 0, 240, 70));
            taskField.Layer.CornerRadius = 5.0f;
            taskField.Layer.BorderWidth = 1.0f;
            taskField.Layer.BorderColor = new CGColor(0, 0, 0);
            taskField.BackgroundColor = UIColor.White;
            addTask.AddSubview(taskField);

            var taskAddButton = new UIButton(UIButtonType.Custom);
            taskAddButton.Frame = new RectangleF(5, 75, 70, 30);
            taskAddButton.BackgroundColor = UIColor.FromRGB(17, 132, 46);
            taskAddButton.Layer.CornerRadius = 5.0f;
            taskAddButton.SetTitle("Add", UIControlState.Normal);
            addTask.AddSubview(taskAddButton);

            var taskCancelButton = new UIButton(UIButtonType.Custom);
            taskCancelButton.Frame = new RectangleF(80, 75, 25, 25);
            taskCancelButton.BackgroundColor = UIColor.Gray;
            taskCancelButton.Layer.CornerRadius = 5.0f;
            taskCancelButton.SetTitle("X", UIControlState.Normal);
            addTask.AddSubview(taskCancelButton);

            addTask.Hidden = false;

            taskAddButton.TouchUpInside += (sender, e) =>
            {
                PopupViewOK(state, taskField.Text, "", false);
                addTask.Hidden = true;
                buttonType.Hidden = false;
            };

            taskCancelButton.TouchUpInside += (sender, e) =>
            {
                addTask.Hidden = true;
                buttonType.Hidden = false;
                scrollType.ContentSize = new SizeF(245, viewY);
            };

            taskField.ShouldReturn += (textField) =>
            {
                textField.ResignFirstResponder();
                return true;
            };

            taskField.BecomeFirstResponder();
        }

        public UIPanGestureRecognizer createPanGesture(UIButton ub, int ID)
        {
            nfloat dx = 0;
            nfloat dy = 0;

            UIPanGestureRecognizer panObj = new UIPanGestureRecognizer((pg) =>
            {

                if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed) && (pg.NumberOfTouches == 1))
                {
                    //if(dragFlag == true) { 					   
                    var p0 = pg.LocationInView(View);

                    if (dx == 0)
                        dx = p0.X - ub.Center.X;

                    if (dy == 0)
                        dy = p0.Y - ub.Center.Y;

                    var p1 = new PointF((float)(p0.X - dx), (float)(p0.Y - dy));

                    ub.Center = p1;
                    //		mainView.Layer.ZPosition = 999;
                    Console.WriteLine("Tag" + ub.Tag);

                    Console.WriteLine("po.x : " + p0.X);
                    Console.WriteLine("po.y : " + p0.Y);
                    Console.WriteLine("BtnTest Center X : " + ub.Center.X);
                    Console.WriteLine("BtnTest Center Y : " + ub.Center.Y);

                    if (p0.X > 0 && p0.X < 290 && p0.Y < 335)
                    {
                        ub.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 15);
                        todoView.BackgroundColor = UIColor.Green;
                        progressView.BackgroundColor = Theme.Box;
                        doneView.BackgroundColor = Theme.Box;
                        holdView.BackgroundColor = Theme.Box;
                    }
                    else if (p0.X > 290 && p0.X < 600 && p0.Y < 400)
                    {
                        ub.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 15);
                        progressView.BackgroundColor = UIColor.Green;
                        todoView.BackgroundColor = Theme.Box;
                        doneView.BackgroundColor = Theme.Box;
                        holdView.BackgroundColor = Theme.Box;
                    }
                    else if (p0.X > 600 && p0.X < 910 && p0.Y < 400)
                    {
                        ub.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 15);
                        doneView.BackgroundColor = UIColor.Green;
                        todoView.BackgroundColor = Theme.Box;
                        progressView.BackgroundColor = Theme.Box;
                        holdView.BackgroundColor = Theme.Box;
                    }
                    else if (p0.X > 0 && p0.X < 370 && p0.Y > 335 && p0.Y < 620)
                    {
                        ub.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 15);
                        holdView.BackgroundColor = UIColor.Green;
                        todoView.BackgroundColor = Theme.Box;
                        progressView.BackgroundColor = Theme.Box;
                        doneView.BackgroundColor = Theme.Box;
                    }
                    else
                    {
                        todoView.BackgroundColor = Theme.Box;
                        progressView.BackgroundColor = Theme.Box;
                        doneView.BackgroundColor = Theme.Box;
                        holdView.BackgroundColor = Theme.Box;
                    }
                    if (pg.State == UIGestureRecognizerState.Began)
                    {
                        if (oldState == "")
                        {
                            if (p0.X > 0 && p0.X < 290 && p0.Y < 335)
                            {
                                oldState = "todo";
                            }
                            else if (p0.X > 290 && p0.X < 600 && p0.Y < 400)
                            {
                                oldState = "progress";
                            }
                            else if (p0.X > 600 && p0.X < 910 && p0.Y < 400)
                            {
                                oldState = "done";
                            }
                            else if (p0.X > 0 && p0.X < 370 && p0.Y > 335 && p0.Y < 620)
                            {
                                oldState = "hold";
                            }
                        }
                    }
                    //}

                }
                else if (pg.State == UIGestureRecognizerState.Ended)
                {

                    dragFlag = false;
                    dx = 0;
                    dy = 0;

                    var p00 = pg.LocationInView(View);

                    if (p00.X > 0 && p00.X < 290 && p00.Y < 335)
                    {
                        Console.WriteLine("old state : " + oldState);
                        UpdateDatabase("todo", ID);
                        try
                        {

                            todoScroll.RemoveFromSuperview();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        //ub.BringSubviewToFront(todoView);


                        todoView.Layer.ZPosition = 0;
                        progressView.Layer.ZPosition = 0;
                        doneView.Layer.ZPosition = 0;
                        holdView.Layer.ZPosition = 0;
                        todoView.BackgroundColor = Theme.Box;

                        cntTodo++;

                    }
                    else if (p00.X > 290 && p00.X < 600 && p00.Y < 400)
                    {
                        Console.WriteLine("old state : " + oldState);
                        UpdateDatabase("progress", ID);
                        try
                        {

                            progressScroll.RemoveFromSuperview();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        progressScroll = commanView("progress", progressView);

                        progressView.Layer.ZPosition = 10;
                        doneView.Layer.ZPosition = 0;
                        holdView.Layer.ZPosition = 0;
                        progressView.BackgroundColor = Theme.Box;
                        cntProgress++;
                    }
                    else if (p00.X > 600 && p00.X < 910 && p00.Y < 400)
                    {
                        Console.WriteLine("old state : " + oldState);

                        UpdateDatabase("done", ID);
                        try
                        {

                            doneScroll.RemoveFromSuperview();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        doneScroll = commanView("done", doneView);

                        doneView.Layer.ZPosition = 10;
                        holdView.Layer.ZPosition = 0;
                        doneView.BackgroundColor = Theme.Box;
                        cntDone++;
                    }
                    else if (p00.X > 0 && p00.X < 370 && p00.Y > 335 && p00.Y < 620)
                    {
                        Console.WriteLine("old state : " + oldState);
                        UpdateDatabase("hold", ID);
                        try
                        {

                            holdScroll.RemoveFromSuperview();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        holdScroll = commanView("hold", holdView);

                        holdView.Layer.ZPosition = 10;
                        holdView.BackgroundColor = Theme.Box;
                        cntHold++;

                    }
                    else
                    {

                        lblmsg.Hidden = false;
                        lblmsg.Text = "1 Item Deleted";
                        DeleteDatabase((int)ub.Tag);

                        Console.WriteLine("date " + new NSDate());
                        Action action = () =>
                        {
                            Console.WriteLine("action fire ");
                            lblmsg.Hidden = true;
                        };

                        NSTimer.CreateScheduledTimer(5, (NSTimer obj) => action.Invoke());

                    }

                    reloadView(oldState);

                    var p01 = new PointF((float)(p00.X - dx), (float)(p00.Y - dy));
                    ub.Center = p01;

                }
            });

            return panObj;
        }

        public void reloadView(string state)
        {
            if (state == "todo")
            {

                try
                {

                    todoScroll.RemoveFromSuperview();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                todoScroll = commanView(state, todoView);
            }
            else if (state == "progress")
            {
                try
                {

                    progressScroll.RemoveFromSuperview();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                progressScroll = commanView(state, progressView);
            }
            else if (state == "done")
            {
                try
                {

                    doneScroll.RemoveFromSuperview();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                doneScroll = commanView(state, doneView);
            }
            else if (state == "hold")
            {
                try
                {

                    holdScroll.RemoveFromSuperview();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                holdScroll = commanView(state, holdView);
            }

            oldState = "";
        }

        public void PopupViewOK(string state, string title, string details, bool uimage)
        {
            Console.WriteLine("Test OK");

            InsertDatabase(state, title, details, uimage);

            Console.WriteLine("OK STate : " + state);

            reloadView(state);

            //popover.Dismiss (true);
        }
        private static void CreateDatabase(SqliteConnection connection)
        {
            var sql = "CREATE TABLE Board (Id INTEGER PRIMARY KEY AUTOINCREMENT,state varchar,title varchar, details varchar, uimage varchar)";

            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

        }

        private void InsertDatabase(string state, string title, string details, bool uimage)
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

            var connection = new SqliteConnection("Data Source=" + dbPath);
            connection.Open();

            var sql = "insert into Board (state,title,details,uimage) values (@state,@title,@details,@uimage)";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@state", state);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@details", details);
                cmd.Parameters.AddWithValue("@uimage", uimage);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            //				
            //			if (uimage == true) { 
            //				var sql1 = "SELECT id from board order by id DESC limit 1";
            //				using (var cmd= connection.CreateCommand()) {
            //					cmd.CommandText = sql1;	 
            //					SqliteDataAdapter da = new SqliteDataAdapter (cmd);
            //					DataSet ds = new DataSet ();
            //					da.Fill (ds);
            //				 
            //					string documentsDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
            //					string sourceFile = System.IO.Path.Combine (documentsDirectory, "Photo.png"); 
            //					string destinationFile = System.IO.Path.Combine (documentsDirectory, ds.Tables [0].Rows [0] ["id"].ToString () + ".png");
            //					File.Move (sourceFile, destinationFile);
            //				}  
            //			} else {
            //				Console.WriteLine ("Without Image sucess");
            //			}
        }

        private void DeleteDatabase(int id)
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

            var connection = new SqliteConnection("Data Source=" + dbPath);
            connection.Open();

            var sql = "delete from board where id = @id";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            connection.Close();

        }

        private void UpdateDatabase(string state, int id)
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

            var connection = new SqliteConnection("Data Source=" + dbPath);

            connection.Open();

            var sql = "update Board set state=@state where id = @id ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@state", state);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        private void UpdateDatabaseImage(bool uimage, int id)
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

            Console.WriteLine("UpdateID : " + id);

            var connection = new SqliteConnection("Data Source=" + dbPath);

            connection.Open();

            var sql = "update Board set uimage=@uimage where id = @id ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@uimage", uimage);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        public UIScrollView commanView(string state, UIView viewType)
        {

            todoButton.Hidden = false;
            progressButton.Hidden = false;
            doneButton.Hidden = false;
            holdButton.Hidden = false;

            UIScrollView scrollType = new UIScrollView(new RectangleF(0, 45, 245, 180));

            var conn3 = new SqliteConnection("Data Source=" + dbPath);
            conn3.Open();

            var sql = "select * from board where state=@state";
            using (var cmd = conn3.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@state", state);
                SqliteDataAdapter da = new SqliteDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //scrollType.ScrollEnabled = true;
                    Console.WriteLine(ds.Tables[0].Rows.Count);
                    y = 0;
                    y = y + 5;

                    int len = ds.Tables[0].Rows.Count;
                    cntTodo = len;

                    int scollHeight = 10 + (((int)(len)) * 60);


                    scrollType.ContentSize = new SizeF(245, scollHeight);

                    viewType.AddSubview(scrollType);

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        UIButton MainButton = new UIButton(UIButtonType.Custom);
                        MainButton.Tag = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                        Console.WriteLine("MainButtonID : " + MainButton.Tag);

                        MainButton.Frame = new RectangleF(5, y, 240, 50);
                        MainButton.Layer.BorderWidth = 2.0f;
                        MainButton.Layer.BorderColor = new CGColor(255, 0, 0);
                        MainButton.Layer.CornerRadius = 6.0f;

                        arrY[MainButton.Tag] = y + 110;

                        MainButton.AccessibilityValue = y.ToString();

                        Console.WriteLine("ACCESSIBILITYVALUE : " + MainButton.AccessibilityValue);

                        //						MainButton.Transform = CGAffineTransform.MakeRotation ((float)Math.PI / 15);  
                        //						MainButton.SetBackgroundImage (UIImage.FromFile ("images/greybox.png"), UIControlState.Normal);

                        MainButton.BackgroundColor = UIColor.White;
                        MainButton.UserInteractionEnabled = true;

                        var lblname = new UILabel(new RectangleF(5, 5, 150, 20));
                        lblname.Text = ds.Tables[0].Rows[i]["title"].ToString();

                        var lbldetails = new UILabel(new RectangleF(5, 25, 150, 20));
                        lbldetails.Text = ds.Tables[0].Rows[i]["details"].ToString();

                        UIImageView imgView = new UIImageView(new RectangleF(175, 4, 60, 42));

                        var imgButton = new UIButton(UIButtonType.Custom);
                        imgButton.Frame = new RectangleF(173, 2, 65, 46);
                        //imgButton.Layer.BorderWidth = 1.0f;  
                        //						imgButton.Layer.CornerRadius = 6.0f;


                        string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string destinationFile = System.IO.Path.Combine(documentsDirectory, ds.Tables[0].Rows[i]["id"].ToString() + ".png");

                        imgView.Image = UIImage.FromFile(destinationFile);

                        UIPanGestureRecognizer panObj = createPanGesture(MainButton, (int)MainButton.Tag);
                        MainButton.AddGestureRecognizer(panObj);

                        MainButton.AddSubview(lblname);
                        MainButton.AddSubview(lbldetails);
                        MainButton.AddSubview(imgView);
                        MainButton.AddSubview(imgButton);
                        scrollType.AddSubview(MainButton);
                        MainButton.BringSubviewToFront(scrollType);
                        y += 60;


                        UITapGestureRecognizer doubletap = new UITapGestureRecognizer();
                        doubletap.NumberOfTapsRequired = 2; // double tap
                        doubletap.AddTarget(this, new ObjCRuntime.Selector("DoubleTapSelector:"));
                        MainButton.AddGestureRecognizer(doubletap);

                        UILongPressGestureRecognizer longpress = new UILongPressGestureRecognizer();
                        longpress.MinimumPressDuration = 1;
                        longpress.AddTarget(this, new ObjCRuntime.Selector("LongPressSelector:"));
                        MainButton.AddGestureRecognizer(longpress);

                        imgButton.TouchUpInside += (sender, e) =>
                        {
                            Console.WriteLine("ImageButton Clicked ");

                            imagePicker = new UIImagePickerController();
                            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
                            photoId = (int)MainButton.Tag;
                            imgSave = imgView;
                            PickImage("png", "1", UIImagePickerControllerSourceType.PhotoLibrary, imgButton);

                        };

                    }

                }
                else
                {

                    scrollType.ContentSize = new SizeF(245, 50);
                    viewType.AddSubview(scrollType);

                    var lblviewMsg = new UILabel(new RectangleF(10, 50, 240, 25));
                    lblviewMsg.Text = "No tasks are done yet..";
                    lblviewMsg.BackgroundColor = UIColor.Clear;
                    lblviewMsg.TextColor = UIColor.FromRGB(219, 219, 120);
                    scrollType.AddSubview(lblviewMsg);

                }
                if (state == "todo")
                {
                    todoY = y;
                }
                else if (state == "progress")
                {
                    progressY = y;
                }
                else if (state == "done")
                {
                    doneY = y;
                }
                else
                {
                    holdY = y;
                }
            }
            return scrollType;
        }


        public void dragView(int ID)
        {

            var conn3 = new SqliteConnection("Data Source=" + dbPath);
            conn3.Open();

            var sql = "select * from board where id=@id";
            using (var cmd = conn3.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", ID);
                SqliteDataAdapter da = new SqliteDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    UIButton MainButton = new UIButton(UIButtonType.Custom);


                    Console.WriteLine("MainButtonID : " + MainButton.Tag);

                    MainButton.Frame = new RectangleF(20, arrY[ID], 240, 50);
                    MainButton.Layer.BorderWidth = 2.0f;
                    MainButton.Layer.BorderColor = new CGColor(255, 0, 0);
                    MainButton.Layer.CornerRadius = 6.0f;
                    MainButton.Tag = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

                    MainButton.BackgroundColor = UIColor.White;
                    MainButton.UserInteractionEnabled = true;
                    MainButton.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 20);


                    var lblname = new UILabel(new RectangleF(5, 5, 150, 20));
                    lblname.Text = ds.Tables[0].Rows[0]["title"].ToString();

                    var lbldetails = new UILabel(new RectangleF(5, 25, 150, 20));
                    lbldetails.Text = ds.Tables[0].Rows[0]["details"].ToString();

                    UIImageView imgView = new UIImageView(new RectangleF(175, 4, 60, 42));

                    var imgButton = new UIButton(UIButtonType.Custom);
                    imgButton.Frame = new RectangleF(173, 2, 65, 46);

                    string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string destinationFile = System.IO.Path.Combine(documentsDirectory, ds.Tables[0].Rows[0]["id"].ToString() + ".png");

                    imgView.Image = UIImage.FromFile(destinationFile);

                    UIPanGestureRecognizer panObj = createPanGesture(MainButton, (int)MainButton.Tag);
                    MainButton.AddGestureRecognizer(panObj);

                    MainButton.AddSubview(lblname);
                    MainButton.AddSubview(lbldetails);
                    MainButton.AddSubview(imgView);
                    MainButton.AddSubview(imgButton);
                    View.AddSubview(MainButton);


                }
            }
        }

        [Export("LongPressSelector:")]
        public void OnLongPress(UIGestureRecognizer sender)
        {
            //	if(dragFlag == false) {
            Console.WriteLine("LongPress Clicked");

            UIGestureRecognizer btn = (UIGestureRecognizer)sender;
            UIButton btn2 = (UIButton)btn.View;
            btn2.BringSubviewToFront(btn.View);
            Console.WriteLine("id - {0}", btn2.Tag);
            dragFlag = true;
        }

        [Export("DoubleTapSelector:")]
        public void OnDoubleTap(UIGestureRecognizer sender)
        {
            Console.WriteLine("Doouble Clicked");

            todoView.Layer.ZPosition = 0;
            progressView.Layer.ZPosition = 0;
            doneView.Layer.ZPosition = 0;
            holdView.Layer.ZPosition = 0;

            var popView = new UIView(new RectangleF(300, 200, 350, 250));
            popView.BackgroundColor = UIColor.FromRGB(223, 231, 255);
            popView.Layer.CornerRadius = 6.0f;

            var txtname = new UITextField(new RectangleF(10, 10, 330, 80));
            txtname.Layer.CornerRadius = 5.0f;
            txtname.BackgroundColor = UIColor.White;
            popView.AddSubview(txtname);

            var btnok = UIButton.FromType(UIButtonType.RoundedRect);
            btnok.Frame = new RectangleF(125, 140, 100, 40);
            btnok.SetTitle("OK", UIControlState.Normal);
            popView.AddSubview(btnok);

            mainView.AddSubview(popView);

            popView.Hidden = false;

            btnok.TouchUpInside += delegate
            {
                popView.Hidden = true;
            };
        }

        private void PickImage(string fileExtension, string contextIdValue, UIImagePickerControllerSourceType sourceType, UIButton ub)
        {
            Console.WriteLine("photoId  :" + photoId);

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
                        imgSave.Image = originalImage;

                        var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                        Console.WriteLine("photoIdSECOND  :" + photoId);

                        string jpgFilename = System.IO.Path.Combine(documentsDirectory, photoId + ".png");
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
                        popOver.Dismiss(true);
                        UpdateDatabaseImage(true, photoId);
                    }

                    // get the edited image
                    UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                    if (editedImage != null)
                    {
                        // do something with the image
                        Console.WriteLine("got the edited image");
                        imgSave.Image = editedImage;
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
                popOver.PresentFromRect(new RectangleF(55, 10, 10, 10), ub, UIPopoverArrowDirection.Left, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return true;
        }
    }
}

