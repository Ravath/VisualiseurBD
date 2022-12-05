using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace VisualiseurBD
{
	internal class Course
	{
		/// <summary>
		/// Task of the Course Timer.
		/// </summary>
		private Task _courseTimer;

		/// <summary>
		/// Timer Thread manipulation flag
		/// </summary>
		private bool _courseContinue = true;

		/// <summary>
		/// Display of the timer.
		/// </summary>
		private TextBlock _timerDisplay;

		/// <summary>
		/// Default Course steps
		/// </summary>
		private List<Tuple<int, int>> _courseSteps = new List<Tuple<int, int>>
		{
            // Duration // Number of occurences
            Tuple.Create(30,10),    //30s * 10
            Tuple.Create(60,5),     // 1m *  5
            Tuple.Create(300,2),    // 5m *  2
            Tuple.Create(600,1)     //10m *  1
        };

		/// <summary>
		/// The main window display.
		/// </summary>
		private MainWindow _main;

		public Course(MainWindow mainWindow, TextBlock timerText)
		{
			this._timerDisplay = timerText;
			this._main = mainWindow;
		}

		/// <summary>
		/// Load a csv file to configure the course timers.
		/// </summary>
		/// <param name="configFile"></param>
		public void LoadFile(FileInfo configFile)
		{
			using (var reader = new StreamReader(configFile.FullName))
			{
				_courseSteps.Clear();
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(';');

					int val1 = int.Parse(values[0]);
					int val2 = int.Parse(values[1]);

					_courseSteps.Add(Tuple.Create(val1, val2));
				}
			}
		}

		public void StartCourse()
		{
			if (_courseTimer?.Status == TaskStatus.Running)
			{
				// Stop Course
				_courseContinue = false;
				_timerDisplay.Visibility = Visibility.Hidden;
			}
			else
			{
				// Start Course
				_courseContinue = true;
				_timerDisplay.Visibility = Visibility.Visible;
				_courseTimer = Task.Run(() => CourseTiming());
			}
		}

		private void CourseTiming()
		{
			foreach (var step in _courseSteps)
			{
				int stepTime = step.Item1;
				int stepOccurence = step.Item2;
				for (int j = stepOccurence; j > 0 && _courseContinue; j--)
				{
					for (int i = stepTime; i > 0 && _courseContinue; i--)
					{
						// Update Displayed Timer
						_timerDisplay.Dispatcher.Invoke(() =>
						{
							_timerDisplay.Text = string.Format("{0} / {1} - {2}", i, stepTime, j);
						});

						// 1 second timer
						_courseTimer.Wait(1000);
					}

					// Next picture
					if (_courseContinue)
						_main.Next(true);
				}

				if (!_courseContinue)
					break;
			}

			// END

			// Hide Displayed Timer
			_timerDisplay.Dispatcher.Invoke(() => { _timerDisplay.Visibility = Visibility.Hidden; });
		}
	}
}
