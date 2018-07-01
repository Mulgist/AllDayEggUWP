using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Core;
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

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace NexpringThirdParty
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            App.titleStack.Push("홈");
            // 내부 페이지에 HomePage 로드
            // MainSplitViewContent.Navigate(typeof(HomePage));
            HomeListBoxItem.IsSelected = false;

            // 뒤로가기 버튼
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if ((MainSplitView.Content as Frame).CanGoBack)
                {
                    App.titleStack.Pop();
                    Title.Text = App.titleStack.Peek();
                    (MainSplitView.Content as Frame).GoBack();
                    a.Handled = true;
                }
                else
                {
                    if ((bool)(App.localSettings.Values["IsBackExit"]) == true)
                        CoreApplication.Exit();
                }
            };
        }

        private void HambergerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void HambergerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
            if (HomeListBoxItem.IsSelected)
            {
                App.titleStack.Push("홈");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(HomePage));
                return;
            }
            if (LTEInfoListBoxItem.IsSelected)
            {
                App.titleStack.Push("LTE 정보");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "164");
                return;
            }
            if (WiFiInfoListBoxItem.IsSelected)
            {
                App.titleStack.Push("WiFi 정보");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(PlanPage));
                return;
            }
            if (SystemInfoListBoxItem.IsSelected)
            {
                App.titleStack.Push("시스템 정보");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(PlanPage));
                return;
            }
            if (EggSettingListBoxItem.IsSelected)
            {
                App.titleStack.Push("Egg 설정");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(PlanPage));
                return;
            }
            if (AppSettingListBoxItem.IsSelected)
            {
                App.titleStack.Push("My all-day egg 설정");
                Title.Text = App.titleStack.Peek();
                // (MainSplitView.Content as Frame).Navigate(typeof(PlanPage));
                return;
            }
        }

        private async void MessageBoxOpen(string content, string title)
        {
            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }

        // 확인, 취소 버튼이 있는 메시지를 나타낸다. 그리고 확인을 눌렀는지 취소를 눌렀는지에 대한 Command를 return한다.
        private async Task<IUICommand> YesOrNoMessageBoxOpen(string content, string title, UICommand okCommand, UICommand cancelCommand)
        {
            var dialog = new MessageDialog(content, title);
            dialog.Options = MessageDialogOptions.None;
            dialog.Commands.Add(okCommand);

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            if (cancelCommand != null)
            {
                dialog.Commands.Add(cancelCommand);
                dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
            }

            var command = await dialog.ShowAsync();
            return command;
        }

        private async void UnderlineBtns_ClickAsync(object sender, RoutedEventArgs e)
        {
            if ((Button)sender == RefreshBtn)
            {
                MessageBoxOpen("기능 준비중입니다.", "준비중");
            }
            else if ((Button)sender == ShutdownBtn)
            {
                var okCommand = new UICommand("확인", cmd => {
                    // Egg 종료
                    var content = CommunicationService.instance.GetXmlString("http://192.168.1.1/cgi-bin/systemreboot.cgi?Command=Poweroff");
                    if (content != "ERROR")
                    {
                        // 응답이 설치
                        MessageBoxOpen("Egg가 종료중입니다. 다시 켜기 전까지 새로운 정보를 얻을 수 없습니다.", "Power Off");
                    }
                    else
                    {
                        MessageBoxOpen("Egg 종료에 실패했습니다. 다시 한 번 시도해 주십시오.", "Power Off");
                    }
                });
                var cancelCommand = new UICommand("취소", cmd => { });
                await YesOrNoMessageBoxOpen("Egg를 종료합니다.", "Power Off", okCommand, cancelCommand);
            }
            else if ((Button)sender == RestartBtn)
            {
                var okCommand = new UICommand("확인", cmd => {
                    // Egg 재부팅
                    var content = CommunicationService.instance.GetXmlString("http://192.168.1.1/cgi-bin/systemreboot.cgi?Command=Reset");
                    if (content != "ERROR")
                    {
                        // 응답이 설치
                        MessageBoxOpen("Egg가 재부팅중입니다. 재부팅 소요 시간은 약 50초 입니다.", "Restart");
                        // 재부팅 시작 후 작업: 50초동안 refresh 막기 localsetting을 사용하는 것도 좋은 방법
                        
                    }
                    else
                    {
                        MessageBoxOpen("Egg 재부팅에 실패했습니다. 다시 한 번 시도해 주십시오.", "Restart");
                    }
                });
                var cancelCommand = new UICommand("취소", cmd => { });
                await YesOrNoMessageBoxOpen("Egg를 재부팅합니다.", "Restart", okCommand, cancelCommand);
            }
            else if ((Button)sender == LogoutBtn)
            {
                var okCommand = new UICommand("확인", cmd => {
                    // 계정 인증정보 초기화
                    App.filter = new HttpBaseProtocolFilter();
                    App.localSettings.Values["isLogout"] = true;
                    // 페이지 뒤로 가기
                    (Window.Current.Content as Frame).GoBack();
                });
                var cancelCommand = new UICommand("취소", cmd => { });
                await YesOrNoMessageBoxOpen("이 앱에서 로그아웃합니다.", "Logout", okCommand, cancelCommand);
            }
        }
    }
}
