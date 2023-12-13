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

		if (IsRunning)
		{
			courseDeviation = PassingDistance - PlannedPassingDistance;
			bool tooLong = PassingDistance > PlannedPassingDistance;

			if (PassingSideText == "SB")
			{
				courseDeviation *= -1;
			}
		}
		IsRunning = !IsRunning;
		StartStopText = IsRunning ? "Stopp" : "Start";
		return courseDeviation;
	}
	public void ChooseStarboardPassing()
	{
		PassingSideText = "SB";
		PassingSideTextColor = Colors.YellowGreen;

	}
	public void ChoosePortPassing()
	{
		PassingSideText = "BB";
		PassingSideTextColor = Colors.IndianRed;
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

	private bool isRunning = false;
	private string startStopText = "Start";
	private string passingSideText = "";
	private Color passingSideTextColor = Colors.Transparent;

	private double passingDistance = 0;
	private double plannedPassingDistance = 0;
	public bool IsRunning { 
		get => isRunning;
		set 
		{ 
			isRunning = value;
			OnPropertyChanged(nameof(IsRunning));
		} }
	public string StartStopText
	{
		get => startStopText;
		private set
		{
			startStopText = value;
			OnPropertyChanged(nameof(StartStopText));
		}
	}
	public string PassingSideText {
		get => passingSideText;
		private set
		{
			passingSideText = value;
			OnPropertyChanged(nameof(PassingSideText));
		}
	}
	public Color PassingSideTextColor { 
		get => passingSideTextColor;
		private set 
		{
			passingSideTextColor = value;
			OnPropertyChanged(nameof(PassingSideTextColor));
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
}
