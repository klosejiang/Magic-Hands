using System;
using System.IO;


namespace GesturesViewer
{
    partial class MainWindow
    {
        void StartVoiceCommander()
        {
            voiceCommander.Start();
        }

        void voiceCommander_OrderDetected(string order)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (audioControl.IsChecked == false)
                    return;

                switch (order)
                {
                    case "record":
                        DirectRecord(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "kinectRecord" + Guid.NewGuid() + ".replay"));
                        break;

                    case "stop":
                        StopRecord();
                        break;


                    case "delete circle":
                        circleGestureRecognizer.DeleteLastTemplate();
                        //rectangleGestureRecognizer.DeleteLastTemplate();
                        templates.UpdateLayout();
                        break;

                    case "delete rectangle":
                        rectangleGestureRecognizer.DeleteLastTemplate();
                        //rectangleGestureRecognizer.DeleteLastTemplate();
                        templates.UpdateLayout();
                        break;

                    //new gesture Step11: delete template
                    case "delete pigtail":
                        pigtailGestureRecognizer.DeleteLastTemplate();
                        templates.UpdateLayout();
                        break;

                    case "delete flashstar":
                        starGestureRecognizer.DeleteLastTemplate();
                        templates.UpdateLayout();
                        break;



                    case "delete check":
                        checkGestureRecognizer.DeleteLastTemplate();
                        templates.UpdateLayout();
                        break;

                    case "end":
                        if (circleGestureRecognizer.IsRecordingPath)
                        {
                            circleGestureRecognizer.EndRecordTemplate();
                            
                            
                        }
                        else if (rectangleGestureRecognizer.IsRecordingPath)
                        {
                            rectangleGestureRecognizer.EndRecordTemplate();
                            
                        }

                        //new gesuture Step9: end recording
                        else if (pigtailGestureRecognizer.IsRecordingPath)
                        {
                            pigtailGestureRecognizer.EndRecordTemplate();
                        }

                        else if (checkGestureRecognizer.IsRecordingPath)
                        {
                            checkGestureRecognizer.EndRecordTemplate();
                        }

                        else if (starGestureRecognizer.IsRecordingPath)
                        {
                            starGestureRecognizer.EndRecordTemplate();
                        }

                        templates.UpdateLayout();
                        recordCircle.Content = "Record new Gesture";
                        break;

                    case "circle":
                        circleGestureRecognizer.StartRecordTemplate();
                        recordCircle.Content = "End Recording";
                        break;

                    case "rectangle":
                        rectangleGestureRecognizer.StartRecordTemplate();
                        recordCircle.Content = "End Recording";
                        break;

                    //new gesture Step10: add new voice commander for recording
                    case "pigtail":     
                        pigtailGestureRecognizer.StartRecordTemplate();
                        recordCircle.Content = "End Recording";
                        break;

                    case "check":
                        checkGestureRecognizer.StartRecordTemplate();
                        recordCircle.Content = "End Recording";
                        break;

                    case "flashstar":
                        starGestureRecognizer.StartRecordTemplate();
                        recordCircle.Content = "End Recording";
                        break;
                    /*remember to change the voice commander instance*/

                    //working
                    //case "start":
                    //    if (circleGestureRecognizer.IsRecordingPath)
                    //    {
                    //        circleGestureRecognizer.EndRecordTemplate();
                    //        recordCircle.Content = "Record new Circle";
                    //         break;
                    //    }

                    //    circleGestureRecognizer.StartRecordTemplate();
                    //    recordCircle.Content = "Stop Recording";
                    //    break;

                }
            }));
        }
    }
}
