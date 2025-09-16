using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace talk2.Settings
{
    public static class LoginSettingsManager
    {
        private static readonly string configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "talk2" // → 이 이름은 실제 앱 이름으로 바꾸세요
        );

        private static readonly string configPath = Path.Combine(configDir, "settings.json");

        public static void Save(LoginSettings settings)
        {
            Directory.CreateDirectory(configDir);
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }

        public static LoginSettings Load()
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<LoginSettings>(json) ?? new LoginSettings();
            }

            return new LoginSettings(); // 기본값 리턴
        }
    }
}

/*
private void Login_Click(object sender, RoutedEventArgs e)
{
    string id = txtID.Text;
    string pw = txtPassword.Password;

    bool loginSuccess = DoLogin(id, pw);

    if (loginSuccess)
    {
        var settings = new LoginSettings();

        if (chkSaveID.IsChecked == true)
        {
            settings.SavedID = id;
            settings.SaveID = true;
        }
        else
        {
            settings.SavedID = "";
            settings.SaveID = false;
        }

        settings.AutoLogin = chkAutoLogin.IsChecked == true;

        LoginSettingsManager.Save(settings);

        MessageBox.Show("로그인 성공");
        // 메인 화면으로 이동
    }
    else
    {
        MessageBox.Show("로그인 실패");
    }
}
 */

/*
public MainWindow()
{
    InitializeComponent();

    var settings = LoginSettingsManager.Load();

    if (settings.SaveID)
    {
        txtID.Text = settings.SavedID;
        chkSaveID.IsChecked = true;
    }

    chkAutoLogin.IsChecked = settings.AutoLogin;

    // 자동 로그인 동작 구현 가능
}
 */