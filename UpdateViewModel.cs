using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TelBook
{
    public class UpdateViewModel
    {
        #region objects, lists, properties and variables
        public MainViewModel MainViewModel { get; set; }
        public DBViewModel DBViewModel { get; set; }
        public bool CorrectItem = false;
        public UpdateViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public Dictionary<string, List<List<string>>> DBToUpdate = new Dictionary<string, List<List<string>>>();
        public Dictionary<string, List<List<string>>> DBToUpdateTmp = new Dictionary<string, List<List<string>>>();
        public Dictionary<string, List<List<string>>> DBTmp = new Dictionary<string, List<List<string>>>();
        public List<string> FieldsFormular = new List<string>() { "Surname", "Name", "Middle name", "Telephone", "Telephone", "Telephone", "Email", "Email", "Address", "City", "Country", "Birthday", "Skype", "VK", "Homepage" };
        public List<string> FieldsUpdate = new List<string>() { "Surname", "Name", "Middle name", "Telephone", "Email", "Address", "City", "Country", "Birthday", "Skype", "VK", "Homepage" };
        public List<string> ImportFields1 = new List<string>() { "surname", "familyname", "family name" };
        public List<string> ImportFields2 = new List<string>() { "name", "first name", "firstname", "myname", "my name" };
        public List<string> ImportFields3 = new List<string>() { "middlename", "middle name", "fathers name", "fathersname" };
        public List<string> ImportFields4 = new List<string>() { "telephone", "tel", "tel.", "phone", "fon", "telefon" };
        public List<string> ImportFields5 = new List<string>() { "email", "e.mail", "e-Mail", "mail" };
        public List<string> ImportFields6 = new List<string>() { "address", "addresse", "addr", "addr.", "ad", "ad.", "place to live" };
        public List<string> ImportFields7 = new List<string>() { "city", "town", "metropolia" };
        public List<string> ImportFields8 = new List<string>() { "country", "area", "province" };
        public List<string> ImportFields9 = new List<string>() { "birthday", "b-day" };
        public List<string> ImportFields10 = new List<string>() { "skype", "myskype", "my skype", "skypeaccount", "skype account", "skype-account", "skypenickname", "skype nickname", "skype-nickname", "skypelogin", "skype login", "skype-login" };
        public List<string> ImportFields11 = new List<string>() { "vk", "myvk", "my vk", "vkaccount", "vk account", "vk-account", "vknickname", "vk nickname", 
            "vk-nickname", "vklogin", "vk login", "vk-login", "Comment", "Kommentar","Comm.","Data","Firma","Company","Firm"};
        public List<string> ImportFields12 = new List<string>() { "homepage", "internetpage", "internet page", "internet-page", "internetsite", "internet site", "internet-site", "internetseite" };
        List<List<string>> testForImport = new List<List<string>>();
        public List<int> IdenticalItems = new List<int>();
        public List<int> DifferentItemsSameSurname = new List<int>();
        public bool item = false;
        public List<List<string>> TestForImport
        {
            get
            {
                return SetTestForImport();
            }
        }
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
        #endregion
        public void ManualUpdate()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                if (!MainViewModel.textRange.Text.Contains("Manual update"))
                {
                    string caption = "\nManual update\n\n";
                    string textbody = "";
                    for (int i = 0; i < FieldsFormular.Count(); i++)
                    {
                        textbody += FieldsFormular[i] + ":\n";
                    }
                    MainViewModel.ShowResults(caption, textbody);
                }
                else
                {
                    if (MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                    {
                        MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
                    }
                    else
                    {
                        List<string> DBsListCopy = new List<string>(MainViewModel.DBsList);
                        DBsListCopy.Add(MainViewModel.DBItem);
                        MainViewModel.ReadWrite.SaveList(DBsListCopy, "DBs");
                        MainViewModel.ReadWrite.SaveList(MainViewModel.DBViewModel.fields, MainViewModel.DBItem + "Fields");
                        MainViewModel.DBViewModel.DB.Clear();
                    }
                    MainViewModel.DBViewModel.Import_String(MainViewModel.textRange.Text);
                    MainViewModel.DBViewModel.Import_Txt();
                    MainViewModel.textRange.Text = string.Empty;
                }
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public bool CanDoManualUpdate()
        {
            bool canUpdate = false;
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                canUpdate = true;
            }
            return true;
        }
        List<List<string>> SetTestForImport()
        {
            testForImport.Clear();
            testForImport.Add(ImportFields1);
            testForImport.Add(ImportFields2);
            testForImport.Add(ImportFields3);
            testForImport.Add(ImportFields4);
            testForImport.Add(ImportFields5);
            testForImport.Add(ImportFields6);
            testForImport.Add(ImportFields7);
            testForImport.Add(ImportFields8);
            testForImport.Add(ImportFields9);
            testForImport.Add(ImportFields10);
            testForImport.Add(ImportFields11);
            testForImport.Add(ImportFields12);
            return testForImport;
        }
        public List<string> SplitString(string text)
        {
            List<string> splitList = new List<string>();
            string[] split = text.Split('\n');
            foreach (string spl in split)
            {
                if (spl != "" && spl != " " && spl != "\r")
                {
                    string[] split1 = spl.Split('\r');
                    foreach (string spl1 in split1)
                    {
                        if (spl1 != "" && spl1 != " ")
                        {
                            splitList.Add(spl1);
                        }
                    }
                }
            }
            return splitList;
        }
        public string KillWhitespaces(string line)
        {
            string result = line;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                {
                    result = result.Substring(1);
                }
                else
                {
                    break;
                }
            }
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] == ' ')
                {
                    result = result.Substring(0, result.Length - 1);
                }
                else
                {
                    break;
                }
            }
            return result;
        }
        public void NextItem()
        {
            List<List<string>> DBvalue = new List<List<string>>();
            if (Surname.Count == 0) { Surname.Add(""); }
            if (Name.Count == 0) { Name.Add(""); }
            if (MiddleName.Count == 0) { MiddleName.Add(""); }
            if (Telephone.Count == 0) { Telephone.Add(""); }
            if (Email.Count == 0) { Email.Add(""); }
            if (Address.Count == 0) { Address.Add(""); }
            if (City.Count == 0) { City.Add(""); }
            if (Country.Count == 0) { Country.Add(""); }
            if (Birthday.Count == 0) { Birthday.Add(""); }
            if (Skype.Count == 0) { Skype.Add(""); }
            if (VK.Count == 0) { VK.Add(""); }
            if (Homepage.Count == 0) { Homepage.Add(""); }

            List<int> Count5_7 = new List<int>() { Address.Count, City.Count, Country.Count };
            Count5_7.Sort();
            int maxCount = Count5_7[Count5_7.Count - 1];

            for (int d = 0; d < maxCount - Address.Count; d++)
            {
               Address.Add("");
            }
            for (int d = 0; d < maxCount - City.Count; d++)
            {
                City.Add("");
            }
            for (int d = 0; d < maxCount - Country.Count; d++)
            {
                Country.Add("");
            }
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
            if (!String.IsNullOrEmpty(Surname[0]))
            {
                MainViewModel.DBViewModel.DB[Surname[0]] = DBvalue;
            }
            Surname = new List<string>();
            Name = new List<string>();
            MiddleName = new List<string>();
            Telephone = new List<string>();
            Email = new List<string>();
            Address = new List<string>();
            City = new List<string>();
            Country = new List<string>();
            Birthday = new List<string>();
            Skype = new List<string>();
            VK = new List<string>();
            Homepage = new List<string>();
        }
        public void MergeDics()
        {
            DBTmp = new Dictionary<string, List<List<string>>>(MainViewModel.DBViewModel.DB); //Newly imported DB
            List<string> keyListTmp = new List<string>(DBTmp.Keys);
            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            DBToUpdate = new Dictionary<string, List<List<string>>>(MainViewModel.DBViewModel.DB); //Already existing DB
            List<string> keyListToUpdate = new List<string>(DBToUpdate.Keys);
            for (int i = 0; i < keyListTmp.Count(); i++)
            {
                IdenticalItems = new List<int>();
                DifferentItemsSameSurname = new List<int>();
                string SurnameTmp = KillWhitespaces(DBTmp[keyListTmp[i]][0][0]);
                for (int j = 0; j < keyListToUpdate.Count(); j++)
                {
                    string SurnameToUpdate = DBToUpdate[keyListToUpdate[j]][0][0];
                    //If Surnames are identical
                    if (SurnameToUpdate.ToLower() == SurnameTmp.ToLower())
                    {
                        //Check whether it is the same item
                        DetectRedunduncies(keyListTmp[i], j, keyListToUpdate);
                    }
                }
                //Same item
                if (IdenticalItems.Count() > 0)
                {
                    SameItems(keyListTmp[i], keyListToUpdate);
                }
                //New item same surname
                else if (IdenticalItems.Count() == 0 && DifferentItemsSameSurname.Count() > 0)
                {
                    NewItemSameSurname(keyListToUpdate, keyListTmp[i]);
                }
                //Completely new item
                else if (IdenticalItems.Count() == 0 && DifferentItemsSameSurname.Count() == 0)
                {
                    DBToUpdate[keyListTmp[i]] = DBTmp[keyListTmp[i]];
                }
            }
            MainViewModel.ReadWrite.SaveDic(DBToUpdate, @"DB\", MainViewModel.DBItem);
            MainViewModel.DBViewModel.DB.Clear();
            DBToUpdate.Clear();
            DBTmp.Clear();
        }
        private void DetectRedunduncies(string keyTmp, int indexToUpdate, List<string> keyListToUpdate)
        {
            bool SameEntry = false;
            for (int i = 3; i < 4; i++)
            {
                for (int j = 0; j < DBTmp[keyTmp][i].Count(); j++)
                {
                    if (!String.IsNullOrEmpty(DBTmp[keyTmp][i][j]) && !String.IsNullOrWhiteSpace(DBTmp[keyTmp][i][j]))
                    {
                        for (int k = 0; k < DBToUpdate[keyListToUpdate[indexToUpdate]][i].Count(); k++)
                        {
                            if (DBToUpdate[keyListToUpdate[indexToUpdate]][i][k].ToLower() == KillWhitespaces(DBTmp[keyTmp][i][j]).ToLower())
                            {
                                SameEntry = true;
                            }
                        }
                    }
                }
            }
            if (SameEntry == true)
            {
                IdenticalItems.Add(indexToUpdate);
            }
            else
            {
                DifferentItemsSameSurname.Add(indexToUpdate);
            }
        }
        private void SameItems(string keyTmp, List<string> keyListToUpdate)
        {
            //Merging data from the new and multiple DBToUpdate identical items (if any)
            DBToUpdate["TestTmp"] = DBToUpdate[keyListToUpdate[IdenticalItems[0]]];
            for (int i = 0; i < DBToUpdate["TestTmp"].Count(); i++)
            {
                for (int j = 0; j < DBTmp[keyTmp][i].Count(); j++)
                {
                    string newTmpSubentry = KillWhitespaces(DBTmp[keyTmp][i][j]);
                    if (!DBToUpdate["TestTmp"][i].Contains(newTmpSubentry))
                    {
                        DBToUpdate["TestTmp"][i].Add(newTmpSubentry);
                    }
                }
            }
            for (int i = 1; i < IdenticalItems.Count(); i++)
            {
                for (int j = 0; j < DBToUpdate["TestTmp"].Count(); j++)
                {
                    for (int k = 0; k < DBToUpdate[keyListToUpdate[IdenticalItems[i]]][j].Count(); k++)
                    {
                        if (!DBToUpdate["TestTmp"][j].Contains(DBToUpdate[keyListToUpdate[IdenticalItems[i]]][j][k]))
                        {
                            DBToUpdate["TestTmp"][j].Add(DBToUpdate[keyListToUpdate[IdenticalItems[i]]][j][k]);
                        }
                    }
                }
            }
            DBToUpdateTmp = new Dictionary<string, List<List<string>>>(DBToUpdate);
            for (int i = 0; i < IdenticalItems.Count(); i++)
            {
                DBToUpdateTmp.Remove(keyListToUpdate[IdenticalItems[i]]);
            }
            DBToUpdateTmp[keyListToUpdate[IdenticalItems[0]]] = DBToUpdate["TestTmp"];
            DBToUpdateTmp.Remove("TestTmp");
            List<string> keyListToUpdateTmp = new List<string>(DBToUpdateTmp.Keys);
            MakeKeysUnique(keyListToUpdateTmp, keyListToUpdateTmp[IdenticalItems[0]]);
            DBToUpdate = new Dictionary<string, List<List<string>>>(DBToUpdateTmp);
        }
        private void NewItemSameSurname(List<string> keyListToUpdate, string KeyTmp)
        {
            DBToUpdateTmp = new Dictionary<string, List<List<string>>>(DBToUpdate);
            int MaxItemVersionNumber = FindTheHighestKeyVersionNumber(keyListToUpdate);
            if (MaxItemVersionNumber == 0)
            {
                DBToUpdateTmp[KeyTmp.Split('(')[0] + "(1)"] = DBToUpdateTmp[KeyTmp.Split('(')[0]];
                DBToUpdateTmp[KeyTmp.Split('(')[0] + "(2)"] = DBTmp[KeyTmp];
                DBToUpdateTmp.Remove(KeyTmp.Split('(')[0]);
            }
            else if (MaxItemVersionNumber > 0)
            {
                DBToUpdateTmp[KeyTmp.Split('(')[0] + "(" + (MaxItemVersionNumber + 1).ToString() + ")"] = DBTmp[KeyTmp];
            }
            DBToUpdate = new Dictionary<string, List<List<string>>>(DBToUpdateTmp);
            DBToUpdateTmp.Clear();
        }
        //Make keys for a particular surname unique in the whole DBToUpdate through creating indices: keys: Surname(1), Surname(2)...
        public void MakeKeysUnique(List<string> keyListTmp, string keyToAdjustTmp)
        {
            Dictionary<string, List<List<string>>> DBTmpTmp = new Dictionary<string, List<List<string>>>(DBToUpdate);
            string surnameToAdjustKeys = DBToUpdate[keyToAdjustTmp][0][0];
            int count = 0;
            for (int i = 0; i < keyListTmp.Count(); i++)
            {
                if (DBTmpTmp[keyListTmp[i]][0][0] == surnameToAdjustKeys)
                {
                    DBToUpdate.Remove(keyListTmp[i]);
                    if (count == 0)
                    {
                        DBToUpdate[surnameToAdjustKeys] = DBTmpTmp[keyListTmp[i]];
                    }
                    else
                    {
                        DBToUpdate[surnameToAdjustKeys + "(" + count.ToString() + ")"] = DBTmpTmp[keyListTmp[i]];
                    }
                    count += 1;
                }
            }
        }
        public int FindTheHighestKeyVersionNumber(List<string> keyListToUpdate)
        {
            int ItemVersionNumber = 0;
            for (int i = 0; i < DifferentItemsSameSurname.Count(); i++)
            {
                if (keyListToUpdate[DifferentItemsSameSurname[i]].Contains("(") && keyListToUpdate[i].Contains(")"))
                {
                    if (Int32.TryParse(keyListToUpdate[DifferentItemsSameSurname[i]].Split('(')[1].Split(')')[0], out int result))
                    {
                        int res = Int32.Parse(keyListToUpdate[DifferentItemsSameSurname[i]].Split('(')[1].Split(')')[0]);
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