using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace TelBook
{
    public class Test
    {
        #region Objects lists variables properties
        Random rnd = new Random();
        public Dictionary<string, List<List<string>>> TestTelDB = new Dictionary<string, List<List<string>>>();
        public List<string> Fields = new List<string>() { "Surname", "Name", "Middle name", "Telephone", "Email", "Address",
                "City", "Country", "Birthday", "Skype", "VK", "Homepage" };
        public List<string> Surname = new List<string>();
        public List<string> Name = new List<string>();
        public List<string> MiddleName = new List<string>();
        public List<string> Telephone = new List<string>();
        public List<string> Email = new List<string>();
        public List<string> Address = new List<string>();
        public List<string> City = new List<string>();
        public List<string> Country = new List<string>();
        public List<string> Birthday = new List<string>();
        public List<string> Skype = new List<string>();
        public List<string> VK = new List<string>();
        public List<string> Homepage = new List<string>();
        public List<string> ListABC = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "O", "P", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
        public List<string> Listabc = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public List<string> ListTel = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public MainViewModel MainViewModel { get; set; }
        #endregion
        public Test(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public void MakeTestDB()
        {
            Dictionary<string, List<List<string>>> TestDB = new Dictionary<string, List<List<string>>>();
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrEmpty(MainViewModel.Import_DBname))
            {
                int number = Int32.Parse(MainViewModel.Import_DBname);
                for (int i = 0; i < number; i++)
                {
                    List<List<string>> DBvalue = new List<List<string>>();
                    //#####Make random Surname#############################################
                    //surnameStart:
                    string surname = "";
                    List<string> Surname = new List<string>();
                    if (MainViewModel.DBItem.Length >= 3)
                    {
                        surname = MainViewModel.DBItem.Substring(0, 3);
                        surname += RndName(6, rnd.Next(10, 15) - 3);
                        Surname.Add(surname);
                    }
                    else
                    {
                        surname += RndName(6, rnd.Next(10, 15) - 3);
                        Surname.Add(surname);
                    }
                    //CheckSurname.Add(surname);
                    //#####Make random Name#############################################
                    List<string> Name = new List<string>();
                    string name = RndName(5, rnd.Next(10, 15));
                    Name.Add(name);
                    //#####Make random MiddleName#############################################
                    List<string> MiddleName = new List<string>();
                    string middleName = RndName(5, rnd.Next(10, 15));
                    MiddleName.Add(middleName);
                    //#####Make random Telephone#############################################
                    List<string> Telephone = new List<string>();
                    string telephone = RndTel(12);
                    Telephone.Add(telephone);
                    telephone = RndTel(12);
                    Telephone.Add(telephone);
                    telephone = RndTel(12);
                    Telephone.Add(telephone);
                    //#####Make random Email#############################################
                    List<string> Email = new List<string>();
                    string email = RndEmail(4, 3);
                    Email.Add(email);
                    email = RndEmail(4, 3);
                    Email.Add(email);
                    //#####Make random Address#############################################
                    List<string> Address = new List<string>();
                    string address = RndAddress(6, 2);
                    Address.Add(address);
                    //#####Make random City#############################################
                    List<string> City = new List<string>();
                    string city = RndName(5, 10);
                    City.Add(city);
                    //#####Make random Country#############################################
                    List<string> Country = new List<string>();
                    string country = RndName(4, 10);
                    Country.Add(country);
                    //#####Make random Birthday#############################################
                    List<string> Birthday = new List<string>();
                    string birthday = RndBirthday();
                    Birthday.Add(birthday);
                    //#####Make random Skype#############################################
                    List<string> Skype = new List<string>();
                    string skype = ListABC[rnd.Next(0, ListABC.Count)].ToUpper();
                    skype += RndString(rnd.Next(5, 10));
                    Skype.Add(skype);
                    //#####Make random VK#############################################
                    List<string> VK = new List<string>();
                    string vk = ListABC[rnd.Next(0, ListABC.Count)].ToUpper();
                    vk += RndString(rnd.Next(5, 10));
                    VK.Add(vk);
                    //#####Make random Homepage#############################################
                    List<string> Homepage = new List<string>();
                    string homepage = RndHomepage(rnd.Next(5, 10), rnd.Next(2, 4));
                    Homepage.Add(homepage);
                    DBvalue.Add(Surname);
                    DBvalue.Add(Name);
                    DBvalue.Add(MiddleName);
                    DBvalue.Add(Telephone);
                    DBvalue.Add(Email);
                    DBvalue.Add(Address);
                    DBvalue.Add(City);
                    DBvalue.Add(Country);
                    DBvalue.Add(Birthday);
                    DBvalue.Add(Skype);
                    DBvalue.Add(VK);
                    DBvalue.Add(Homepage);
                    TestDB[surname] = DBvalue;
                }
                if (MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                {
                    if (MainViewModel.DBItem.Contains("(") && MainViewModel.DBItem.Contains(")"))
                    {
                        int Num = HighestVersionNumber();
                        Num += 1;
                        MainViewModel.DBItem = MainViewModel.DBItem.Split('(')[0] + "(" + Num + ")";
                    }
                    else
                    {
                        MainViewModel.DBItem = MainViewModel.DBItem + "(1)";
                    }
                }

                MainViewModel.ReadWrite.SaveDic(TestDB, @"DB\", MainViewModel.DBItem);
                MainViewModel.ReadWrite.SaveList(MainViewModel.DBViewModel.fields, MainViewModel.DBItem + "Fields");
                if (File.Exists(@"DB\DBs.bin"))
                {
                    List<string> DBsListCopy = new List<string>(MainViewModel.DBsList);
                    DBsListCopy.Add(MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveList(DBsListCopy, "DBs");
                }
                else
                {
                    List<string> DBsListCopy = new List<string>();
                    DBsListCopy.Add(MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveList(DBsListCopy, "DBs");
                }
            }
        }
        public bool CanMakeTestDB()
        {
            bool a = false;
            if (MainViewModel.admin == true)
            {
                a = true;
            }
            return a;
        }
        public string RndString(int a)
        {
            string word = "";
            for (int i = 0; i < a; i++)
            {
                word += ListABC[rnd.Next(0, ListABC.Count)];
            }
            return word;
        }
        public string RndTel(int a)
        {
            string tel = "+";
            for (int i = 0; i < a; i++)
            {
                tel += ListTel[rnd.Next(0, ListTel.Count)];
            }
            return tel;
        }
        public string RndEmail(int a, int b)
        {
            string email = "";
            for (int i = 0; i < a; i++)
            {
                email += ListABC[rnd.Next(0, ListABC.Count)];
            }
            email += "@";
            for (int i = 0; i < b; i++)
            {
                email += ListABC[rnd.Next(0, ListABC.Count)];
            }
            email += ".";
            for (int i = 0; i < 3; i++)
            {
                email += ListABC[rnd.Next(0, ListABC.Count)];
            }

            return email;
        }
        public string RndAddress(int a, int b)
        {
            string address = "";
            string c = "0";
            while (c == "0")
            {
                c = ListTel[rnd.Next(0, ListTel.Count)].ToString();
                address += c;
            }
            for (int i = 0; i < 6; i++)
            {
                address += ListTel[rnd.Next(0, ListTel.Count)];
            }
            address += " ";
            address += Listabc[rnd.Next(0, Listabc.Count)].ToUpper();
            for (int i = 0; i < a - 1; i++)
            {
                address += Listabc[rnd.Next(0, Listabc.Count)];
            }
            address += " street ";
            string d = "0";
            while (d == "0")
            {
                d = ListTel[rnd.Next(0, ListTel.Count)].ToString();
                address += d;
            }
            address += ListTel[rnd.Next(0, ListTel.Count)].ToString();
            return address;
        }
        public string RndName(int a, int b)
        {
            string name = ListABC[rnd.Next(0, Listabc.Count)].ToUpper();
            int length = rnd.Next(a, b);

            for (int i = 0; i < length; i++)
            {
                name += Listabc[rnd.Next(0, Listabc.Count)];
            }
            return name;
        }
        public string RndBirthday()
        {
            string birthday = "";
            int day = rnd.Next(1, 32);
            if (day > 9)
            {
                birthday += day.ToString() + ".";
            }
            else
            {
                birthday += "0" + day.ToString() + ".";
            }

            int month = rnd.Next(1, 13);
            if (month > 9)
            {
                birthday += month.ToString() + ".";
            }
            else
            {
                birthday += "0" + month.ToString() + ".";
            }
            birthday += "19";

            int year = rnd.Next(0, 100);
            if (year > 9)
            {
                birthday += year.ToString();
            }
            else
            {
                birthday += "0" + year.ToString();
            }
            return birthday;
        }
        public string RndHomepage(int a, int b)
        {
            string homepage = "www.";
            for (int i = 0; i < a; i++)
            {
                homepage += ListABC[rnd.Next(0, ListABC.Count)];
            }
            homepage += ".";
            for (int i = 0; i < b; i++)
            {
                homepage += Listabc[rnd.Next(0, Listabc.Count)];
            }
            return homepage;
        }
        public int HighestVersionNumber()
        {
            int ItemVersionNumber = 0;
            for (int i = 0; i < MainViewModel.DBsList.Count; i++)
            {
                if (MainViewModel.DBsList[i].Contains("(") && MainViewModel.DBsList[i].Contains(")"))
                {
                    if (Int32.TryParse(MainViewModel.DBsList[i].Split('(')[1].Split(')')[0], out int result))
                    {
                        int res = Int32.Parse(MainViewModel.DBsList[i].Split('(')[1].Split(')')[0]);
                        if (res > ItemVersionNumber)
                        {
                            ItemVersionNumber = res;
                        }
                    }
                }
            }
            return ItemVersionNumber;
        }
    }
}
