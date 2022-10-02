using Avalonia;
using System;
using Avalonia.Svg.Skia;

namespace LoudnessMeter.Desktop
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        //{
        //    GC.KeepAlive(typeof(SvgImageExtension).Assembly);
        //    GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

        //    return AppBuilder.Configure<App>()
        //        .UsePlatformDetect()
        //        .LogToTrace();
        //}
    }
}
