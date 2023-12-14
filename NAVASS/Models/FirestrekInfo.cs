using CommunityToolkit.Mvvm.ComponentModel;

namespace NAVASS.Models;

public class FirestrekInfo : ObservableObject
{
	public void Update(double deltaTime, double speed)
	{
		if (IsRunning)
		{
			PassingDistance += speed / 3600 * deltaTime;
		}
	}
	public double Run()
	{
		double courseDeviation = 0;
		if (!IsRunning)
		{
			PassingDistance = 0;
		}
		else
		{
			courseDeviation = PassingDistance - PlannedPassingDistance;
		}
		IsRunning = !IsRunning;
		StartStopText = IsRunning ? "Stopp" : "Start";
		StartStopImage = IsRunning ? "circle_stop.png" : "circle_play.png";
		return IsStarboardPassing ? courseDeviation * -1 : courseDeviation;
	}

	public void ChooseSide(string arg)
	{
		if (arg == "BB")
		{
			IsStarboardPassing = false;
			BBBorderColor = Colors.IndianRed;
			SBBorderColor = Colors.Grey;
		}
		else
		{
			IsStarboardPassing = true;
			BBBorderColor = Colors.Grey;
			SBBorderColor = Colors.YellowGreen;
		}
	}

	public async Task SetPlannedPassingDistance()
	{
		Application? app = Application.Current;
		if (app is null)
		{
			return;
		}
		Page? page = app.MainPage;
		if (page is not null)
		{
			string result = await page.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", maxLength: 4, keyboard: Keyboard.Numeric);
			PlannedPassingDistance = Convert.ToDouble(result);
		}
	}

	private Color bbBorderColor = Colors.IndianRed;
	private Color sbBorderColor = Colors.Grey;
	private ImageSource startStopImage = "circle_play.png";
	private bool isStarboardPassing = true;
	private bool isRunning = false;
	private string startStopText = "Start";
	private double passingDistance = 0;
	private double plannedPassingDistance = 0;

	public bool IsNotRunning => !IsRunning;
	public bool IsRunning
	{
		get => isRunning;
		set
		{
			isRunning = value;
			OnPropertyChanged(nameof(IsRunning));
			OnPropertyChanged(nameof(IsNotRunning));
		}
	}
	public string StartStopText
	{
		get => startStopText;
		private set
		{
			startStopText = value;
			OnPropertyChanged(nameof(StartStopText));
		}
	}
	public bool IsStarboardPassing
	{
		get => isStarboardPassing;
		set
		{
			isStarboardPassing = value;
			OnPropertyChanged(nameof(IsStarboardPassing));
		}
	}
	public double PassingDistance
	{
		get => passingDistance;
		set
		{
			passingDistance = value;
			OnPropertyChanged(nameof(PassingDistance));
		}
	}
	public double PlannedPassingDistance
	{
		get => plannedPassingDistance;
		set
		{
			plannedPassingDistance = value;
			OnPropertyChanged(nameof(PlannedPassingDistance));
		}
	}

	public ImageSource StartStopImage
	{
		get => startStopImage;
		private set
		{
			startStopImage = value;
			OnPropertyChanged(nameof(StartStopImage));
		}
	}
	public Color BBBorderColor { get => bbBorderColor; set { bbBorderColor = value; OnPropertyChanged(nameof(BBBorderColor)); } }
	public Color SBBorderColor { get => sbBorderColor; set { sbBorderColor = value; OnPropertyChanged(nameof(SBBorderColor)); } }
}
