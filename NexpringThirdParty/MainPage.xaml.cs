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

        private async void MessageBoxOpen(string showString)
        {
            var dialog = new MessageDialog(showString);
            await dialog.ShowAsync();
        }
    }
}
