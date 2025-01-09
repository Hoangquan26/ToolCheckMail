using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolMail.Constant;

namespace ToolMail.Models
{
    public class InputMail : INotifyPropertyChanged
    {
        public static int index = 0;
        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        private int _stt;
        public int STT
        {
            get => _stt;
            set
            {
                _stt = value;
                OnPropertyChanged(nameof(STT));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _proxy;
        public string Proxy
        {
            get => _proxy;
            set
            {
                _proxy = value;
                OnPropertyChanged(nameof(Proxy));
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public InputMail(string email, string password)
        {
            this.Email = email;
            this.Status = MailStatusConstant.Idle;
            this.Password = password;
            this.Proxy = "";
            this.STT = ++index;
            this.Checked = false;
        }
    }
}
