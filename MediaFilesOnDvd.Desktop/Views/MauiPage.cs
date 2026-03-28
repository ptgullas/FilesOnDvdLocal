using Microsoft.AspNetCore.Components.WebView.Maui;
//using VijayAnand.MauiBlazor.Markup;

namespace MediaFilesOnDvd.Desktop.Views
{
    public partial class MauiPage : ContentPage
    {
        public MauiPage()
        {
            ControlTemplate = (ControlTemplate)Application.Current!.Resources[nameof(VersionTemplate)];

            // For a much simplified initialization
            // Add reference to the VijayAnand.MauiBlazor.Markup NuGet package
            // dotnet add package VijayAnand.MauiBlazor.Markup
            // Then uncomment the using statement at the top of this file and the line below
            //var bwv = new BlazorWebView().Configure(typeof(Main), "/counter");

            var bwv = new BlazorWebView()
            {
                StartPath = "/counter",
                HostPage = "wwwroot/index.html"
            };

            bwv.RootComponents.Add(new RootComponent()
            {
                Selector = "#app",
                ComponentType = typeof(Main),
                Parameters = null
            });

            Content = bwv;
        }
    }
}
