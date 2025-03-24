using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Navigation;
using talk2.Repositories;


// using System.Windows.Navigation;

using talk2.Services;
// using talk2.Stores;
using talk2.ViewModels;
using talk2.Views;

namespace talk2;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public new static App Current => (App)Application.Current;

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IChatService, ChatService>();

        services.AddSingleton<IUserRepository, UserRepository>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<UserViewModel>();
        services.AddSingleton<ChatViewModel>();
        services.AddSingleton<RoomViewModel>();
        services.AddSingleton<SettingViewModel>();

        services.AddSingleton(s => new MainView()
        {
            DataContext = s.GetRequiredService<MainViewModel>()
        });

        return services.BuildServiceProvider();
    }

    public App()
    {
        Services = ConfigureServices();

        var mainView = Services.GetRequiredService<MainView>();
        mainView.Show();
    }

    public IServiceProvider Services { get; }
}

