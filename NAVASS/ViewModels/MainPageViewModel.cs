using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Timers;
using System.Windows.Input;

using NAVASS.Models;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject, INotifyPropertyChanged
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
	HalvstrekInfo halvstrek = new();

	[ObservableProperty]
	FirestrekInfo firestrek;

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

		Firestrek = new FirestrekInfo();

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
		ChooseFirestrekSideCommand = new RelayCommand<string>(
			execute: (string? arg) =>
			{
				if (arg is null)
				{
					return;
				}
				Firestrek.ChooseSide(arg);
				UpdateFirestrekButtons();
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


	//Code for firestrek functionality

	[RelayCommand]
	async Task SetPlannedPassingDistance() => await Firestrek.SetPlannedPassingDistance();

	[ObservableProperty]
	string firestrekStartStopText = "Start";
	[ObservableProperty]
	ImageSource firestrekStartStopImage = "circle_play.png";
	[RelayCommand]
	void RunFirestrek()
	{
		double result = Firestrek.Run();
		if (result != 0)
		{
			Navigation.CourseDeviation = result;
		}
		FirestrekStartStopText = Firestrek.IsRunning ? "Stopp" : "Start";
		FirestrekStartStopImage = Firestrek.IsRunning ? "circle_stop.png" : "circle_play.png";
	}

	public ICommand ChooseFirestrekSideCommand { private set; get; }
	[ObservableProperty]
	Color firestrekBBButtonColor = Colors.Grey;
	[ObservableProperty]
	Color firestrekSBButtonColor = Colors.YellowGreen;

	void UpdateFirestrekButtons()
	{
		if (Firestrek.IsStarboardPassing) //Starboard
		{
			FirestrekSBButtonColor = Colors.YellowGreen;
			FirestrekBBButtonColor = Colors.Grey;
		}
		else //Port
		{
			FirestrekSBButtonColor = Colors.Grey;
			FirestrekBBButtonColor = Colors.IndianRed;
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