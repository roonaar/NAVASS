using Microsoft.Extensions.Logging;
using NAVASS.ViewModels;
using NAVASS.Views;

namespace NAVASS;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		var services = builder.Services;
		services.AddSingleton<MainPageViewModel>();
		services.AddSingleton<MainPage>();

		return builder.Build();
	}
}
