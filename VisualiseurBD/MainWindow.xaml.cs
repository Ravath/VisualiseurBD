﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualiseurBD {

	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public enum PictureSizeAdjust
        {
            // Try to fit the Height of the picture to the height of the screen.
            Height,
            // Use the natural width of the picture.
            Width,
            // Adjust size of the window to the size of the screen.
            FullScreen
        };

        /// <summary>
        /// Dafault Randomizer.
        /// </summary>
        static Random _rand = new Random();

        /// <summary>
        /// Course manager.
        /// Browses images with a timer for the purpose of drawing training.
        /// </summary>
        private Course _course;

        /// <summary>
        /// The pictures in the same folder than the current displayed picture.
        /// </summary>
        private List<string> _picts = new List<string>();
        /// <summary>
        /// Index of the current picture in the picture list.
        /// </summary>
		private int _currentPict = 0;
        /// <summary>
        /// The current size adjustement method.
        /// </summary>
        private PictureSizeAdjust _currentSizeAdjust = PictureSizeAdjust.Width;
        /// <summary>
        /// When true, adjust the picture to the screen when displayed.
        /// </summary>
        private bool _autoAdjust = false;

        public MainWindow() {
			InitializeComponent();
			this.Width = SystemParameters.MaximizedPrimaryScreenWidth / 2;
			this.Height = SystemParameters.MaximizedPrimaryScreenHeight;
			this.Left = -SystemParameters.ResizeFrameVerticalBorderWidth;
			this.Top = 0;
            this._course = new Course(this, xTimer);

            // Load Course Config File
            FileInfo exec = new FileInfo(Assembly.GetEntryAssembly().Location);
            FileInfo configInfo = new FileInfo(exec.DirectoryName+"/course.csv");
			if (configInfo.Exists)
			    this._course.LoadFile(configInfo);

			// Open files given in argument
			foreach (string arg in Environment.GetCommandLineArgs()) {
				if(arg == null)
					continue;
				if(File.Exists(arg)) {
					if(Compatible(arg)) {
						Open(arg);
						break;
					}
				}
			};
		}

		private void Window_KeyDown( object sender, KeyEventArgs e ) {
			switch(e.Key) {
				case Key.O:
					try { GetFile(); } catch (Exception x) { MessageBox.Show(x.Message); }
					break;
				case Key.Q:
					GoFormer();
					break;
				case Key.D:
					GoNext();
					break;
				case Key.Z:
					xScroll.ScrollToTop();
					break;
				case Key.S:
					xScroll.ScrollToBottom();
					break;
				case Key.Space:
					xScroll.ScrollToVerticalOffset(xScroll.ContentVerticalOffset + SystemParameters.WorkArea.Height / 10);
					break;
				case Key.A://Width adjust
                    SizeAdjust(PictureSizeAdjust.Width);
					break;
				case Key.E://Height adjust
                    SizeAdjust(PictureSizeAdjust.Height);
                    break;
				case Key.F://Full Screen Togle
                    SizeAdjust(PictureSizeAdjust.FullScreen);
					break;
                case Key.R://Random
                    GoRandom();
                    break;
                case Key.C://Start/Stop a 30m drawing course
					_course.StartCourse();
                    break;
				default:
				break;
			}
		}

        #region Navigation
        private void GoNext()
        {
            _currentPict++;
            _currentPict %= _picts.Count;
            ShowPicture(_picts[_currentPict]);
        }

        private void GoFormer()
        {
            _currentPict--;
            _currentPict += _picts.Count;
            _currentPict %= _picts.Count;
            ShowPicture(_picts[_currentPict]);
        }

        private void GoRandom()
        {
            _currentPict = _rand.Next();
            _currentPict %= _picts.Count;
            ShowPicture(_picts[_currentPict]);
		}
		#endregion

		#region File Manipulation
		public void GetFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pictures|*.bmp;*.gif;*.ico;*.jpg;*.png;*.wdp;*.tiff;*.webp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Open(openFileDialog.FileName);
            }
        }

        public void Open(string filePath)
        {
            GetAllPictures(filePath);
            ShowPicture(filePath);
        }

        public void ShowPicture(string filePath)
        {
            //afficher image
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            xImage.Source = bi;
            //trouver index
            _currentPict = _picts.FindIndex(p => p.Equals(filePath));
            RefreshIndex();
            xScroll.ScrollToTop();

            if (_autoAdjust)
            {
                SizeAdjust(_currentSizeAdjust);
            }
        }

        /// <summary>
        /// Enforce the different size adjustements to the picture.
        /// Note : There is no 'zoom'. All is done by changing the window width.
        /// </summary>
        /// <param name="newAdjust"></param>
        private void SizeAdjust(PictureSizeAdjust newAdjust)
        {
            _currentSizeAdjust = newAdjust;

            if (xImage.Source == null)
                return;
            
            switch (_currentSizeAdjust)
            {
                case PictureSizeAdjust.Height:
                    // Set window width to fit the picture Height.
                    this.Width = xImage.Source.Width * SystemParameters.WorkArea.Height / xImage.Source.Height;
                    // Try to not exceed the screen width
                    if (this.Width > SystemParameters.WorkArea.Width)
                        this.Width = SystemParameters.WorkArea.Width;
                    break;
                case PictureSizeAdjust.Width:
                    this.Width = xImage.Source.Width;
                    // Try to not exceed the screen width
                    if (this.Width > SystemParameters.WorkArea.Width)
                        this.Width = SystemParameters.WorkArea.Width;
                    break;
                case PictureSizeAdjust.FullScreen:
                    // Toogle between miximzed and normal
                    if (this.WindowState == WindowState.Maximized)
                    {
                        this.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        this.WindowState = WindowState.Maximized;
                    }
                    break;
                default:
                    break;
            }
        }

        private void RefreshIndex()
        {
            xIndex.Text = string.Format("{0}/{1}", _currentPict + 1, _picts.Count);
        }

        private void GetAllPictures(string filePath)
        {
            _picts.Clear();
            FileInfo fi = new FileInfo(filePath);
            foreach (var f in fi.Directory.GetFiles())
            {
                if (Compatible(f.FullName))
                {
                    _picts.Add(f.FullName);
                }
            }
        }

        public bool Compatible(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            switch (fi.Extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                case ".ico":
                case ".wdp":
                case ".tiff":
                case ".webp":
                    return true;
                default:
                    return false;
            }
        }

        public void Next(bool forceAutoAdjust)
		{
            if(forceAutoAdjust)
            {
			    bool _prevVal = _autoAdjust;
			    _autoAdjust = true;
			    Dispatcher.Invoke(() => GoNext());
			    _autoAdjust = _prevVal;
            }
            else
			{
				Dispatcher.Invoke(() => GoNext());
			}
		}
        #endregion
    }
}
