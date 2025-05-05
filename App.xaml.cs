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
        services.AddSingleton<IFileService, FileService>();

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IChatRepository, ChatRepository>();
        services.AddSingleton<IFileRepository, FileRepository>();

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
        // [중복실행 방지1]
        // string applicationName = Process.GetCurrentProcess().ProcessName;
        // Duplicate_execution(applicationName);

        Services = ConfigureServices();

        var mainView = Services.GetRequiredService<MainView>();
        mainView.Show();
    }

    public IServiceProvider Services { get; }

    // [중복실행 방지1]
    /// <summary>
    /// 중복실행방지
    /// </summary>
    /// <param name="mutexName"></param>
    Mutex mutex = null;
    private void Duplicate_execution(string mutexName)
    {
        try
        {
            mutex = new Mutex(false, mutexName);
        }
        catch (Exception ex)
        {
            Application.Current.Shutdown();
        }
        if (mutex.WaitOne(0, false))
        {
            // InitializeComponent();
        }
        else
        {
            Application.Current.Shutdown();
        }
    }
}

