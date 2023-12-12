using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Timers;

namespace NAVASS.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private System.Timers.Timer updateLoopTimer;

	public MainPageViewModel()
	{
		updateLoopTimer = new System.Timers.Timer(200);
		updateLoopTimer.Elapsed += Update;
		updateLoopTimer.Start();
	}

	DateTime previousTickAt = DateTime.MinValue;
	TimeSpan deltaTime = TimeSpan.Zero;
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

		if (DistanseTilTurn > 0)
		{
			DistanseTilTurn -= Fart / 3600 * deltaTime.TotalSeconds;
			TidTilTurn = TimeSpan.FromHours(DistanseTilTurn / Fart);
			Fremgang = DistanseTilTurn / DistansePåLeg;
		}
		if (FirestrekRunning)
		{
			Passeringsavstand += Fart / 3600 * deltaTime.TotalSeconds;
		}
		if (HalvstrekRunning)
		{
			double distanseCurrentTick = Fart / 3600 * deltaTime.TotalSeconds * (HalvstrekGrader / 60);
			HalvstrekDist += distanseCurrentTick;
			if (HalvstrekSide == "BB")
			{
				Kursavvik -= distanseCurrentTick;
			}
			else
			{
				Kursavvik += distanseCurrentTick;
			}

			KursavvikSide = Kursavvik > 0 ? "STB" : "BB";
			if (Kursavvik < 0.005 && Kursavvik > -0.005)
			{
				KursavvikSide = "";
			}

			KursavvikSideFarge = KursavvikSide == "BB" ? Colors.IndianRed : KursavvikSide == "STB" ? Colors.YellowGreen : Colors.Transparent;
		}
	}

	[ObservableProperty]
	double distansePåLeg = 5.0;
	[ObservableProperty]
	double fremgang;
	[ObservableProperty]
	double distanseTilTurn = 2.0;
	[ObservableProperty]
	TimeSpan tidTilTurn;
	[ObservableProperty]
	double kurs = 180.0;
	[ObservableProperty]
	double fart = 12.0;
	[ObservableProperty]
	double kursavvik = 0.0;
	[ObservableProperty]
	string kursavvikSide = "";
	[ObservableProperty]
	Color kursavvikSideFarge = Colors.Transparent;
	[ObservableProperty]
	double planlagtPassering = 0.0;
	[ObservableProperty]
	double passeringsavstand = 0.0;
	[ObservableProperty]
	string passeringsside = "BB";
	[ObservableProperty]
	Color passeringssideFarge = Colors.IndianRed;

	[RelayCommand]
	async Task SettPasseringsavstand() => PlanlagtPassering = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", maxLength: 4, keyboard: Keyboard.Numeric));

	[RelayCommand]
	void SettBabordPassering()
	{
		Passeringsside = "BB";
		PasseringssideFarge = Colors.IndianRed;
	}

	[RelayCommand]
	void SettStyrbordPassering()
	{
		Passeringsside = "STB";
		PasseringssideFarge = Colors.YellowGreen;
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

	[ObservableProperty]
	bool firestrekRunning = false;
	[ObservableProperty]
	string firestrekText = "Start";

	[RelayCommand]
	void Firestrek()
	{
		if (!FirestrekRunning)
		{
			Passeringsavstand = 0;
		}
		
		if (FirestrekRunning)
		{
			Kursavvik = Passeringsavstand - PlanlagtPassering;
			bool for_langt = Passeringsavstand > PlanlagtPassering;

			if (Passeringsside == "BB")
			{
				KursavvikSide = for_langt ? "STB" : "BB";
			}
			else
			{
				KursavvikSide = for_langt ? "BB" : "STB";
				Kursavvik *= -1;
			}

			KursavvikSideFarge = KursavvikSide == "BB" ? Colors.IndianRed : KursavvikSide == "STB" ? Colors.YellowGreen : Colors.Transparent;
		}

		FirestrekRunning = !FirestrekRunning;
		FirestrekText = FirestrekRunning ? "Stopp" : "Start";
	}

	[ObservableProperty]
	bool halvstrekRunning = false;
	[ObservableProperty]
	string halvstrekText = "Start";
	[ObservableProperty]
	double halvstrekDist = 0.0;
	[ObservableProperty]
	double halvstrekGrader = 6;
	[ObservableProperty]
	string halvstrekSide = "";
	[ObservableProperty]
	Color halvstrekSideFarge = Colors.Transparent;

	[RelayCommand]
	void Halvstrek()
	{
		if (!HalvstrekRunning)
		{
			HalvstrekDist = 0;
		}

		HalvstrekRunning = !HalvstrekRunning;
		HalvstrekText = HalvstrekRunning ? "Stopp" : "Start";
	}

	[RelayCommand]
	async Task SettAntallGrader() => HalvstrekGrader = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Antall grader", "Sett antall grader:", maxLength: 4, keyboard: Keyboard.Numeric));

	[RelayCommand]
	void SettBabordHalvstrek()
	{
		HalvstrekSide = "BB";
		HalvstrekSideFarge = Colors.IndianRed;
	}

	[RelayCommand]
	void SettStyrbordHalvstrek()
	{
		HalvstrekSide = "STB";
		HalvstrekSideFarge = Colors.YellowGreen;
	}
}