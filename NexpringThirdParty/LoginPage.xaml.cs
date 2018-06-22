using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace NexpringThirdParty
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginProcess();
        }

        private void IDBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Windows RT의 버그(KeyDown이 중복 발생) 방지 코드
            if (e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    LoginProcess();
            }
        }

        private void PWBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    LoginProcess();
            }
        }

        private void LoginProcess()
        {
            // ID나 Password를 안 적으면 오류 발생
            if (IDBox.Text == "")
            {
                MessageBoxOpen("ID를 입력하십시오.");
            }
            else if (PWBox.Password == "")
            {
                MessageBoxOpen("Password를 입력하십시오.");
            }
            else
            {
                SigningRing.IsActive = true;

                string uri = App.localSettings.Values["defaultAddress"] + "cgi-bin/ltestatus.cgi?Command=Status";
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.ServerCredential = new PasswordCredential(uri, IDBox.Text, PWBox.Password);
                // 잘못 로그인 시 재로그인 UI출현 방지
                filter.AllowUI = false;
                HttpClient httpClient = new HttpClient(filter);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri, UriKind.Absolute);
                

                try
                {
                    HttpResponseMessage response = Task.Run(async () => { return await httpClient.SendRequestAsync(request); }).Result;
                    var content = response.Content.ReadAsStringAsync().GetResults();
                    // Content.Text = content;
                    MessageBoxOpen(content);
                    SigningRing.IsActive = false;
                }
                catch (Exception)
                {
                    MessageBoxOpen("오류가 발생했습니다.\n이 장치가 egg와 연결되어 있는지 확인하시고 egg의 IP주소가 192.168.1.1인지 확인해주십시오.");
                    SigningRing.IsActive = false;
                }
            }
        }

        private async void MessageBoxOpen(string showString)
        {
            var dialog = new MessageDialog(showString);
            await dialog.ShowAsync();
        }

        
    }
}
