/// <summary>
/// Date: 14-Apr-2024
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Khoa Minh Ngo and Duke Nguyen - This work may not
///            be copied for use in Academic Coursework.
/// 
/// We, Khoa Minh Ngo and Duke Nguyen, certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source. All references used in the completion 
/// of the assignments are cited in my README file
/// 
/// File Contents
/// 
///     This class provides a static method for creating a MauiApp instance.
/// </summary>

using Logger;
using Microsoft.Extensions.Logging;

namespace ClientGUI
{
    /// <summary>
    /// Provides a static method for creating a MauiApp instance.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures a MauiApp instance.
        /// </summary>
        /// <returns>The configured MauiApp instance.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })

                .Services.AddLogging(configure =>
                {
                    configure.AddDebug();
                    configure.AddProvider(new CustomFileLoggerProvider());
                    configure.SetMinimumLevel(LogLevel.Debug);
                })
                .AddTransient<MainPage>();


            return builder.Build();
        }
    }
}
