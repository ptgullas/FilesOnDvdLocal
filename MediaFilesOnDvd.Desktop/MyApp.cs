using System.Reflection;

namespace MediaFilesOnDvd.Desktop
{
    public partial class MyApp : Microsoft.Maui.Controls.Application
    {
        //private static readonly Color DotNetPurple = Color.Parse("#512BD4");

        public MyApp()
        {
            Resources.Add("Primary", Color.Parse("#512BD4"));
            Resources.Add(nameof(VersionTemplate), new ControlTemplate(typeof(VersionTemplate)));
        }

        public static string MauiVersion
        {
            get
            {
                var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
                return $".NET MAUI ver. {version[..version.IndexOf('+')]}";
            }
        }
    }
}
