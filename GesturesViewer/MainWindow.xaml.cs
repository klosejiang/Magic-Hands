using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;
using Microsoft.Research.Kinect.Nui;
using Kinect.Toolbox.Record;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;


namespace GesturesViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        Runtime kinectRuntime;

        readonly SwipeGestureDetector swipeGestureRecognizer = new SwipeGestureDetector();
        //new gesture Step1: create new Gesture Recognizer 
        TemplatedGestureDetector circleGestureRecognizer, rectangleGestureRecognizer, pigtailGestureRecognizer, checkGestureRecognizer, starGestureRecognizer;
        readonly ColorStreamManager streamManager = new ColorStreamManager();
        SkeletonDisplayManager skeletonDisplayManager;
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();
        readonly AlgorithmicPostureDetector algorithmicPostureRecognizer = new AlgorithmicPostureDetector();
        TemplatedPostureDetector templatePostureDetector;
        bool recordNextFrameForPosture;

        //presentation: create and intitialize the presentation image array
        Image [] presentationImages = new Image [5];
        //presentation: current presenting image
        int currentPresentingImageIndex = 0;
        //presentation: current movie states; 0 for first one not activated, 1 for first one is activated, 2 for first one is done, 3 for second one is activated
        int currentMovieIndex = 0;

        //int presentIndex = 0;


        string circleKBPath;
        //rectangle
        string rectangleKBPath;
        //new gesture Step2: create new path
        string pigtailKBPath;
        string checkKBPath;
        string starKBPath;

        string letterT_KBPath;
        

       

        SkeletonRecorder recorder;
        SkeletonReplay replay;

        BindableNUICamera nuiCamera;

        VoiceCommander voiceCommander;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            circleKBPath = Path.Combine(Environment.CurrentDirectory, @"data\circleKB.save");
            //rectangle
            rectangleKBPath = Path.Combine(Environment.CurrentDirectory, @"data\rectangleKB.save");

            //new gesture Step3: add new path
            pigtailKBPath = Path.Combine(Environment.CurrentDirectory, @"data\pigtailKB.save");
            checkKBPath = Path.Combine(Environment.CurrentDirectory, @"data\checkKB.save");
            starKBPath = Path.Combine(Environment.CurrentDirectory, @"data\starKB.save");

            letterT_KBPath = Path.Combine(Environment.CurrentDirectory, @"data\t_KB.save");

            //presentation: add presentation images to image array
            presentationImages[0] = presentationImage0;
            presentationImages[1] = presentationImage1;
            presentationImages[2] = presentationImage3;
            presentationImages[3] = presentationImage4;
            presentationImages[4] = presentationImage5;
            //presentationImages[5] = presentationImage5;


            try
            {
                kinectRuntime = new Runtime();
                kinectRuntime.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                kinectRuntime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                kinectRuntime.SkeletonFrameReady += kinectRuntime_SkeletonFrameReady;
                kinectRuntime.VideoFrameReady += kinectRuntime_VideoFrameReady;

                swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;

                skeletonDisplayManager = new SkeletonDisplayManager(kinectRuntime.SkeletonEngine, kinectCanvas);

                kinectRuntime.SkeletonEngine.TransformSmooth = true;
                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 1.0f,
                    Correction = 0.1f,
                    Prediction = 0.1f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.05f
                };
                kinectRuntime.SkeletonEngine.SmoothParameters = parameters;

                LoadCircleGestureDetector();
                LoadLetterTPostureDetector();
                //rectangle
                LoadRectangleGestureDetector();

                //new gesture Step4: load new detector();
                LoadPigtailGestureDetector();
                LoadCheckGestureDetector();
                LoadStarGestureDetector();

                nuiCamera = new BindableNUICamera(kinectRuntime.NuiCamera);

                elevationSlider.DataContext = nuiCamera;

                //voiceCommander = new VoiceCommander("record", "stop");

                //new gesture Step5: add new voice commander
                voiceCommander = new VoiceCommander("record", "stop", "circle", "rectangle", "flashstar",
                    "pigtail", "check", "end", "delete circle", "delete rectangle", "delete pigtail", "delete check", "delete flashstar");
                voiceCommander.OrderDetected += voiceCommander_OrderDetected;

                StartVoiceCommander();

                //presentationVideo.Stop();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void kinectRuntime_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            kinectDisplay.Source = streamManager.Update(e);
        }

        void kinectRuntime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (recorder != null)
                recorder.Record(e.SkeletonFrame);

            if (e.SkeletonFrame.Skeletons.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked).Count() == 0)
                return;

            ProcessFrame(e.SkeletonFrame);
        }

        void ProcessFrame(ReplaySkeletonFrame frame)
        {
            Dictionary<int, string> stabilities = new Dictionary<int, string>();
            foreach (var skeleton in frame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                barycenterHelper.Add(skeleton.Position.ToVector3(), skeleton.TrackingID);

                stabilities.Add(skeleton.TrackingID, barycenterHelper.IsStable(skeleton.TrackingID) ? "Stable" : "Unstable");
                if (!barycenterHelper.IsStable(skeleton.TrackingID))
                    continue;

                if (recordNextFrameForPosture)
                {
                    recordNextFrameForPosture = false;
                    templatePostureDetector.AddTemplate(skeleton);
                }

                foreach (Joint joint in skeleton.Joints)
                {
                    if (joint.Position.W < 0.8f || joint.TrackingState != JointTrackingState.Tracked)
                        continue;

                    if (joint.ID == JointID.HandRight)
                    {
                        swipeGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                        circleGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                        //rectangle
                        rectangleGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);

                        //new gesture Step6: feed points to the new recognizer;
                        pigtailGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                        checkGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                        starGestureRecognizer.Add(joint.Position, kinectRuntime.SkeletonEngine);
                    }
                }

                algorithmicPostureRecognizer.TrackPostures(skeleton);
                templatePostureDetector.TrackPostures(skeleton);
            }

            skeletonDisplayManager.Draw(frame);

            stabilitiesList.ItemsSource = stabilities;

            currentPosture.Text = "Current posture: " + algorithmicPostureRecognizer.CurrentPosture.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseGestureDetector();

            ClosePostureDetector();

            voiceCommander.Stop();

            if (recorder != null)
                recorder.Stop();
        }

        private void replayButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };

            if (openFileDialog.ShowDialog() == true)
            {
                if (replay != null)
                {
                    replay.SkeletonFrameReady -= replay_SkeletonFrameReady;
                    replay.Stop();
                }
                Stream recordStream = File.OpenRead(openFileDialog.FileName);

                replay = new SkeletonReplay(recordStream);

                replay.SkeletonFrameReady += replay_SkeletonFrameReady;

                replay.Start();
            }
        }

        void replay_SkeletonFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ProcessFrame(e.SkeletonFrame);
        }

        private void elevationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
