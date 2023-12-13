using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace NAVASS.Models;

public class NavigationalInfo : ObservableObject
{
	public void Update(double deltaTime)
	{
		if (DistanceRemaining > 0)
		{
			DistanceSinceLastUpdate += Speed / 3600 * deltaTime;
			DistanceRemaining = DistanceTotal - DistanceSinceLastUpdate;
			TimeToTurn = TimeSpan.FromHours(DistanceRemaining / Speed);
			Progress = DistanceRemaining / DistanceTotal;
		}
	}
	private DateTime t1 = DateTime.MinValue;
	public async Task RunBeholdenFart()
	{
		if (!BeholdenFartIsRunning)
		{
			t1 = DateTime.Now;
			BeholdenFartIsRunning = true;
		}
		else
		{
			DateTime t2 = DateTime.Now;
			TimeSpan t = t2 - t1;
			double s = t.TotalSeconds;
			Application? app = Application.Current;
			if (app is null)
				return;
			
			Page? page = app.MainPage;
			if (page is not null)
			{
				string result = await page.DisplayPromptAsync("Utseilt distanse", "Registrer utseilt distanse i nautiske mil:", maxLength: 4, keyboard: Keyboard.Numeric);
				double d = Convert.ToDouble(result);

				Speed = d / s * 3600;
				BeholdenFartIsRunning = false;
			}
		}
	}
	private bool beholdenFartIsRunning = false;
	public bool BeholdenFartIsRunning { get => beholdenFartIsRunning; set { beholdenFartIsRunning = value; OnPropertyChanged(nameof(BeholdenFartIsRunning)); } }

	private double plannedCourse = 0;
	private double speed = 0;
	private double distanceTotal = 0;
	private double distanceSinceLastUpdate = 0;
	private double distanceRemaining = 0;
	private TimeSpan timeToTurn = TimeSpan.Zero;
	private double progress = 0;
	private double courseDeviation = 0;
	private string courseDeviationSideText = "";
	private Color courseDeviationSideTextColor = Colors.Transparent;
	public double PlannedCourse { get => plannedCourse; set { plannedCourse = value; OnPropertyChanged(nameof(PlannedCourse)); } }
	public double Speed { get => speed; set { speed = value; OnPropertyChanged(nameof(Speed)); } }
	public double DistanceTotal { get => distanceTotal; set { distanceTotal = value; OnPropertyChanged(nameof(DistanceTotal)); } }
	public double DistanceSinceLastUpdate { get => distanceSinceLastUpdate; set { distanceSinceLastUpdate = value; OnPropertyChanged(nameof(DistanceSinceLastUpdate)); } }
	public double DistanceRemaining { get => distanceRemaining; set { distanceRemaining = value; OnPropertyChanged(nameof(DistanceRemaining)); } }
	public TimeSpan TimeToTurn { get => timeToTurn; set { timeToTurn = value; OnPropertyChanged(nameof(TimeToTurn)); } }
	public double Progress { get => progress; set { progress = value; OnPropertyChanged(nameof(Progress)); } }
	public double CourseDeviation { 
		get => courseDeviation;
		set
		{
			courseDeviation = value;
			OnPropertyChanged(nameof(CourseDeviation));
			CourseDeviationSideText = courseDeviation > 0 ? "SB" : "BB";
			CourseDeviationSideTextColor = courseDeviation > 0 ? Colors.YellowGreen : Colors.IndianRed;
		}
	}
	public string CourseDeviationSideText {	get => courseDeviationSideText;	set	{ courseDeviationSideText = value; OnPropertyChanged(nameof(CourseDeviationSideText)); } }
	public Color CourseDeviationSideTextColor
	{
		get => courseDeviationSideTextColor;
		set
		{
			courseDeviationSideTextColor = value;
			OnPropertyChanged(nameof(CourseDeviationSideTextColor));
		}
	}
}