using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Timers;

using NAVASS.Models;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private System.Timers.Timer updateLoopTimer;
	DateTime previousTickAt = DateTime.MinValue;
	TimeSpan deltaTime = TimeSpan.Zero;

	public MainPageViewModel()
	{
		Navigation.DistanceTotal = 20;
		Navigation.DistanceRemaining = 20;
		Navigation.Speed = 12;

		updateLoopTimer = new System.Timers.Timer(200);
		updateLoopTimer.Elapsed += Update;
		updateLoopTimer.Start();
	}
	
	private void Update(object sender, ElapsedEventArgs e)
	{
		if(previousTickAt == DateTime.MinValue)
		{
			previousTickAt = e.SignalTime;
			return;
		}
		var currentTickAt = e.SignalTime;
		deltaTime = currentTickAt - previousTickAt;
		previousTickAt = currentTickAt;

		if (Navigation.DistanceRemaining > 0)
		{
			Navigation.DistanceSinceLastUpdate += Navigation.Speed / 3600 * deltaTime.TotalSeconds;
			Navigation.DistanceRemaining = Navigation.DistanceTotal - Navigation.DistanceSinceLastUpdate;
			Navigation.TimeToTurn = TimeSpan.FromHours(Navigation.DistanceRemaining / Navigation.Speed);
			Navigation.Progress = Navigation.DistanceRemaining / Navigation.DistanceTotal;
		}
		if (Firestrek.IsRunning)
		{
			Firestrek.PassingDistance += Navigation.Speed / 3600 * deltaTime.TotalSeconds;
		}
		if (Halvstrek.IsRunning)
		{
			double distanceCurrentTick = Navigation.Speed / 3600 * deltaTime.TotalSeconds * (Halvstrek.Degrees / 60);
			Halvstrek.Distance += distanceCurrentTick;
			if (Halvstrek.SideText == "BB")
			{
				Navigation.CourseDeviation -= distanceCurrentTick;
			}
			else
			{
				Navigation.CourseDeviation += distanceCurrentTick;
			}

			Navigation.CourseDeviationSideText = Navigation.CourseDeviation > 0 ? "STB" : "BB";
			if (Navigation.CourseDeviation < 0.005 && Navigation.CourseDeviation > -0.005)
			{
				Navigation.CourseDeviationSideText = "";
			}
			Navigation.CourseDeviationSideTextColor = Navigation.CourseDeviationSideText == "BB" ? Colors.IndianRed : Navigation.CourseDeviationSideText == "STB" ? Colors.YellowGreen : Colors.Transparent;
		}
	}

	[ObservableProperty]
	HalvstrekInfo halvstrek = new ();
	[ObservableProperty]
	FirestrekInfo firestrek = new ();
	[ObservableProperty]
	NavigationalInfo navigation = new ();

	[RelayCommand]
	async Task SetPlannedPassingDistance() => Firestrek.PlannedPassingDistance = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", maxLength: 4, keyboard: Keyboard.Numeric));

	[RelayCommand]
	void ChoosePortPassing()
	{
		Firestrek.ChoosePortPassing();
	}

	[RelayCommand]
	void ChooseStarboardPassing()
	{
		Firestrek.ChooseStarboardPassing();
	}

	[RelayCommand]
	void RunFirestrek()
	{
		double result = Firestrek.Run();
		if (result != 0)
		{
			Navigation.CourseDeviation = result;
		}
	}

	[RelayCommand]
	void RunHalvstrek()
	{
		if (!Halvstrek.IsRunning)
		{
			Halvstrek.Distance = 0;
		}

		Halvstrek.IsRunning = !Halvstrek.IsRunning;
		Halvstrek.StartStopText = Halvstrek.IsRunning ? "Stopp" : "Start";
	}

	[RelayCommand]
	async Task ChooseDegrees() => Halvstrek.Degrees = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Antall grader", "Sett antall grader:", maxLength: 4, keyboard: Keyboard.Numeric));

	[RelayCommand]
	void ChoosePortHalvstrek()
	{
		Halvstrek.SideText = "BB";
		Halvstrek.SideTextColor = Colors.IndianRed;
	}

	[RelayCommand]
	void ChooseStarboardHalvstrek()
	{
		Halvstrek.SideText = "STB";
		Halvstrek.SideTextColor = Colors.YellowGreen;
	}

	[ObservableProperty]
	bool beholdenFartIsRunning = false;
	DateTime t1;
	[RelayCommand]
	async Task RunBeholdenFart()
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
			double d = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Utseilt distanse", "Registrer utseilt distanse i nautiske mil:", maxLength: 4, keyboard: Keyboard.Numeric));

			Navigation.Speed = d / s * 3600;
			BeholdenFartIsRunning = false;
		}
	}
}