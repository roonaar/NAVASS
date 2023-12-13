using CommunityToolkit.Mvvm.ComponentModel;

namespace NAVASS.Models;

public class HalvstrekInfo : ObservableObject
{
	public void Update(double deltaTime, NavigationalInfo navInfo)
	{
		double distanceCurrentTick = navInfo.Speed / 3600 * deltaTime * (Degrees / 60);
		Distance += distanceCurrentTick;
		if (side == Side.Port)
		{
			navInfo.CourseDeviation -= distanceCurrentTick;
		}
		else
		{
			navInfo.CourseDeviation += distanceCurrentTick;
		}
	}
	public void Run()
	{
		if (!IsRunning)
		{
			Distance = 0;
		}

		IsRunning = !IsRunning;
		StartStopText = IsRunning ? "Stopp" : "Start";
	}

	public void ChoosePort()
	{
		side = Side.Port;
		SideText = "BB";
		SideTextColor = Colors.IndianRed;
	}

	public void ChooseStarboard()
	{
		side = Side.Starboard;
		SideText = "SB";
		SideTextColor = Colors.YellowGreen;
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

	private enum Side { Port, Starboard };
	private bool isRunning = false;
	private string startStopText = "Start";
	private double degrees = 6;
	private Side side = Side.Port;
	private string sideText = "";
	private Color sideTextColor = Colors.Transparent;
	private double distance = 0;
	public bool IsRunning { get => isRunning; set { isRunning = value; OnPropertyChanged(nameof(IsRunning)); } }
	public string StartStopText { get => startStopText; set { startStopText = value; OnPropertyChanged(nameof(StartStopText)); } }
	public double Degrees { get => degrees; set { degrees = value; OnPropertyChanged(nameof(Degrees)); } }
	public string SideText { get => sideText; set { sideText = value; OnPropertyChanged(nameof(SideText)); } }
	public Color SideTextColor { get => sideTextColor; set { sideTextColor = value; OnPropertyChanged(nameof(SideTextColor)); } }
	public double Distance { get => distance; set { distance = value; OnPropertyChanged(nameof(Distance)); } }
}