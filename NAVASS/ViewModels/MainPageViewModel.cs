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
		//Mockup data for testing
		Navigation.DistanceTotal = 20;
		Navigation.DistanceRemaining = 20;
		Navigation.Speed = 12;
		Navigation.PlannedCourse = 36;

		updateLoopTimer = new System.Timers.Timer(200);
		updateLoopTimer.Elapsed += Update;
		updateLoopTimer.Start();
	}
	
	private void Update(object? sender, ElapsedEventArgs e)
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
			Navigation.Update(deltaTime.TotalSeconds);
		}
		if (Firestrek.IsRunning)
		{
			Firestrek.Update(deltaTime.TotalSeconds, Navigation.Speed);
		}
		if (Halvstrek.IsRunning)
		{
			Halvstrek.Update(deltaTime.TotalSeconds, Navigation);
		}
	}

	[ObservableProperty]
	HalvstrekInfo halvstrek = new ();

	[ObservableProperty]
	FirestrekInfo firestrek = new ();

	[ObservableProperty]
	NavigationalInfo navigation = new ();

	[RelayCommand]
	async Task SetPlannedPassingDistance() => await Firestrek.SetPlannedPassingDistance();

	[RelayCommand]
	void ChoosePortPassing() => Firestrek.ChoosePortPassing();

	[RelayCommand]
	void ChooseStarboardPassing() => Firestrek.ChooseStarboardPassing();

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
	void RunHalvstrek() => Halvstrek.Run();

	[RelayCommand]
	async Task ChooseHalvstrekDegrees() => await Halvstrek.ChooseDegrees();

	[RelayCommand]
	void ChoosePortHalvstrek() => Halvstrek.ChoosePort();

	[RelayCommand]
	void ChooseStarboardHalvstrek() => Halvstrek.ChooseStarboard();

	[RelayCommand]
	async Task RunBeholdenFart() => await Navigation.RunBeholdenFart();

	[RelayCommand]
	async Task SetBeholdenFart() => await Navigation.SetBeholdenFart();
}