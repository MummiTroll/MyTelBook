using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Documents;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace TelBook
{
    public class DBViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region Variables Properties Objects
        public Dictionary<string, List<List<string>>> DB = new Dictionary<string, List<List<string>>>();
        public List<string> fields = new List<string>() { "Surname", "Name", "Middle name", "Telephone", "Email", "Address", "City", "Country", "Birthday", "Skype", "Comment", "Homepage" };
        public List<string> Fields { get { return MainViewModel.ReadWrite.LoadList(MainViewModel.DBItem + "Fields"); } }
        public MainViewModel MainViewModel { get; set; }
        #endregion
        public DBViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public bool FileNotDir = false;
        public void Import_DB()
        {
            MainViewModel.Info1 = string.Empty;
            DB.Clear();
            if (String.IsNullOrEmpty(MainViewModel.selectedText) || String.IsNullOrWhiteSpace(MainViewModel.selectedText))
            {
                if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
                {
                    if (MainViewModel.Import_DBname.EndsWith("txt"))
                    {
                        string DBtext = string.Empty;
                        if (File.Exists(MainViewModel.Import_DBname))
                        {
                            DBtext = File.ReadAllText(MainViewModel.Import_DBname, Encoding.UTF8);
                        }
                        Import_String(DBtext);
                        Import_Txt();
                    }
                    else if (MainViewModel.Import_DBname.EndsWith("xls") || MainViewModel.Import_DBname.EndsWith("xlsx"))
                    {
                        Import_Xlsx();
                    }
                    else if (MainViewModel.Import_DBname.EndsWith("doc") || MainViewModel.Import_DBname.EndsWith("docx"))
                    {
                        Import_Docx();
                    }
                    else if (MainViewModel.Import_DBname.EndsWith("pdf"))
                    {
                        Import_Pdf();
                    }
                    else if (MainViewModel.Import_DBname.EndsWith("bin"))
                    {
                        Import_Bin();
                    }
                }
                else
                {
                    MainViewModel.Import_DBname = "Choose source file";
                }
            }
            else if (!String.IsNullOrEmpty(MainViewModel.selectedText) && !String.IsNullOrWhiteSpace(MainViewModel.selectedText))
            {
                if (MainViewModel.DBsList.Contains(MainViewModel.DBItem) || String.IsNullOrEmpty(MainViewModel.DBItem) || String.IsNullOrWhiteSpace(MainViewModel.DBItem))
                {
                    MainViewModel.DBItem = "Type new DB name!";
                }
                if ((!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem)) && !MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                {
                    string NewText = MakeNewText(MainViewModel.SelectedText);
                    Import_String(MakeNewText(NewText));
                    Import_Txt();
                }
            }
        }
        public string MakeNewText(string text)
        {
            List<string> splitList1 = new List<string>(text.Split('\n'));
            string NewText = string.Empty;

            foreach (string str1 in splitList1)
            {
                if (str1 != "" && str1 != " " && str1 != "\r" && !MainViewModel.UpdateViewModel.KillWhitespaces(str1).Substring(0, 3).Contains("Key"))
                {
                    NewText += str1 + "\n";
                }
            }
            return NewText;
        }
        public void Import_String(string DBtext)
        {
            DB.Clear();
            List<string> splitList = new List<string>();
            MainViewModel.UpdateViewModel.item = false;
            splitList = MainViewModel.UpdateViewModel.SplitString(DBtext);
            for (int i = 0; i < splitList.Count; i++)
            {
                if (!String.IsNullOrEmpty(splitList[i]) && !String.IsNullOrWhiteSpace(splitList[i]))
                {
                    for (int j = 0; j < MainViewModel.UpdateViewModel.TestForImport.Count; j++)
                    {
                        if (MainViewModel.UpdateViewModel.TestForImport[j].Contains(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[0].ToLower())) 
                            && !String.IsNullOrEmpty(splitList[i].Split(':')[1].ToLower()) && !String.IsNullOrWhiteSpace(splitList[i].Split(':')[1].ToLower()))
                        {
                            if (j == 0 && MainViewModel.UpdateViewModel.item == false)
                            {
                                MainViewModel.UpdateViewModel.item = true; MainViewModel.UpdateViewModel.Surname.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]));
                            }
                            else if (j == 0 && MainViewModel.UpdateViewModel.item == true)
                            {
                                MainViewModel.UpdateViewModel.NextItem();
                                MainViewModel.UpdateViewModel.Surname.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]));
                            }
                            else if (j == 1) { MainViewModel.UpdateViewModel.Name.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 2) { MainViewModel.UpdateViewModel.MiddleName.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 3)
                            {
                                string tel = MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]);
                                if (tel.Length > 2 && tel.Substring(0, 2) == "00")
                                {
                                    tel = "+" + tel.Substring(2);
                                    MainViewModel.UpdateViewModel.Telephone.Add(tel);
                                }
                                else if (tel.Length <= 2 || tel.Substring(0, 2) != "00")
                                {
                                    MainViewModel.UpdateViewModel.Telephone.Add(tel);
                                }
                            }
                            else if (j == 4) { MainViewModel.UpdateViewModel.Email.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 5)
                            {
                                if (MainViewModel.UpdateViewModel.Address.Count > MainViewModel.UpdateViewModel.City.Count) { MainViewModel.UpdateViewModel.City.Add(""); }
                                if (MainViewModel.UpdateViewModel.Address.Count > MainViewModel.UpdateViewModel.Country.Count) { MainViewModel.UpdateViewModel.Country.Add(""); }
                                MainViewModel.UpdateViewModel.Address.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]));
                            }
                            else if (j == 6)
                            {
                                if (MainViewModel.UpdateViewModel.City.Count >= MainViewModel.UpdateViewModel.Address.Count) { MainViewModel.UpdateViewModel.Address.Add(""); }
                                if (MainViewModel.UpdateViewModel.City.Count > MainViewModel.UpdateViewModel.Country.Count) { MainViewModel.UpdateViewModel.Country.Add(""); }
                                MainViewModel.UpdateViewModel.City.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]));
                            }
                            else if (j == 7)
                            {
                                if (MainViewModel.UpdateViewModel.Country.Count >= MainViewModel.UpdateViewModel.Address.Count) { MainViewModel.UpdateViewModel.Address.Add(""); }
                                if (MainViewModel.UpdateViewModel.Country.Count >= MainViewModel.UpdateViewModel.City.Count) { MainViewModel.UpdateViewModel.City.Add(""); }
                                MainViewModel.UpdateViewModel.Country.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1]));
                            }
                            else if (j == 8) { MainViewModel.UpdateViewModel.Birthday.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 9) { MainViewModel.UpdateViewModel.Skype.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 10) { MainViewModel.UpdateViewModel.VK.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                            else if (j == 11) { MainViewModel.UpdateViewModel.Homepage.Add(MainViewModel.UpdateViewModel.KillWhitespaces(splitList[i].Split(':')[1])); }
                        }
                    }
                }
            }
            MainViewModel.UpdateViewModel.NextItem();
        }
        public void Import_Txt()
        {
            if (MainViewModel.UpdateViewModel.CorrectItem == true)
            {
                if (MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                {
                    List<string> keyListCorrected = new List<string>(DB.Keys);
                    List<List<string>> ItemCorrected = new List<List<string>>(DB[keyListCorrected[0]]);
                    MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
                    DB.Remove(MainViewModel.Import_DBname);
                    DB[keyListCorrected[0]] = ItemCorrected;
                    MainViewModel.ReadWrite.SaveDic(DB, @"DB\", MainViewModel.DBItem);
                    ShowDB();
                }
                else
                {
                    MainViewModel.DBItem = "\n\nChoose a DB name from list";
                    MainViewModel.textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }
            else
            {
                if (!MainViewModel.DBsList.Contains(MainViewModel.DBItem) && !String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
                {
                    MessageBoxMenu("Save the imported data as a new DB?");
                    MainViewModel.Import_DBname = "New DB imported";
                    List<string> DBsListCopy = new List<string>(MainViewModel.DBsList);
                    DBsListCopy.Add(MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveDic(DB, @"DB\", MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveList(DBsListCopy, "DBs");
                    MainViewModel.ReadWrite.SaveList(MainViewModel.DBViewModel.fields, MainViewModel.DBItem + "Fields");
                }
                else if (MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                {
                    MessageBoxMenu("Save the imported data as '" + MainViewModel.DBItem + "'?");
                    MainViewModel.UpdateViewModel.MergeDics();
                }
            }
            ShowDB();
            MainViewModel.UpdateViewModel.CorrectItem = false;
        }
        public void Import_Xlsx()
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlWorkbook = xlApp.Workbooks.Open(MainViewModel.Import_DBname);
            string text = "";
            for (int i = 1; i <= xlWorkbook.Sheets.Count; i++)
            {
                Worksheet xlWorksheet = xlWorkbook.Sheets[i];
                Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;
                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                for (int j = 1; j <= rowCount; j++)
                {

                    int row = 0;
                    for (int k = 1; k <= colCount; k++)
                    {
                        if (xlRange.Cells[j, k] != null && xlRange.Cells[j, k].Value2 != null
                        && xlRange.Cells[j, k].Value.ToString() != ""
                        && xlRange.Cells[j, k].Value.ToString() != " ")
                        {
                            if (row == 0)
                            {
                                text += xlRange.Cells[j, k].Value2.ToString() + ":";
                                row += 1;
                            }
                            else
                            {
                                text += xlRange.Cells[j, k].Value2.ToString();
                                text += "\n";
                                break;
                            }
                        }
                    }
                }
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            Import_String(text);
            Import_Txt();
        }
        public void Import_Docx()
        {
            Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document document = application.Documents.Open(MainViewModel.Import_DBname);
            string text = string.Empty;
            int count = document.Words.Count;
            for (int i = 1; i <= count; i++)
            {
                text += document.Words[i].Text;
            }
            MainViewModel.textRange.Text = text;
            application.Quit();
            Marshal.ReleaseComObject(application);
            Import_String(text);
            Import_Txt();
        }
        public void Import_Pdf()
        {
            PdfReader reader = new PdfReader(MainViewModel.Import_DBname);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
            Import_String(text);
            Import_Txt();
        }
        public void Import_Bin()
        {
            Dictionary<string, List<List<string>>> textDic2D = new Dictionary<string, List<List<string>>>();
            List<string> DBsListCopy = new List<string>(MainViewModel.DBsList);
            if (File.Exists(MainViewModel.Import_DBname))
            {
                FileStream fs = new FileStream(MainViewModel.Import_DBname, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                textDic2D = (Dictionary<string, List<List<string>>>)formatter.Deserialize(fs); fs.Close();
            }
            if (textDic2D != null && textDic2D.Count != 0)
            {
                if (!MainViewModel.DBsList.Contains(MainViewModel.DBItem) && !String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
                {
                    MessageBoxMenu("Save the imported data as a new DB?");
                    MainViewModel.Import_DBname = "New DB imported";
                    DBsListCopy.Add(MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveDic(textDic2D, @"DB\", MainViewModel.DBItem);
                    MainViewModel.ReadWrite.SaveList(DBsListCopy, "DBs");
                    MainViewModel.ReadWrite.SaveList(MainViewModel.DBViewModel.fields, MainViewModel.DBItem + "Fields");
                    MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
                }
                else if (MainViewModel.DBsList.Contains(MainViewModel.DBItem))
                {
                    MessageBoxMenu("Save the imported data as '" + MainViewModel.DBItem + "'?");
                    DB = new Dictionary<string, List<List<string>>>(textDic2D);
                    MainViewModel.UpdateViewModel.MergeDics();
                }
                MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            }
            else
            {
                MainViewModel.textRange.Text = "\n\nThe DB is not in an accepted format!";
                MainViewModel.textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            ShowDB();
        }
        public void Export_DB()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                string filename = "DB_" + MainViewModel.DBItem + "_" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.ToString("HH.mm");
                if (MainViewModel.ExtensionItem.Contains("txt"))
                {
                    filename += ".txt";
                    Export_Txt(filename);
                }
                else if (MainViewModel.ExtensionItem.Contains("xls"))
                {
                    filename += ".xlsx";
                    Export_Xlsx(filename);
                }
                else if (MainViewModel.ExtensionItem.Contains("doc"))
                {
                    filename += ".docx";
                    Export_Docx(filename);
                }
                else if (MainViewModel.ExtensionItem.Contains("pdf"))
                {
                    filename += ".pdf";
                    Export_Pdf(filename);
                }
                else if (MainViewModel.ExtensionItem.Contains("bin"))
                {
                    filename += ".bin";
                    Export_Bin(filename);
                }
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public void Export_Txt(string filename)
        {
            bool ShowKeys = false;
            string path = string.Empty;

            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            List<string> keyList = new List<string>(DB.Keys);
            string text = MainViewModel.MakeResultsText(keyList, ShowKeys);
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MakePath(MainViewModel.Import_DBname);
                }
                else
                {
                    if (MainViewModel.Import_DBname[MainViewModel.Import_DBname.Length - 1] != '\\')
                    {
                        path = MainViewModel.Import_DBname + @"\";
                    }
                    else
                    {
                        path = MainViewModel.Import_DBname;
                    }
                }
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            else if (String.IsNullOrEmpty(MainViewModel.Import_DBname) || String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                path = root + @"_TelBook\DB\Export\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            MainViewModel.ReadWrite.SaveTxt(text, filename, path);
        }
        public void Export_Xlsx(string filename)
        {
            bool ShowKeys = false;
            string path = string.Empty;

            //Create path
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MakePath(MainViewModel.Import_DBname);
                }
                else
                {
                    if (MainViewModel.Import_DBname[MainViewModel.Import_DBname.Length - 1] != '\\')
                    {
                        path = MainViewModel.Import_DBname + @"\";
                    }
                    else
                    {
                        path = MainViewModel.Import_DBname;
                    }
                }
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            else if (String.IsNullOrEmpty(MainViewModel.Import_DBname) || String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                path = root + @"_TelBook\DB\Export\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Open the DB
            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            List<string> keyList = new List<string>(DB.Keys);

            //Open an Excel application and a workbook
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                return;
            }
            Workbook xlWorkbook = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet xlWorksheet = xlWorkbook.Sheets[1];

            //Fill the workbook with the DB data
            int RowCount = 1;
            for (int i = 0; i < keyList.Count(); i++)
            {
                for (int j = 0; j < DB[keyList[i]].Count(); j++)
                {
                    for (int k = 0; k < DB[keyList[i]][j].Count(); k++)
                    {
                        if (!String.IsNullOrEmpty(DB[keyList[i]][j][k]) && !String.IsNullOrWhiteSpace(DB[keyList[i]][j][k]))
                        {
                            xlWorksheet.Cells[RowCount, 1] = Fields[j];
                            xlWorksheet.Cells[RowCount, 2] = "'" + DB[keyList[i]][j][k];
                            RowCount += 1;
                        }
                    }
                }
                RowCount += 1;
            }
            //##### Save
            xlWorkbook.SaveAs(path + filename, XlFileFormat.xlOpenXMLWorkbook);
            xlWorkbook.Close();
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            MainViewModel.Import_DBname = path + filename;
        }
        public void Export_Docx(string filename)
        {
            bool ShowKeys = false;
            string path = string.Empty;
            string text = string.Empty;

            //Create path 
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MakePath(MainViewModel.Import_DBname);
                }
                else
                {
                    if (MainViewModel.Import_DBname[MainViewModel.Import_DBname.Length - 1] != '\\')
                    {
                        path = MainViewModel.Import_DBname + @"\";
                    }
                    else
                    {
                        path = MainViewModel.Import_DBname;
                    }
                }
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            else if (String.IsNullOrEmpty(MainViewModel.Import_DBname) || String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                path = root + @"_TelBook\DB\Export\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Open the DB
            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            List<string> keyList = new List<string>(DB.Keys);

            //Make a docx file.
            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document document = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            //Filling the file with text
            string textbody = MainViewModel.MakeResultsText(keyList, ShowKeys);
            //adding text to document  
            document.Content.Font.Name = "Courier";
            document.Content.Font.Size = 10;
            document.Content.Text = textbody;

            // Save
            document.SaveAs(path + filename);
            application.ActiveDocument.Close();
            application.Quit();
            Marshal.ReleaseComObject(application);
            MainViewModel.Import_DBname = path + filename;
        }
        public void Export_Pdf(string filename)
        {
            bool ShowKeys = false;
            string path = string.Empty;

            //Open the DB
            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            List<string> keyList = new List<string>(DB.Keys);

            //Create the DB text
            string text = MainViewModel.MakeResultsText(keyList, ShowKeys);

            //Create path 
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MakePath(MainViewModel.Import_DBname);
                }
                else
                {
                    if (MainViewModel.Import_DBname[MainViewModel.Import_DBname.Length - 1] != '\\')
                    {
                        path = MainViewModel.Import_DBname + @"\";
                    }
                    else
                    {
                        path = MainViewModel.Import_DBname;
                    }
                }
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            else if (String.IsNullOrEmpty(MainViewModel.Import_DBname) || String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                path = root + @"_TelBook\DB\Export\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Add text to document and save 
            using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 25, 25))
            {
                using (var writer = PdfWriter.GetInstance(doc, new FileStream(path + filename, FileMode.Create)))
                {
                    doc.Open();
                    var parFont = FontFactory.GetFont("Courier", 10.0f, BaseColor.BLACK);
                    var paragraph = new iTextSharp.text.Paragraph(text, parFont);
                    doc.Add(paragraph);
                    doc.Close();
                    writer.Close();
                }
            }
            MainViewModel.Import_DBname = path + filename;
        }
        public void Export_Bin(string filename)
        {
            bool ShowKeys = false;
            string path = string.Empty;

            //Create path 
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MakePath(MainViewModel.Import_DBname);
                }
                else
                {
                    if (MainViewModel.Import_DBname[MainViewModel.Import_DBname.Length - 1] != '\\')
                    {
                        path = MainViewModel.Import_DBname + @"\";
                    }
                    else
                    {
                        path = MainViewModel.Import_DBname;
                    }
                }
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            else if (String.IsNullOrEmpty(MainViewModel.Import_DBname) || String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                path = root + @"_TelBook\DB\Export\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Open DB 
            MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
            List<string> keyList = new List<string>(DB.Keys);

            //Save DB
            var binaryFormatter = new BinaryFormatter(); var fi = new FileInfo(path + filename);
            using (var binaryFile = fi.Create())
            { binaryFormatter.Serialize(binaryFile, DB); binaryFile.Flush(); }
            MainViewModel.Import_DBname = path + filename;
        }
        public void ListAllItems()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                if (File.Exists(@"DB\" + MainViewModel.DBItem + ".bin"))
                {
                    MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
                    List<string> keyList = new List<string>(DB.Keys);
                    string caption = "\nDB: " + MainViewModel.DBItem + ", " + keyList.Count() + " items\n\n";
                    string textbody = "";
                    keyList.Sort();
                    for (int i = 0; i < keyList.Count; i++)
                    {
                        if (keyList[i] != "")
                        {
                            textbody += keyList[i] + "\n";
                        }
                    }
                    textbody += "\n";
                    MainViewModel.ShowResults(caption, textbody);
                }
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public bool CanListAllItems()
        {
            bool canList = false;
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem) && MainViewModel.DBsList.Contains(MainViewModel.DBItem))
            {
                canList = true;
            }
            return true;
        }
        public void ShowDB()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem) && MainViewModel.DBsList.Contains(MainViewModel.DBItem))
            {
                bool ShowKeys = true;
                string caption = "";
                MainViewModel.ReadWrite.LoadDic(MainViewModel.DBItem);
                List<string> DBkeyList = new List<string>(DB.Keys);
                if (DBkeyList.Count == 1)
                {
                    caption = "\nDB: " + MainViewModel.DBItem + " (" + DB.Count.ToString() + " entry)\n\n";
                }
                else if (DBkeyList.Count > 1 && DBkeyList.Count <= 100)
                {
                    caption = "\nDB: " + MainViewModel.DBItem + " (" + DB.Count.ToString() + " entries)\n\n";
                }
                else if (DBkeyList.Count > 100)
                {
                    DBkeyList.RemoveRange(100, DBkeyList.Count - 100);
                    caption = "\nDB: " + MainViewModel.DBItem + " (" + DB.Count.ToString() + " entries, 100 as example)\n\n";
                }
                string textbody = MainViewModel.MakeResultsText(DBkeyList, ShowKeys);
                MainViewModel.ShowResults(caption, textbody);
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public void BackupDB()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                if (File.Exists(@"DB\" + MainViewModel.DBItem + ".bin"))
                {
                    if (!Directory.Exists(@"Backup")) { Directory.CreateDirectory(@"Backup"); }
                    if (!Directory.Exists(@"Backup\Older")) { Directory.CreateDirectory(@"Backup\Older"); }
                    if (File.Exists(@"Backup\" + MainViewModel.DBItem + ".bin") && File.Exists(@"Backup\" + MainViewModel.DBItem + "Fields.bin"))
                    {
                        File.Move(@"Backup\" + MainViewModel.DBItem + ".bin", @"Backup\Older\" + MainViewModel.DBItem + "_" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.ToString("HH.mm") + ".bin");
                        File.Move(@"Backup\" + MainViewModel.DBItem + "Fields.bin", @"Backup\Older\" + MainViewModel.DBItem + "_" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.ToString("HH.mm") + "_Fields.bin");
                    }
                    if (File.Exists(@"DB\" + MainViewModel.DBItem + ".bin") && File.Exists(@"DB\" + MainViewModel.DBItem + "Fields.bin"))
                    {
                        File.Copy(@"DB\" + MainViewModel.DBItem + ".bin", @"Backup\" + MainViewModel.DBItem + ".bin");
                        File.Copy(@"DB\" + MainViewModel.DBItem + "Fields.bin", @"Backup\" + MainViewModel.DBItem + "Fields.bin");
                    }
                }
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public void RestoreDB()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                if (File.Exists(@"DB\" + MainViewModel.DBItem + ".bin") && File.Exists(@"DB\" + MainViewModel.DBItem + "Fields.bin"))
                {
                    if (File.Exists(@"Backup\" + MainViewModel.DBItem + ".bin") && File.Exists(@"Backup\" + MainViewModel.DBItem + "Fields.bin"))
                    {
                        File.Copy(@"Backup\" + MainViewModel.DBItem + ".bin", @"DB\" + MainViewModel.DBItem + ".bin", true);
                        File.Copy(@"Backup\" + MainViewModel.DBItem + "Fields.bin", @"DB\" + MainViewModel.DBItem + "Fields.bin", true);
                    }
                    else
                    {
                        MainViewModel.DBItem = "No backup for this DB";
                    }
                }
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public void RestoreDBs()
        {
            for (int i = 0; i < MainViewModel.DBsList.Count; i++)
            {
                if (File.Exists(@"Backup\" + MainViewModel.DBsList[i] + ".bin") && File.Exists(@"Backup\" + MainViewModel.DBsList[i] + "Fields.bin"))
                {
                    File.Copy(@"Backup\" + MainViewModel.DBsList[i] + ".bin", @"DB\" + MainViewModel.DBsList[i] + ".bin", true);
                    File.Copy(@"Backup\" + MainViewModel.DBsList[i] + "Fields.bin", @"DB\" + MainViewModel.DBsList[i] + "Fields.bin", true);
                }
                else
                {
                    string caption = "\n\nNo backup for the DB " + "\"" + MainViewModel.DBsList[i] + "\"" + " available";
                    string warning = "\n\n";
                    MainViewModel.ShowResults(caption, warning);
                }
            }
        }
        public void Optimize_DB()
        {
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                MainViewModel.UpdateViewModel.MergeDics();
                string caption = "\n\nOptimization of the DB " + MainViewModel.DBItem + " successful";
                string warning = "\n\n";
                MainViewModel.ShowResults(caption, warning);
            }
            else
            {
                MainViewModel.DBItem = "Type a DB name";
            }
        }
        public bool FileOrNot(string path)
        {
            bool IsFile = false;
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    IsFile = false;
                }
                else if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    IsFile = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException || ex is FileNotFoundException)
                {
                    return false;
                }
                throw;
            }
            return IsFile;
        }
        public string MakePath(string path)
        {
            if (FileOrNot(path))
            {
                string[] split = path.Split('\\');
                path = split.Take(split.Count() - 1).ToArray().Aggregate((string a, string b) => a + @"\" + b) + @"\";
            }
            else
            {
                if (path.Substring(path.Length - 1) != @"\")
                {
                    path = path + @"\";
                }
            }
            return path;
        }
        private void MessageBoxMenu(string message)
        {
            MessageBoxResult result = MessageBox.Show("", message, MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    return;
            }
        }
        public bool CanShowDB()
        {
            bool canShow = false;
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem) && MainViewModel.DBsList.Contains(MainViewModel.DBItem))
            {
                canShow = true;
            }
            return true;
        }
        public bool CanImport()
        {
            bool youCan = false;
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                youCan = true;
            }
            return true;
        }
        public bool CanExportOptimizeDB()
        {
            bool youcan = false;
            if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrEmpty(MainViewModel.DBItem))
            {
                youcan = true;
            }
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
