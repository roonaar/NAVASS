using CommunityToolkit.Mvvm.ComponentModel;

namespace NAVASS.Models;

public class HalvstrekInfo : ObservableObject
{
	private bool isRunning = false;
	private string startStopText = "Start";
	private double degrees = 6;
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
