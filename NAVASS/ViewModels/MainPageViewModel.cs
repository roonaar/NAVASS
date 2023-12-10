using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Timers;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private System.Timers.Timer updateLoopTimer;

	public MainPageViewModel()
	{
		updateLoopTimer = new System.Timers.Timer(1000);
		updateLoopTimer.Elapsed += Update;
		updateLoopTimer.Start();
	}
	double distanse = 5.0;
	private void Update(object sender, ElapsedEventArgs e)
	{
		if (Distanse_til_turn > 0)
		{
			Distanse_til_turn -= Fart / 3600;
			Tid_til_turn = TimeSpan.FromHours(Distanse_til_turn / Fart);
			Fremgang = Distanse_til_turn / distanse;
		}
	}
	[ObservableProperty]
	double fremgang;
	[ObservableProperty]
	double distanse_til_turn = 2.0;
	[ObservableProperty]
	TimeSpan tid_til_turn;
	[ObservableProperty]
	double kurs = 180.0;
	[ObservableProperty]
	double fart = 12.0;
	[ObservableProperty]
	double kurssavvik = 0.0;
	[ObservableProperty]
	double planlagt_passering = 0.1;
	[ObservableProperty]
	double passeringsavstand = 0.0;
	[ObservableProperty]
	string passeringsside = "BB";
	[ObservableProperty]
	Color passeringsside_farge = Colors.IndianRed;

	[RelayCommand]
	async Task SettPasseringsavstand() => Planlagt_passering = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", initialValue: Planlagt_passering.ToString(), maxLength: 4, keyboard: Keyboard.Numeric));

	[RelayCommand]
	void SettBabordPassering()
	{
		Passeringsside = "BB";
		Passeringsside_farge = Colors.IndianRed;
	}

	[RelayCommand]
	void SettStyrbordPassering()
	{
		Passeringsside = "STB";
		Passeringsside_farge = Colors.YellowGreen;
	}

	[ObservableProperty]
	bool isRunning = false;
	DateTime t1;
	[RelayCommand]
	async Task BeholdenFart()
	{
		if (!IsRunning)
		{
			t1 = DateTime.Now;
			IsRunning = true;
		}
		else
		{
			DateTime t2 = DateTime.Now;
			TimeSpan t = t2 - t1;
			double s = t.TotalSeconds;
			double d = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Utseilt distanse", "Registrer utseilt distanse i nautiske mil:", maxLength: 4, keyboard: Keyboard.Numeric));

			Fart = d / s * 3600;
			IsRunning = false;
		}
	}
}