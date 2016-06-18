using System;
using System.Windows.Navigation;
using mshtml;

namespace vk_search_v3.Windows
{
    public partial class LoginWindow
    {
        public string AccessToken { get; set; }
        public string UserId { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            wbLogin.Navigate("https://oauth.vk.com/authorize?client_id=3890541&scope=audio,offline&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.1&response_type=token");
        }

        private void WbLogin_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("access_token=") && e.Uri.ToString().Contains("user_id="))
            {
                AccessToken = e.Uri.ToString().Split(new[] {"access_token="}, StringSplitOptions.None)[1].Split('&')[0];
                UserId = e.Uri.ToString().Split(new[] {"user_id="}, StringSplitOptions.None)[1].Split('&')[0];
                DialogResult = true;
            }
            else
            {
                Title = "Please log in - vk.com search";
            }

            var dom = (HTMLDocumentClass)wbLogin.Document;
            var styleSheet = dom.createStyleSheet("", dom.styleSheets.length);
            var styleText = Properties.Resources.login_style;
            styleSheet.cssText = styleText;
        }
    }
}
