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

        // 생성자에서 작업을 하면 오류남. (생성자에서 작업을 한 뒤 페이지를 로드하므로) 그래서 페이지 로드 이벤트 함수에 작업을 넣었다.
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if ((bool)App.localSettings.Values["isAutoLogin"])
            {
                // this.Loaded += Sleep;
                LoginProcess(true);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginProcess(false);
        }

        private void IDBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Windows RT의 버그(KeyDown이 중복 발생) 방지 코드
            if (e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    LoginProcess(false);
            }
        }

        private void PWBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    LoginProcess(false);
            }
        }

        private void LoginProcess(bool isAutoLogin)
        {
            // ID나 Password를 안 적으면 오류 발생
            if (!isAutoLogin)
            {
                if (IDBox.Text == "")
                {
                    MessageBoxOpen("ID를 입력하십시오.");
                    return;
                }
                else if (PWBox.Password == "")
                {
                    MessageBoxOpen("Password를 입력하십시오.");
                    return;
                }
            }
            
            SigningRing.IsActive = true;

            string uri = App.localSettings.Values["defaultAddress"] + "cgi-bin/ltestatus.cgi?Command=Status";
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            if (isAutoLogin)
            {
                filter.ServerCredential = new PasswordCredential(uri, Convert.ToString(App.localSettings.Values["savedID"]), Convert.ToString(App.localSettings.Values["savedPW"]));
            }
            else
            {
                filter.ServerCredential = new PasswordCredential(uri, IDBox.Text, PWBox.Password);
            }
            
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
                // MessageBoxOpen(content);
                SigningRing.IsActive = false;


                // XML 분석으로 성공과 실패를 구별해야 한다.

                // 성공 시 자동 로그인에 체크되어 있으면 체크 정보를 저장하고 아이디와 패스워드를 저장한다.
                // IsChecked만으로는 Bool? 자료형이기 때문에 조건문이 성립되지 않는다. == true가 같이 붙어야 한다.
                if (AutoLoginCheckBox.IsChecked == true && !isAutoLogin)
                {
                    App.localSettings.Values["isAutoLogin"] = true;
                    App.localSettings.Values["savedID"] = IDBox.Text;
                    App.localSettings.Values["savedPW"] = PWBox.Password;
                }

                // 성공 시 MainPage로 페이지 이동
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception)
            {
                MessageBoxOpen("AP에서 응답이 없습니다.\n이 장치가 egg와 연결되어 있는지 확인하시고 egg의 IP주소가 192.168.1.1인지 확인해주십시오.");
                SigningRing.IsActive = false;
            }
        }

        private async void MessageBoxOpen(string showString)
        {
            var dialog = new MessageDialog(showString);
            await dialog.ShowAsync();
        }

        // Sleep 대체 함수
        private async void Sleep(object sender, RoutedEventArgs e)
        {
            // 2 seconds (= 2000ms) 기다림
            await Task.Delay(2000);
        }
    }
}
