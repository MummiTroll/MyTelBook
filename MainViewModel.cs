using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using TelBook.Enums;

namespace TelBook
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public DBViewModel DBViewModel { get; set; }
        public SearchViewModel SearchViewModel { get; set; }
        public ReadWrite ReadWrite { get; set; }
        public UpdateViewModel UpdateViewModel { get; set; }
        public Test Test { get; set; }
        public Admin Admin { get; set; }
        string dBItem { get; set; }
        public string DBItem
        {
            get
            {
                return dBItem;
            }
            set
            {
                if (dBItem != value)
                {
                    dBItem = value;
                    OnPropertyChange(nameof(DBItem));
                    OnPropertyChange(nameof(DBsList));
                }
            }
        }
        public List<string> DBsList
        {
            get
            {
                return ReadWrite.LoadList("DBs");
            }
            set
            {
                OnPropertyChange(nameof(DBsList));
            }
        }
        private int hitsItem { get; set; }
        public int HitsItem
        {
            get
            {
                return hitsItem;
            }
            set
            {
                if (hitsItem != value)
                {
                    hitsItem = value;
                    OnPropertyChange(nameof(HitsItem));
                }
            }
        }
        public List<int> HitsCombo = new List<int>(){ 1, 2, 5, 10, 50, 100, 250 };
        public List<string> Extensions = new List<string>() { "txt", "xls", "xlsx", "docx", "pdf", "bin" };
        private string extensionItem { get; set; }
        public string ExtensionItem
        {
            get
            {
                return extensionItem;
            }
            set
            {
                if (extensionItem != value)
                {
                    extensionItem = value;
                    OnPropertyChange(nameof(ExtensionItem));
                    OnPropertyChange(nameof(Import_DBname));
                }
            }
        }
        int minWindow { get; set; } = 5;
        public int MinWindow
        {
            get
            {
                return minWindow;
            }
            set
            {
                if (minWindow != value)
                {
                    minWindow = value;
                    OnPropertyChange(nameof(MinWindow));
                }
            }
        }
        string Keyworditem { get; set; }
        public string KeywordItem
        {
            get
            {
                return Keyworditem;
            }
            set
            {
                if (Keyworditem != value)
                {
                    Keyworditem = value;
                    OnPropertyChange(nameof(KeywordItem));
                }
            }
        }
        public List<string> KeywordList
        {
            get
            {
                List<string> KeywordList = new List<string>();
                if (!String.IsNullOrEmpty(KeywordItem))
                {
                    string[] split = KeywordItem.Split(',');
                    foreach (string spl in split)
                    {
                        if (!String.IsNullOrEmpty(spl) && !String.IsNullOrWhiteSpace(spl) && spl != "\n" && spl != "\r")
                        {
                            string spl_new = UpdateViewModel.KillWhitespaces(spl);
                            KeywordList.Add(spl_new);
                        }
                    }
                    OnPropertyChange(nameof(KeywordList));
                }
                return KeywordList;
            }
        }
        string import_DBname { get; set; }
        public string Import_DBname
        {
            get
            {
                return import_DBname;
            }
            set
            {
                if (import_DBname != value)
                {
                    import_DBname = value;
                    OnPropertyChange(nameof(this.Import_DBname));
                }
            }
        }
        string import_Item { get; set; }
        public string Import_Item
        {
            get
            {
                return import_Item;
            }
            set
            {
                if (import_Item != value)
                {
                    import_Item = value;
                    OnPropertyChange(nameof(Import_Item));
                }
            }
        }
        string info { get; set; }
        public string Info
        {
            get
            {
                return info;
            }
            set
            {
                if (info != value)
                {
                    info = value;
                    OnPropertyChange(nameof(Info));
                }
            }
        }
        string info1 { get; set; } = "";
        public string Info1
        {
            get
            {
                return info1;
            }
            set
            {
                if (info1 != value)
                {
                    info1 = value;
                    OnPropertyChange(nameof(Info1));
                }
            }
        }
        public TextRange textrange { get; set; }
        public TextRange textRange
        {
            get
            {
                return textrange;
            }
            set
            {
                if (textrange != value)
                {
                    textrange = value;
                }
            }
        }
        public string selectedText { get; set; }
        public string SelectedText
        {
            get
            {
                return selectedText;
            }
            set
            {
                if (selectedText != value)
                {
                    selectedText = value;
                    OnPropertyChange(nameof(SelectedText));
                }
            }
        }
        private bool all_InOne = false;
        public bool All_InOne
        {
            get { return all_InOne; }
            set
            {
                if (all_InOne != value)
                {
                    all_InOne = value;
                    OnPropertyChange(nameof(All_InOne));
                }
            }
        }
        private bool aDmin { get; set; } = true;
        public bool admin
        {
            get
            {
                return aDmin;
            }
            set
            {
                if (aDmin != value)
                {
                    aDmin = value;
                    OnPropertyChange(nameof(admin));
                    OnPropertyChange(nameof(Visibility));
                }
            }
        }
        private MainWindowFunctions visible = MainWindowFunctions.Normalize;
        public MainWindowFunctions Visible
        {
            get { return this.visible; }
            set
            {
                this.visible = value;

                OnPropertyChange(nameof(Visible));
                OnPropertyChange(nameof(MaximizeButton));
                OnPropertyChange(nameof(NormalizeButton));
            }
        }
        public Visibility Visibility => admin == true ? Visibility.Visible : Visibility.Hidden;
        public Visibility MaximizeButton => Visible == MainWindowFunctions.Normalize ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NormalizeButton => Visible == MainWindowFunctions.Maximize ? Visibility.Visible : Visibility.Collapsed;
        public bool ComboEmpty { get; set; }
        #endregion
        public MainViewModel()
        {
            DBViewModel = new DBViewModel(this);
            SearchViewModel = new SearchViewModel(this);
            UpdateViewModel = new UpdateViewModel(this);
            ReadWrite = new ReadWrite(this);
            Test = new Test(this);
            Admin = new Admin(this);

            TestIt = new Command(() => AllTests(), () => CanClearBox("DelKeyword"));

            Clear_KeywordTextBox = new Command(() => ClearBox("DelKeyword"), () => CanClearBox("DelKeyword"));
            Clear_ImportTextBox = new Command(() => ClearBox("DelImport"), () => CanClearBox("DelImport"));
            Clear_Info = new Command(() => ClearBox("Clear_Info"), () => CanClearBox("Clear_Info"));
            Clear_DBComboBox = new Command(() => ClearBox("DBCombo"), () => CanClearBox("DBCombo"));
            Clear_screen = new Command(() => ClearBox("ScreenTextBox"), () => CanClearBox("ScreenTextBox"));
            Delete_Item = new Command(() => DeleteItem(), () => CanImportORDelete());
            Delete_DB = new Command(() => DeleteDB(), () => CanDeleteDB());
            ImportDB = new Command(() => DBViewModel.Import_DB(), () => DBViewModel.CanImport());
            ExportDB = new Command(() => DBViewModel.Export_DB(), () => DBViewModel.CanExportOptimizeDB());
            SearchItem = new Command(() => Search_Item(), () => CanSearch());
            SaveResult = new Command(() => ReadWrite.Save_Result(), () => ReadWrite.CanSave_Result());
            Correct_Item = new Command(() => ItemCorrect(), () => CanImportORDelete());
            CreateTestDB = new Command(() => Test.MakeTestDB(), () => Test.CanMakeTestDB());
            Show_DB = new Command(() => DBViewModel.ShowDB(), () => DBViewModel.CanShowDB());
            List_Items = new Command(() => DBViewModel.ListAllItems(), () => DBViewModel.CanListAllItems());
            Manual_Update = new Command(() => UpdateViewModel.ManualUpdate(), () => UpdateViewModel.CanDoManualUpdate());
            OptimizeDB = new Command(() => DBViewModel.Optimize_DB(), () => DBViewModel.CanExportOptimizeDB());
            LoginIt = new Command(() => Admin.LoginIn(), () => Admin.CanAdmin());
            Set_Password = new Command(() => Admin.SetPass(), () => Admin.CanAdmin());
            Backup_DB = new Command(() => DBViewModel.BackupDB(), () => CanDeleteDB());
            Restore_DB = new Command(() => DBViewModel.RestoreDB(), () => CanDeleteDB());
            Restore_DBs = new Command(() => DBViewModel.RestoreDBs(), () => CanDeleteDB());
            Exit = new Command(() => ExitApp(), () => CanExitApp());
        }
        #region Commands
        public ICommand TestIt { get; set; }
        public ICommand ImportDB { get; set; }
        public ICommand ExportDB { get; set; }
        public ICommand OptimizeDB { get; set; }
        public ICommand Show_DB { get; set; }
        public ICommand List_Items { get; set; }
        public ICommand Manual_Update { get; set; }
        public ICommand Backup_DB { get; set; }
        public ICommand Restore_DB { get; set; }
        public ICommand Restore_DBs { get; set; }
        public ICommand Delete_DB { get; set; }
        public ICommand Correct_Item { get; set; }
        public ICommand Delete_Item { get; set; }
        public ICommand SearchItem { get; set; }
        public ICommand SaveResult { get; set; }
        public ICommand Clear_KeywordTextBox { get; set; }
        public ICommand Clear_ImportTextBox { get; set; }
        public ICommand Clear_Info { get; set; }
        public ICommand Clear_DBComboBox { get; set; }
        public ICommand CreateTestDB { get; set; }
        public ICommand LoginIt { get; set; }
        public ICommand Set_Password { get; set; }
        public ICommand Clear_screen { get; set; }
        public ICommand MaximizeIt { get; set; }
        public ICommand NormalizeIt { get; set; }
        public ICommand MinimizeIt { get; set; }
        public ICommand CloseIt { get; set; }
        public ICommand Exit { get; set; }
        #endregion
        public void AllTests()
        {
            textRange.Text = "\n\n";
            textRange.Text += selectedText;
        }
        public void DeleteDB()
        {
            if (!String.IsNullOrEmpty(DBItem) && !String.IsNullOrWhiteSpace(DBItem))
            {
                if (File.Exists(@"DB\" + DBItem + ".bin"))
                {
                    if (File.Exists(@"DB\" + DBItem + ".bin")) { File.Delete(@"DB\" + DBItem + ".bin"); }
                    if (File.Exists(@"DB\" + DBItem + "Fields.bin")) { File.Delete(@"DB\" + DBItem + "Fields.bin"); }
                    if (DBsList.Count > 1)
                    {
                        List<string> DBsListCopy = new List<string>(DBsList);
                        DBsListCopy.Remove(DBItem);
                        ReadWrite.SaveList(DBsListCopy, "DBs");
                        if (DBsList.Contains("MyTelbook"))
                        {
                            DBItem = DBsList[DBsList.IndexOf("MyTelbook")];
                        }
                        else
                        {
                            DBItem = DBsList[0];
                        }
                    }
                    else if (DBsList.Count == 1)
                    {
                        File.Delete(@"DB\DBs.bin");
                        DBItem = string.Empty;
                    }
                    textRange.Text = "";
                }
            }
            else
            {
                DBItem = "Type a DB name";
            }
        }
        private bool CanDeleteDB()
        {
            bool a = false;
            if ((!String.IsNullOrEmpty(DBItem) || !String.IsNullOrWhiteSpace(DBItem)) && admin == true && Info1 == Admin.Password)
            {
                a = true;
            }
            return true;
        }
        private void ItemCorrect()
        {
            if (!String.IsNullOrEmpty(Import_DBname) && !String.IsNullOrWhiteSpace(Import_DBname))
            {
                ReadWrite.LoadDic(DBItem);
                if (new List<string>(DBViewModel.DB.Keys).Contains(Import_DBname))
                {
                    if (!textRange.Text.Contains("Correct item"))
                    {
                        string caption = "\nCorrect item:\n\n";
                        bool ShowKeys = true;
                        string textbody = "";
                        ReadWrite.LoadDic(DBItem);
                        if (!String.IsNullOrEmpty(Import_DBname) && !String.IsNullOrWhiteSpace(Import_DBname))
                        {
                            List<string> keyWord = new List<string>();
                            keyWord.Add(Import_DBname);
                            textbody = MakeResultsText(keyWord, ShowKeys);
                            ShowResults(caption, textbody);
                        }
                    }
                    else
                    {
                        UpdateViewModel.CorrectItem = true;
                        DBViewModel.Import_String(textRange.Text);
                        DBViewModel.Import_Txt();
                        textRange.Text = string.Empty;
                        ShowResults("\nCorrected item:\n\n", MakeResultsText(new List<string>() { Import_DBname }, false));
                    }
                }
                else
                {
                    Import_DBname = "Type the key of item to correct";
                }
            }
            else
            {
                Import_DBname = "Type the key of item to correct";
            }
        }
        public void DeleteItem()
        {
            if (!String.IsNullOrEmpty(Import_DBname) && !String.IsNullOrWhiteSpace(Import_DBname))
            {
                ReadWrite.LoadDic(DBItem);
                if (new List<string>(DBViewModel.DB.Keys).Contains(Import_DBname))
                {
                    Import_DBname = Import_DBname.Split('\n')[0].Split('\r')[0];
                    ReadWrite.LoadDic(DBItem);
                    List<string> KeyList = new List<string>(DBViewModel.DB.Keys);
                    if (KeyList.Contains(Import_DBname))
                    {
                        DBViewModel.DB.Remove(Import_DBname);
                        ReadWrite.SaveDic(DBViewModel.DB, @"DB\", DBItem);
                        Import_DBname = string.Empty;
                        DBViewModel.ListAllItems();
                    }
                    else
                    {
                        Info1 = "Type the key to delete correctly!";
                    }
                }
                else
                {
                    Import_DBname = "Type the key again";
                }
            }
            else
            {
                Import_DBname = "Type the key of item to delete";
            }
        }
        private bool CanImportORDelete()
        {
            bool youcan = false;
            if (!String.IsNullOrEmpty(Import_DBname) && !String.IsNullOrEmpty(DBItem))
            {
                youcan = true;
            }
            return true;
        }
        public void ClearBox(string name)
        {
            switch (name)
            {
                case "DelKeyword":
                    KeywordItem = string.Empty;
                    break;
                case "DelImport":
                    Import_DBname = string.Empty;
                    break;
                case "DBCombo":
                    DBItem = "";
                    break;
                case "Clear_Info":
                    Info1 = "";
                    break;
                case "ScreenTextBox":
                    textRange.Text = string.Empty;
                    break;
            }
        }
        public bool CanClearBox(string name)
        {
            bool permission = false;

            if (!String.IsNullOrEmpty(KeywordItem) && name == "DelKeyword")
            {
                permission = true;
            }
            else if (!String.IsNullOrEmpty(Import_DBname) && name == "DelImport")
            {
                permission = true;
            }
            else if (!String.IsNullOrEmpty(DBItem) && name == "DBCombo")
            {
                permission = true;
            }
            else if (!String.IsNullOrEmpty(DBItem) && name == "Clear_Info")
            {
                permission = true;
            }
            else if (!String.IsNullOrEmpty(textRange.Text) && !String.IsNullOrWhiteSpace(textRange.Text) && name == "ScreenTextBox")
            {
                permission = true;
            }
            return true;
        }
        public void Search_Item()
        {
            if (!String.IsNullOrEmpty(KeywordItem) && !String.IsNullOrWhiteSpace(KeywordItem) && DBsList.Contains(DBItem))
            {
                bool ShowKeys = false;
                string caption = string.Empty;
                string textbody = string.Empty;
                ReadWrite.LoadDic(DBItem);
                SearchViewModel.Search();
                textbody = MakeResultsText(SearchViewModel.searchResults, ShowKeys);
                if (SearchViewModel.searchResults.Count == 1)
                {
                    caption = "\nDB: " + DBItem + ", for keyword(s) '" + KeywordItem + "' found " + SearchViewModel.searchResults.Count.ToString() + " hit\n\n";
                }
                else if (SearchViewModel.searchResults.Count >1 && SearchViewModel.searchResults.Count <= 100)
                {
                    caption = "\nDB: " + DBItem + ", for keyword(s) '" + KeywordItem + "' found " + SearchViewModel.searchResults.Count.ToString() + " hits\n\n";
                }
                else if (SearchViewModel.searchResults.Count > 100)
                {
                    SearchViewModel.searchResults.RemoveRange(100, SearchViewModel.searchResults.Count - 100);
                    caption = "\nDB: " + DBItem + ", for keyword(s) '" + KeywordItem + "' found " + SearchViewModel.searchResults.Count.ToString() + " hits\n\n";
                }
                else if (SearchViewModel.searchResults.Count==0 || SearchViewModel.searchResults==null)
                {
                    caption = "\nDB: " + DBItem + ", for keyword(s) '" + KeywordItem + "' no hits found\n\n";
                    textbody = "\n";
                }
                ShowResults(caption, textbody);
            }
            else if (String.IsNullOrEmpty(KeywordItem) || String.IsNullOrWhiteSpace(KeywordItem))
            {
                KeywordItem = "Please type a keyword";
            }
        }
        public bool CanSearch()
        {
            bool youcan = false;
            if (!String.IsNullOrEmpty(KeywordItem) && DBsList.Contains(DBItem))
            {
                youcan = true;
            }
            return true;
        }
        public string MakeResultsText(List<string> listToPrint, bool ShowKeys)
        {
            string textbody = string.Empty;
            for (int i = 0; i < listToPrint.Count; i++)
            {
                if (ShowKeys == true)
                {
                    textbody += "Key: " + "".PadLeft(15 - "Key".Length - 3) + listToPrint[i] + "\n";
                }
                for (int j = 0; j <= 4; j++)
                {
                    for (int k = 0; k < DBViewModel.DB[listToPrint[i]][j].Count; k++)
                    {
                        if (!String.IsNullOrEmpty(DBViewModel.DB[listToPrint[i]][j][k]))
                        {
                            textbody += DBViewModel.Fields[j] + ": " + "".PadLeft(15 - DBViewModel.Fields[j].Length - 3) + DBViewModel.DB[listToPrint[i]][j][k] + "\n";
                        }
                    }
                }
                for (int k = 0; k < DBViewModel.DB[listToPrint[i]][5].Count; k++)
                {
                    for (int j = 5; j <= 7; j++)
                    {
                        if (!String.IsNullOrEmpty(DBViewModel.DB[listToPrint[i]][j][k]))
                        {
                            textbody += DBViewModel.Fields[j] + ": " + "".PadLeft(15 - DBViewModel.Fields[j].Length - 3) + DBViewModel.DB[listToPrint[i]][j][k] + "\n";
                        }
                    }
                }
                for (int j = 8; j <= 11; j++)
                {
                    for (int k = 0; k < DBViewModel.DB[listToPrint[i]][j].Count; k++)
                    {
                        if (!String.IsNullOrEmpty(DBViewModel.DB[listToPrint[i]][j][k]))
                        {
                            textbody += DBViewModel.Fields[j] + ": " + "".PadLeft(15 - DBViewModel.Fields[j].Length - 3) + DBViewModel.DB[listToPrint[i]][j][k] + "\n";
                        }
                    }
                }
                textbody += "\n";
            }
            return textbody;
        }
        public void ShowResults(string caption, string textbody)
        {
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                textRange.Text = caption + textbody;
                TextPointer start = textRange.Start.GetPositionAtOffset(textRange.Text.IndexOf(caption), LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(textRange.Text.IndexOf(caption) + caption.Length + 10, LogicalDirection.Backward);
                var textRange1 = new TextRange(start, end);
                textRange1.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }
        private void ExitApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private bool CanExitApp()
        {
            return true;
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