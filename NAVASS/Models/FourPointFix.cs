using CommunityToolkit.Mvvm.ComponentModel;

namespace NAVASS.Models;

public class FourPointFix: ObservableObject
{
	public void Update(double deltaTime, double speed)
	{
		if (IsRunning)
		{
			PassingDistance += speed / 3600 * deltaTime;
		}
	}
	public double Run()
	{
		double courseDeviation = 0;
		if (!IsRunning)
		{
			PassingDistance = 0;
		}
		else
		{
			courseDeviation = PassingDistance - PlannedPassingDistance;
		}
		IsRunning = !IsRunning;
		return IsStarboardPassing ? courseDeviation * -1 : courseDeviation;
	}

	public void ChooseSide(string arg)
	{
		if (arg == "PS")
		{
			IsStarboardPassing = false;
		}
		else
		{
			IsStarboardPassing = true;
		}
	}

	public async Task SetPlannedPassingDistance()
	{
		Application? app = Application.Current;
		if (app is null)
		{
			return;
		}
		Page? page = app.MainPage;
		if (page is not null)
		{
			string result = await page.DisplayPromptAsync("Passeringsavstand", "Sett planlagt passeringsavstand:", maxLength: 4, keyboard: Keyboard.Numeric);
			PlannedPassingDistance = Convert.ToDouble(result);
		}
	}

	private bool isStarboardPassing = true;
	private bool isRunning = false;
	private double passingDistance = 0;
	private double plannedPassingDistance = 0;

	public bool IsNotRunning => !IsRunning;
	public bool IsRunning
	{
		get => isRunning;
		set
		{
			isRunning = value;
			OnPropertyChanged(nameof(IsRunning));
			OnPropertyChanged(nameof(IsNotRunning));
		}
	}
	public bool IsStarboardPassing
	{
		get => isStarboardPassing;
		set
		{
			isStarboardPassing = value;
			OnPropertyChanged(nameof(IsStarboardPassing));
		}
	}
	public double PassingDistance
	{
		get => passingDistance;
		set
		{
			passingDistance = value;
			OnPropertyChanged(nameof(PassingDistance));
		}
	}
	public double PlannedPassingDistance
	{
		get => plannedPassingDistance;
		set
		{
			plannedPassingDistance = value;
			OnPropertyChanged(nameof(PlannedPassingDistance));
		}
	}
}
