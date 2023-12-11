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

	DateTime previous_tick_at;
	TimeSpan delta_time = TimeSpan.FromSeconds(1);
	private void Update(object sender, ElapsedEventArgs e)
	{
		if(previous_tick_at == DateTime.MinValue)
		{
			previous_tick_at = e.SignalTime;
			return;
		}
		var current_tick_at = e.SignalTime;
		delta_time = current_tick_at - previous_tick_at;
		previous_tick_at = current_tick_at;

		if (Distanse_til_turn > 0)
		{
			Distanse_til_turn -= Fart / 3600 * delta_time.TotalSeconds;
			Tid_til_turn = TimeSpan.FromHours(Distanse_til_turn / Fart);
			Fremgang = Distanse_til_turn / distanse;
		}
		if (firestrek_running)
		{
			Passeringsavstand += Fart / 3600 * delta_time.TotalSeconds;
		}
		if (halvstrek_running)
		{
			double distanse_current_tick = Fart / 3600 * delta_time.TotalSeconds * (HalvstrekGrader / 60);
			Halvstrek_dist += distanse_current_tick;
			if (HalvstrekSide == "BB")
			{
				Kursavvik -= distanse_current_tick;
			}
			else
			{
				Kursavvik += distanse_current_tick;
			}

			Kursavvik_side = Kursavvik > 0 ? "STB" : "BB";
			if (Kursavvik < 0.005 && Kursavvik > -0.005)
			{
				Kursavvik_side = "";
			}

			Kursavvik_side_farge = Kursavvik_side == "BB" ? Colors.IndianRed : Kursavvik_side == "STB" ? Colors.YellowGreen : Colors.Transparent;
		}
	}

	double distanse = 5.0;
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
	double kursavvik = 0.0;
	[ObservableProperty]
	string kursavvik_side = "";
	[ObservableProperty]
	Color kursavvik_side_farge = Colors.Transparent;
	[ObservableProperty]
	double planlagt_passering = 0.0;
	[ObservableProperty]
	double passeringsavstand = 0.0;
	[ObservableProperty]
	string passeringsside = "BB";
	[ObservableProperty]
	Color passeringsside_farge = Colors.IndianRed;

	[RelayCommand]
	async Task SettPasseringsavstand() => Planlagt_passering = Convert.ToDouble(await Application.Current.MainPage.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", maxLength: 4, keyboard: Keyboard.Numeric));

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

	bool firestrek_running = false;
	[ObservableProperty]
	string firestrek_text = "Start";

	[RelayCommand]
	void Firestrek()
	{
		if (!firestrek_running)
		{
			Passeringsavstand = 0;
		}
		
		if (firestrek_running)
		{
			Kursavvik = Passeringsavstand - Planlagt_passering;
			bool for_langt = Passeringsavstand > Planlagt_passering;

			if (Passeringsside == "BB")
			{
				Kursavvik_side = for_langt ? "STB" : "BB";
			}
			else
			{
				Kursavvik_side = for_langt ? "BB" : "STB";
				Kursavvik *= -1;
			}

			Kursavvik_side_farge = Kursavvik_side == "BB" ? Colors.IndianRed : Kursavvik_side == "STB" ? Colors.YellowGreen : Colors.Transparent;
		}

		firestrek_running = !firestrek_running;
		Firestrek_text = firestrek_running ? "Stopp" : "Start";
	}

	bool halvstrek_running = false;
	[ObservableProperty]
	string halvstrek_text = "Start";
	[ObservableProperty]
	double halvstrek_dist = 0.0;
	[ObservableProperty]
	double halvstrekGrader = 6;
	[ObservableProperty]
	string halvstrekSide = "";
	[ObservableProperty]
	Color halvstrekSideFarge = Colors.Transparent;

	[RelayCommand]
	void Halvstrek()
	{
		if (!halvstrek_running)
		{
			Halvstrek_dist = 0;
		}

		halvstrek_running = !halvstrek_running;
		Halvstrek_text = halvstrek_running ? "Stopp" : "Start";
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