using CommunityToolkit.Mvvm.ComponentModel;

namespace NAVASS.Models;

public class HalvstrekInfo : ObservableObject
{
	public void Update(double deltaTime, NavigationalInfo navInfo)
	{
		double distanceCurrentTick = navInfo.Speed / 3600 * deltaTime * (Degrees / 60);
		Distance += distanceCurrentTick;
		if (IsStarboard)
		{
			navInfo.CourseDeviation += distanceCurrentTick;
		}
		else
		{
			navInfo.CourseDeviation -= distanceCurrentTick;
		}
	}
	public void Run()
	{
		if (!IsRunning)
		{
			Distance = 0;
		}

		IsRunning = !IsRunning;
	}

	public async Task ChooseDegrees()
	{
		Application? app = Application.Current;
		if (app == null)
		{
			return;
		}
		Page? page = app.MainPage;
		if (page is not null)
		{
			string result = await page.DisplayPromptAsync("Antall grader", "Sett antall grader:", maxLength: 4, keyboard: Keyboard.Numeric);
			Degrees = Convert.ToDouble(result);
		}
	}

	public void ChooseSide(string side)
	{
		if (side == "BB")
		{
			IsStarboard = false;
		}
		else
		{
			IsStarboard = true;
		}
	}

	private bool isStarboard = false;
	private bool isRunning = false;
	private double degrees = 6;
	private double distance = 0;

	public bool IsStarboard { get => isStarboard; set { isStarboard = value; OnPropertyChanged(nameof(IsStarboard)); } }
	public bool IsNotRunning => !IsRunning;
	public bool IsRunning { get => isRunning; set { isRunning = value; OnPropertyChanged(nameof(IsRunning)); OnPropertyChanged(nameof(IsNotRunning)); } }
	public double Degrees { get => degrees; set { degrees = value; OnPropertyChanged(nameof(Degrees)); } }
	public double Distance { get => distance; set { distance = value; OnPropertyChanged(nameof(Distance)); } }
}