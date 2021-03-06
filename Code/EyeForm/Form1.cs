﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EyeForm
{
    public partial class Form1 : Form
    {
        public CascadeClassifier _face = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_default.xml");//Our face detection method 
        public CascadeClassifier _eyes = new CascadeClassifier(Application.StartupPath + "/haarcascade_eye.xml");//Our face detection method 

        Classifier_Train _recognition = new Classifier_Train();

        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        VideoCapture grabber; //This is our capture variable
        public Form1()
        {
            InitializeComponent();
            //InitBrowser();
            //Global.ShowWaitForm(this);
            initialise_capture();
        }
       // public ChromiumWebBrowser browser;



        //public void InitBrowser()
        //{
        //    Cef.Initialize(new CefSettings());
        //    string urlBrowser = Global.SEVER_VPS_URL + "/" + Global.WATING_URL + "/";
        //    browser = new ChromiumWebBrowser(urlBrowser);
        //    browser.Dock = DockStyle.Fill;
        //    browser.Show();
        //    this.Controls.Add(browser);
        //}

        private void ToeicExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }



        private void stop_capture()
        {
            //if (parrellelToolStripMenuItem.Checked)
            //{
            //    Application.Idle -= new EventHandler(FrameGrabber_Parrellel);
            //}
            //else
            //{
            Application.Idle -= new EventHandler(FrameGrabber_Standard);
            //}
            if (grabber != null)
            {
                grabber.Dispose();
            }
        }

        //Camera Start Stop
        public void initialise_capture()
        {
            grabber = new VideoCapture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            //if (parrellelToolStripMenuItem.Checked)
            //{
            //    Application.Idle += new EventHandler(FrameGrabber_Parrellel);
            //}
            //else
            //{
            Application.Idle += new EventHandler(FrameGrabber_Standard);
            //}
        }
        //Process Frame
        void FrameGrabber_Standard(object sender, EventArgs e)
        {
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().ToImage<Bgr, byte>().Resize(320, 240, Emgu.CV.CvEnum.Inter.Cubic);

            //Convert it to Grayscale
            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();
                lbCountEyes.Text = "0";
                lbCountFace.Text = "0";
                //Face Detector
                Rectangle[] facesDetected = _face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);
                Rectangle[] eyesDetected = _eyes.DetectMultiScale(gray_frame, 1.2, 10, new Size(10, 10), Size.Empty);

                //Action for each element detected
                Parallel.For(0, facesDetected.Length, i =>
                {
                    try
                    {
                        //facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                        //facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                        //facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                        //facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                        //Rectangle[] R = 
                        //int scale = 2;
                        //Point pt1 = new Point(), pt2 = new Point();
                        //pt1.X = facesDetected[i].X * scale;
                        //pt2.X = (facesDetected[i].X + facesDetected[i].Width) * scale;

                        //pt1.Y = gray_frame.Height - facesDetected[i].Y* scale;
                        //pt2.Y = gray_frame.Height - (facesDetected[i].Y + facesDetected[i].Height) * scale;

                        //pt1.Y = facesDetected[i].Y * scale;
                        //pt2.Y = (facesDetected[i].Y + facesDetected[i].Height) * scale;

                        //facesDetected[i].X = (pt1.X + pt2.X);
                        //facesDetected[i].Y = (pt2.X - pt1.X);
                        //facesDetected[i].Width = pt2.X - pt1.X;
                        //facesDetected[i].Height = pt1.Y - pt2.Y;



                        result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                        result._EqualizeHist();
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);
                        lbCountFace.Text = facesDetected.Length.ToString();



                        //if (_recognition.IsTrained)
                        //{
                        //    string name = _recognition.Recognise(result);
                        //    int match_value = (int)_recognition.Get_Fisher_Distance;

                        //    //Draw the label for each face detected and recognized
                        //    currentFrame.Draw(name + " ", new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), Emgu.CV.CvEnum.FontFace.HersheyComplex, 1, new Bgr(Color.LightGreen));
                        //    ADD_Face_Found(result, name, match_value);
                        //}

                    }
                    catch
                    {
                        //do nothing as parrellel loop buggy
                        //No action as the error is useless, it is simply an error in 
                        //no data being there to process and this occurss sporadically 
                    }
                });
                lbXRightEye.Text = "0";
                lbYRightEye.Text = "0";
                lbXLeftEye.Text = "0";
                lbYLeftEye.Text = "0";
                Parallel.For(0, eyesDetected.Length, i =>
                {
                    try
                    {
                        //facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                        //facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                        //facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                        //facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                        //result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                        //result._EqualizeHist();
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(eyesDetected[i], new Bgr(Color.Red), 2);
                        lbCountEyes.Text = eyesDetected.Length.ToString();
                        if (eyesDetected.Length > 0)
                        {
                            if (i == 0)
                            {
                                int x_left = 0;
                                int y_left = 0;
                                int width_left = (int)((float)(eyesDetected[i].Width / 2.0));
                                int height_left = (int)((float)(eyesDetected[i].Height));
                                lbXLeftEye.Text = eyesDetected[i].X.ToString();
                                lbYLeftEye.Text = eyesDetected[i].Y.ToString();
                                //eyesDetected[i].
                            }
                            if (i == 1)
                            {
                                int x_right = 0;
                                int y_right = 0;
                                int width_right = (int)((float)(eyesDetected[i].Width / 2.0));
                                int height_right = (int)((float)(eyesDetected[i].Height));
                                lbXRightEye.Text = eyesDetected[i].X.ToString();
                                lbYRightEye.Text = eyesDetected[i].Y.ToString();
                            }

                        }
                        if (_recognition.IsTrained)
                        {
                            //string name = _recognition.Recognise(result);
                            //int match_value = (int)_recognition.Get_Fisher_Distance;

                            ////Draw the label for each face detected and recognized
                            //currentFrame.Draw(name + " ", new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), Emgu.CV.CvEnum.FontFace.HersheyComplex, 1, new Bgr(Color.LightGreen));
                            //ADD_Face_Found(result, name, match_value);
                        }

                    }
                    catch
                    {
                        //do nothing as parrellel loop buggy
                        //No action as the error is useless, it is simply an error in 
                        //no data being there to process and this occurss sporadically 
                    }
                });


                //};
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
            }
        }
        //ADD Picture box and label to a panel for each face
        int faces_count = 0;
        int faces_panel_Y = 0;
        int faces_panel_X = 0;
        void Clear_Faces_Found()
        {
            faces_count = 0;
            faces_panel_Y = 0;
            faces_panel_X = 0;
        }
        void ADD_Face_Found(Image<Gray, Byte> img_found, string name_person, int match_value)
        {
            PictureBox PI = new PictureBox();
            PI.Location = new Point(faces_panel_X, faces_panel_Y);
            PI.Height = 80;
            PI.Width = 80;
            PI.SizeMode = PictureBoxSizeMode.StretchImage;
            PI.Image = img_found.ToBitmap();
            Label LB = new Label();
            LB.Text = name_person + " " + match_value.ToString();
            LB.Location = new Point(faces_panel_X, faces_panel_Y + 80);
            //LB.Width = 80;
            LB.Height = 15;

            //this.Faces_Found_Panel.Controls.Add(PI);
            //this.Faces_Found_Panel.Controls.Add(LB);
            faces_count++;
            if (faces_count == 2)
            {
                faces_panel_X = 0;
                faces_panel_Y += 100;
                faces_count = 0;
            }
            else faces_panel_X += 85;

            //if (Faces_Found_Panel.Controls.Count > 10)
            //{
            //    Clear_Faces_Found();
            //}

        }

        //cv::Rect getLeftmostEye(Rectangle[]  eyes)
        //{
        //    int leftmost = 99999999;
        //    int leftmostIndex = -1;
        //    for (int i = 0; i < eyes.Length; i++)
        //    {
        //        if (eyes[i].Top. < leftmost)
        //        {
        //            leftmost = eyes[i].tl().x;
        //            leftmostIndex = i;
        //        }
        //    }
        //    return eyes[leftmostIndex];
        //}
    }
}
