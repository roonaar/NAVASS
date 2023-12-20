using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Timers;
using System.Windows.Input;

using NAVASS.Models;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private void Update(object? sender, ElapsedEventArgs e)
	{
		if (previousTickAt == DateTime.MinValue)
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
		if (FourPointFix.IsRunning)
		{
			FourPointFix.Update(deltaTime.TotalSeconds, Navigation.Speed);
		}
		if (Halvstrek.IsRunning)
		{
			Halvstrek.Update(deltaTime.TotalSeconds, Navigation);
		}
	}
	
	[ObservableProperty]
	HalvstrekInfo halvstrek = new();

	[ObservableProperty]
	FourPointFix fourPointFix;

	[ObservableProperty]
	NavigationalInfo navigation = new();

	private readonly System.Timers.Timer updateLoopTimer;
	DateTime previousTickAt = DateTime.MinValue;
	TimeSpan deltaTime = TimeSpan.Zero;

	public MainPageViewModel()
	{
		//Mockup data for testing
		Navigation.DistanceTotal = 20;
		Navigation.DistanceRemaining = 20;
		Navigation.Speed = 12;
		Navigation.PlannedCourse = 36;

		FourPointFix = new FourPointFix();
		UpdateFourPointFixButtons();
		UpdateHalvstrekButtons();

		updateLoopTimer = new System.Timers.Timer(200);
		updateLoopTimer.Elapsed += Update;
		updateLoopTimer.Start();

		ChooseHalvstrekSideCommand = new RelayCommand<string>(
			execute: (string? arg) =>
			{
				if (arg is null)
				{
					return;
				}
				Halvstrek.ChooseSide(arg);
				UpdateHalvstrekButtons();
			});
		ChooseFourPointFixSideCommand = new RelayCommand<string>(
			execute: (string? arg) =>
			{
				if (arg is null)
				{
					return;
				}
				FourPointFix.ChooseSide(arg);
				UpdateFourPointFixButtons();
			});
	}


	//Code for halvstrek functionality
	[RelayCommand]
	async Task ChooseHalvstrekDegrees() => await Halvstrek.ChooseDegrees();

	[ObservableProperty]
	string halvstrekStartStopText = "Start";
	[ObservableProperty]
	ImageSource halvstrekStartStopImage = "circle_play.png";

	[RelayCommand]
	void RunHalvstrek()
	{
		Halvstrek.Run();
		HalvstrekStartStopText = Halvstrek.IsRunning ? "Stopp" : "Start";
		HalvstrekStartStopImage = Halvstrek.IsRunning ? "circle_stop.png" : "circle_play.png";
	}

	public ICommand ChooseHalvstrekSideCommand { private set; get; }
	[ObservableProperty]
	Color halvstrekBBButtonColor = Colors.Grey;
	[ObservableProperty]
	Color halvstrekSBButtonColor = Colors.YellowGreen;
	void UpdateHalvstrekButtons()
	{
		if (Halvstrek.IsStarboard) //Starboard
		{
			HalvstrekSBButtonColor = Colors.YellowGreen;
			HalvstrekBBButtonColor = Colors.Grey;
		}
		else //Port
		{
			HalvstrekSBButtonColor = Colors.Grey;
			HalvstrekBBButtonColor = Colors.IndianRed;
		}
	}


	//Code for four point fix functionality

	[RelayCommand]
	async Task SetPlannedPassingDistance() => await FourPointFix.SetPlannedPassingDistance();

	[ObservableProperty]
	string fourPointFixStartStopText = "Start";
	[ObservableProperty]
	ImageSource fourPointFixStartStopImage = "circle_play.png";
	[RelayCommand]
	void RunFourPointFix()
	{
		double result = FourPointFix.Run();
		if (result != 0)
		{
			Navigation.CourseDeviation = result;
		}
		FourPointFixStartStopText = FourPointFix.IsRunning ? "Stopp" : "Start";
		FourPointFixStartStopImage = FourPointFix.IsRunning ? "circle_stop.png" : "circle_play.png";
	}

	public ICommand ChooseFourPointFixSideCommand { private set; get; }
	[ObservableProperty]
	Color fourPointFixPSButtonColor = Colors.Grey;
	[ObservableProperty]
	Color fourPointFixSSButtonColor = Colors.YellowGreen;

	void UpdateFourPointFixButtons()
	{
		if (FourPointFix.IsStarboardPassing) //Starboard
		{
			FourPointFixSSButtonColor = Colors.YellowGreen;
			FourPointFixPSButtonColor = Colors.Grey;
		}
		else //Port
		{
			FourPointFixSSButtonColor = Colors.Grey;
			FourPointFixPSButtonColor = Colors.IndianRed;
		}
	}


	//Code for beholden fart functionality

	[RelayCommand]
	async Task RunBeholdenFart()
	{
		await Navigation.RunBeholdenFart();
	}

	[RelayCommand]
	async Task SetBeholdenFart() => await Navigation.SetBeholdenFart();
}