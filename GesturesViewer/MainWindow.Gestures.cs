using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;


namespace GesturesViewer
{
    partial class MainWindow
    {
        void LoadCircleGestureDetector()
        {
            Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate);
            circleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
            circleGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
            circleGestureRecognizer.OnGestureDetected += OnGestureDetected;

            templates.ItemsSource = circleGestureRecognizer.LearningMachine.Paths;
        }

        //rectangle
        void LoadRectangleGestureDetector()
        {
            Stream recordStream = File.Open(rectangleKBPath, FileMode.OpenOrCreate);
            rectangleGestureRecognizer = new TemplatedGestureDetector("Rectangle", recordStream);
            rectangleGestureRecognizer.TraceTo(gesturesCanvas, Colors.Blue);
            rectangleGestureRecognizer.OnGestureDetected += OnGestureDetected;

            //templates.ItemsSource = rectangleGestureRecognizer.LearningMachine.Paths;
            
        }

        //new gesture Step7: create new method of Load Gesture Detector
        void LoadPigtailGestureDetector()
        {
            Stream recordStream = File.Open(pigtailKBPath, FileMode.OpenOrCreate);
            pigtailGestureRecognizer = new TemplatedGestureDetector("Pigtail", recordStream);
            pigtailGestureRecognizer.TraceTo(gesturesCanvas, Colors.Pink);
            pigtailGestureRecognizer.OnGestureDetected += OnGestureDetected;

            //templates.ItemsSource = pigtailGestureRecognizer.LearningMachine.Paths;
        }
       
        void LoadCheckGestureDetector()
        {
            Stream recordStream = File.Open(checkKBPath, FileMode.OpenOrCreate);
            checkGestureRecognizer = new TemplatedGestureDetector("Check", recordStream);
            checkGestureRecognizer.TraceTo(gesturesCanvas, Colors.Green);
            checkGestureRecognizer.OnGestureDetected += OnGestureDetected;
            
            //templates.ItemsSource = checkGestureRecognizer.LearningMachine.Paths;
            
        }

        void LoadStarGestureDetector()
        {
            Stream recordStream = File.Open(starKBPath, FileMode.OpenOrCreate);
            starGestureRecognizer = new TemplatedGestureDetector("Star", recordStream);
            starGestureRecognizer.TraceTo(gesturesCanvas, Colors.Blue);
            starGestureRecognizer.OnGestureDetected += OnGestureDetected;

            //templates.ItemsSource = starGestureRecognizer.LearningMachine.Paths;

        }

        private void recordCircle_Click(object sender, RoutedEventArgs e)
        {
            if (circleGestureRecognizer.IsRecordingPath)
            {
                circleGestureRecognizer.EndRecordTemplate();
                recordCircle.Content = "Record new Circle";
                return;
            }

            circleGestureRecognizer.StartRecordTemplate();
            recordCircle.Content = "Stop Recording";
        }

        void OnGestureDetected(string gesture)
        {
            //int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            //detectedGestures.SelectedIndex = pos;
            if (detectedGestures.Items.Count > 6)
            {
                detectedGestures.Items.RemoveAt(0);
            }

            detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));
            
   
            //presentation: if the gesture is swipe to right, then first set the current image to be hidden, then next image
            //to be visible

            if (gesture.Equals("SwipeToRight") && currentPresentingImageIndex < 4)
            {
                presentationImages[currentPresentingImageIndex].Visibility = Visibility.Hidden;
                presentationImages[currentPresentingImageIndex + 1].Visibility = Visibility.Visible;
                currentPresentingImageIndex++;

                
            }

            else if (gesture.Equals("SwipeToLeft") && currentPresentingImageIndex > 0)
            {
                presentationImages[currentPresentingImageIndex].Visibility = Visibility.Hidden;
                presentationImages[currentPresentingImageIndex - 1].Visibility = Visibility.Visible;
                currentPresentingImageIndex--;
            }

            else if (gesture.Equals("Circle"))
            {
                //presentation: current movie states; 0 for first one not activated, 1 for first one is activated, 
                //2 for first one is done, 3 for second one is activated

                if (currentMovieIndex == 0)
                {
                    currentMovieIndex++;
                    presentationVideo1.Play();
                   
                }
                
                else if (currentMovieIndex == 1)
                {
                    currentMovieIndex--;
                    presentationVideo1.Stop();
                    presentationVideo1.Visibility = Visibility.Hidden;
                }

                //else if (currentMovieIndex == 2)
                //{
                //    currentMovieIndex++;
                //    presentationVideo2.Play();

                //}

                //else if (currentMovieIndex == 3)
                //{
                //    presentationVideo2.Stop();
                //    presentationVideo2.Visibility = Visibility.Hidden;

                //}
            }

            else
            {
                //presentationImages[currentPresentingImageIndex].Stretch = Stretch.UniformToFill;
            }
        }

        void CloseGestureDetector()
        {
            Stream recordStream = File.Create(circleKBPath);
            circleGestureRecognizer.SaveState(recordStream);

            //rectangle
            Stream rectangleRecordStream = File.Create(rectangleKBPath);
            rectangleGestureRecognizer.SaveState(rectangleRecordStream);

            //new gesture Step8: save the stream
            Stream pigtailRecordStream = File.Create(pigtailKBPath);
            pigtailGestureRecognizer.SaveState(pigtailRecordStream);

            Stream checkRecordStream = File.Create(checkKBPath);
            checkGestureRecognizer.SaveState(checkRecordStream);

            Stream starRecordStream = File.Create(starKBPath);
            starGestureRecognizer.SaveState(starRecordStream);
        }
    }
}
