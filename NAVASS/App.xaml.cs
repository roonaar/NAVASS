using NAVASS.ViewModels;

namespace NAVASS;

public partial class App : Application
{
	public App(MainPageViewModel vm)
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}