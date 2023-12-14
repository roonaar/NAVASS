using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAVASS.Models;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject, INotifyPropertyChanged
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

		ChooseHalvstrekSideCommand = new RelayCommand<string>(
			execute: (string? arg) =>
			{
				if (arg is null)
				{
					return;
				}
				Halvstrek.ChooseSide(arg);
			});
		ChooseFirestrekSideCommand = new RelayCommand<string>(
			execute: (string? arg) =>
			{
				if (arg is null)
				{
					return;
				}
				Firestrek.ChooseSide(arg);
			});
	}

	public ICommand ChooseHalvstrekSideCommand { private set; get; }
	public ICommand ChooseFirestrekSideCommand { private set; get; }

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
	FirestrekInfo firestrek = new();

	[ObservableProperty]
	NavigationalInfo navigation = new();

	[RelayCommand]
	async Task SetPlannedPassingDistance() => await Firestrek.SetPlannedPassingDistance();

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
	async Task RunBeholdenFart() => await Navigation.RunBeholdenFart();

	[RelayCommand]
	async Task SetBeholdenFart() => await Navigation.SetBeholdenFart();
}