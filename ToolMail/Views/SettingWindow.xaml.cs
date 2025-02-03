using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolMail.Controllers;

namespace ToolMail.Views
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        public ProcessMail _ProcessMail { get; set; } = new ProcessMail();
        #region handle event
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow._ProcessMail._captchaKey = wpf_apiKey.Text;
                mainWindow._ProcessMail._numThread = wpf_thread.Value.Value;
                mainWindow._ProcessMail._numThreadPerProxy = wpf_proxyLimit.Value.Value;
                TextRange textRange = new TextRange(wpf_proxyKey.Document.ContentStart, wpf_proxyKey.Document.ContentEnd);
                List<string> proxyKeys = textRange.Text.Replace("\r", "").Split( '\n' ).ToList();
                mainWindow._ProcessMail.setApikey(proxyKeys);
            }
            this.Close();
        }
        #endregion
    }
}
