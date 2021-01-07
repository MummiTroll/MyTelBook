using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TelBook
{
    public class Admin : INotifyPropertyChanged
    {
        public MainViewModel MainViewModel { get; set; }
        public string Password
        {
            get
            {
                return LoadString();
            }
        }
        public Admin(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void LoginIn() 
        {
            if (MainViewModel.Info1 == Password)
            {
                MainViewModel.DeleteDB();
            }
            else
            {
                MainViewModel.Info1 = "Password is wrong!";
            }
        }
        public void SetPass()
        {
            SaveString(MainViewModel.Info1);
        }
        public bool CanAdmin()
        {
            bool a = false;
            if (MainViewModel.admin == true)
            {
                a = true;
            }
            return a;
        }
        public string LoadString()
        {
            string password = string.Empty;
            if (Directory.Exists("Login"))
            {
                if (File.Exists(@"Login\CurrentPassword.bin"))
                {
                    FileStream fs = new FileStream(@"Login\CurrentPassword.bin", FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    password = (string)formatter.Deserialize(fs); fs.Close();
                }
            }
            return password;
        }
        public void SaveString(string password)
        {
            if (!Directory.Exists("Login")) { Directory.CreateDirectory("Login"); }
            if (Directory.Exists("Login"))
            {
                var binaryFormatter = new BinaryFormatter(); var fi = new FileInfo(@"Login\CurrentPassword.bin");
                using (var binaryFile = fi.Create())
                { binaryFormatter.Serialize(binaryFile, password); binaryFile.Flush(); }
            }
        }
        private void OnPropertyChange(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
