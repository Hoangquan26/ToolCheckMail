﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToolMail.Constant;
using ToolMail.Controllers;
using ToolMail.Models;
using ToolMail.Utils;
using ToolMail.Views;

namespace ToolMail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Properties
        public ObservableCollection<InputMail> inputMail = new ObservableCollection<InputMail>();
        public ProcessMail _ProcessMail { get; set; } = new ProcessMail();
        public ObservableCollection<InputMail> InputMail
        {
            get => inputMail;
            set
            {
                inputMail = value;
                OnPropertyChanged(nameof(InputMail));
                UpdateCounts();
            }
        }

        #endregion

        #region StatusProperties

        private int dieCount = 0;
        public int DieCount
        {
            get => dieCount;
            set
            {
                dieCount = value;
                OnPropertyChanged(nameof(DieCount));
            }
        }

        private int liveCount = 0;
        public int LiveCount
        {
            get => liveCount;
            set
            {
                liveCount = value;
                OnPropertyChanged(nameof(LiveCount));
            }
        }

        private int allCount = 0;
        public int AllCount
        {
            get => allCount;
            set
            {
                allCount = value;
                OnPropertyChanged(nameof(AllCount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            accountGrid.ItemsSource = inputMail;
        }


        private void UpdateCounts()
        {
            DieCount = inputMail.Count(mail => mail.Status == MailStatusConstant.Die);
            LiveCount = inputMail.Count(mail => mail.Status == MailStatusConstant.Live);
            AllCount = inputMail.Count;
        }

        #region handleUserInteract
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            var items = accountGrid.SelectedItems;
            foreach (InputMail mail in items)
            {
                mail.Checked = true;
            }
        }

        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            var items = accountGrid.SelectedItems;
            foreach (InputMail mail in items)
            {
                mail.Checked = false;
            }
        }



        private void ImportMail()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Chọn file txt";
            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                this.inputMail = new ObservableCollection<InputMail>(FileUtils.TEXT_getAccount(fileName));
                UpdateCounts();
                accountGrid.ItemsSource = InputMail;
            }
        }
        private void ImportTxt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult alertRes = MessageBoxResult.No;
                if (inputMail.Count() != 0)
                {
                    alertRes = MessageBox.Show(GetWindow(this), "Danh sách mail đã có dữ liệu, bạn có muốn thêm vào danh sách hiện tại không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                else alertRes = MessageBoxResult.Yes;

                if (alertRes == MessageBoxResult.Yes)
                {
                    ImportMail();
                }
            }
            catch
            {

            }
        }

        private void ExportTxt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Chọn nơi lưu file txt";
            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                StringBuilder sb = new StringBuilder();
                foreach (var mail in inputMail)
                {
                    sb.AppendLine($"{mail.Email}|{mail.Password}|{mail.Proxy}|{mail.Status}");
                }
                System.IO.File.WriteAllText(fileName, sb.ToString());
                MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var itemsToKeep = inputMail.Where(item => !item.Checked).ToList();
            var removeLength = inputMail.Count - itemsToKeep.Count;
            inputMail = new ObservableCollection<InputMail>(itemsToKeep);
            accountGrid.ItemsSource = inputMail;
        }


        private void StartProcessingEmails(object sender, RoutedEventArgs e)
        {
            Task mainTask = new Task(async() =>
            {
                var checkedMail = new ObservableCollection<InputMail>(inputMail.Where(mail => mail.Checked));
                await this._ProcessMail.ProcessEmailsAsync(checkedMail, this._ProcessMail._numThread); // Example: limit to 4 concurrent threads
            });
            mainTask.Start();
        }

        private void Setting_click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }
        #endregion

    }
}