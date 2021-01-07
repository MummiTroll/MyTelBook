using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace TelBook
{
    public class ReadWrite
    {
        public MainViewModel MainViewModel { get; set; }
        public ReadWrite(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }
        public void Save_Result()
        {
            MainViewModel.Info = "";
            if (MainViewModel.textRange.Text.Contains("DB:") && MainViewModel.textRange.Text.Contains("for keyword(s)")
                && MainViewModel.textRange.Text.Contains("found"))
            {
                string text = MainViewModel.textRange.Text;
                if (!String.IsNullOrEmpty(MainViewModel.DBItem) && !String.IsNullOrWhiteSpace(MainViewModel.DBItem))
                {
                    string fileName = "Search_results_DB_" + MainViewModel.DBItem + "_" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.ToString("HH.mm");
                    if (MainViewModel.ExtensionItem.Contains("txt"))
                    {
                        fileName += ".txt";
                        SaveResultsTxt(text.Substring(2), fileName);
                    }
                    if (MainViewModel.ExtensionItem.Contains("xls"))
                    {
                        fileName += ".xlsx";
                        SaveResultsXlsx(text.Substring(2), fileName);
                    }
                    if (MainViewModel.ExtensionItem.Contains("doc"))
                    {
                        fileName += ".docx";
                        SaveResultsDocx(text.Substring(2), fileName);
                    }
                    if (MainViewModel.ExtensionItem.Contains("pdf"))
                    {
                        fileName += ".pdf";
                        SaveResultsPdf(text.Substring(2), fileName);
                    }
                }
                else
                {
                    MainViewModel.textRange.Text = "\nPlease, make a search!";
                    MainViewModel.textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }
            else if (String.IsNullOrEmpty(MainViewModel.DBItem) || String.IsNullOrWhiteSpace(MainViewModel.DBItem))
            {
                MainViewModel.DBItem = "Please, choose a DB";
            }
        }
        private void SaveResultsTxt(string searchResults, string fileName)
        {
            string path = string.Empty;

            //Make the path
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (MainViewModel.DBViewModel.FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MainViewModel.DBViewModel.MakePath(MainViewModel.Import_DBname);
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
                path = root + @"_TelBook\DB\Search_Results\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            SaveTxt(searchResults, fileName, path);
        }
        public void SaveTxt(string content, string fileName, string path)
        {
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrWhiteSpace(path))
            {
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                //File.WriteAllText(@"DB\" + name, content, Encoding.UTF7);
                File.WriteAllText(path + fileName, content, Encoding.UTF8);
                //File.WriteAllText(@"DB\" + name, content);
            }
        }
        private void SaveResultsXlsx(string searchResults, string fileName)
        {
            string path = string.Empty;

            //Create a path
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (MainViewModel.DBViewModel.FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MainViewModel.DBViewModel.MakePath(MainViewModel.Import_DBname);
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
                path = root + @"_TelBook\DB\Search_Results\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Create a workbook
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlWorkbook = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet xlWorksheet = xlWorkbook.Sheets[1];

            //Fill the workbook with the DB data
            var range = xlWorksheet.Range["A1:H1"];
            range.Merge();
            xlWorksheet.Cells[1, 1] = MainViewModel.UpdateViewModel.KillWhitespaces(searchResults.Split('\n')[0]);
            searchResults = searchResults.Substring(searchResults.Split('\n')[0].Length - 1, searchResults.Length - searchResults.Split('\n')[0].Length - 1);
            MainViewModel.DBViewModel.Import_String(searchResults);
            List<string> keyList = new List<string>(MainViewModel.DBViewModel.DB.Keys);
            int RowCount = 3;
            for (int i = 0; i < keyList.Count(); i++)
            {
                for (int j = 0; j < MainViewModel.DBViewModel.DB[keyList[i]].Count(); j++)
                {
                    for (int k = 0; k < MainViewModel.DBViewModel.DB[keyList[i]][j].Count(); k++)
                    {
                        if (!String.IsNullOrEmpty(MainViewModel.DBViewModel.DB[keyList[i]][j][k]) && !String.IsNullOrWhiteSpace(MainViewModel.DBViewModel.DB[keyList[i]][j][k]))
                        {
                            xlWorksheet.Cells[RowCount, 1] = MainViewModel.DBViewModel.Fields[j];
                            xlWorksheet.Cells[RowCount, 2] = "'" + MainViewModel.DBViewModel.DB[keyList[i]][j][k];
                            RowCount += 1;
                        }
                    }
                }
                RowCount += 1;
            }
            //##### Save
            xlWorkbook.SaveAs(path + fileName, XlFileFormat.xlOpenXMLWorkbook);
            xlWorkbook.Close();
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
        private void SaveResultsDocx(string searchResults, string fileName)
        {
            string path = string.Empty;

            //Create path
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (MainViewModel.DBViewModel.FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MainViewModel.DBViewModel.MakePath(MainViewModel.Import_DBname);
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
                path = root + @"_TelBook\DB\Search_Results\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Make a docx file.
            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document document = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            //Adding text to document  
            document.Content.Font.Name = "Courier";
            document.Content.Font.Size = 10;
            document.Content.Text = searchResults;

            //Save .docx file
            document.SaveAs(path + fileName);
            application.ActiveDocument.Close();
            application.Quit();
            Marshal.ReleaseComObject(application);
        }
        private void SaveResultsPdf(string searchResults, string fileName)
        {
            string path = string.Empty;

            //Create path
            if (!String.IsNullOrEmpty(MainViewModel.Import_DBname) && !String.IsNullOrWhiteSpace(MainViewModel.Import_DBname))
            {
                if (MainViewModel.DBViewModel.FileOrNot(MainViewModel.Import_DBname) == true)
                {
                    path = MainViewModel.DBViewModel.MakePath(MainViewModel.Import_DBname);
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
                path = root + @"_TelBook\DB\Search_Results\";
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }

            //Add text to document and save 
            using (var doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 25, 25))
            {
                using (var writer = PdfWriter.GetInstance(doc, new FileStream(path + fileName, FileMode.Create)))
                {
                    doc.Open();
                    var parFont = FontFactory.GetFont("Courier", 10.0f, BaseColor.BLACK);
                    var paragraph = new iTextSharp.text.Paragraph(searchResults, parFont);
                    doc.Add(paragraph);
                    doc.Close();
                    writer.Close();
                }
            }
        }
        public void LoadDic(string DBname)
        {
            MainViewModel.DBViewModel.DB.Clear();
            if (Directory.Exists("DB"))
            {
                if (File.Exists(@"DB\" + DBname + ".bin"))
                {
                    FileStream fs = new FileStream(@"DB\" + DBname + ".bin", FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    MainViewModel.DBViewModel.DB = (Dictionary<string, List<List<string>>>)formatter.Deserialize(fs); fs.Close();
                }
            }
        }
        public void LoadDicPath(string path)
        {
            MainViewModel.DBViewModel.DB.Clear();

            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                MainViewModel.DBViewModel.DB = (Dictionary<string, List<List<string>>>)formatter.Deserialize(fs); fs.Close();
            }
        }
        public void SaveDic(Dictionary<string, List<List<string>>> dic, string path, string DBname)
        {
            if (!Directory.Exists(path)) { Directory.CreateDirectory("DB"); }
            if (Directory.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter(); var fi = new FileInfo(path + DBname + ".bin");
                using (var binaryFile = fi.Create())
                { binaryFormatter.Serialize(binaryFile, dic); binaryFile.Flush(); }
            }

            Debug.WriteLine("complete path: " + path + DBname + ".bin");
        }
        public List<string> LoadList(string fileName)
        {
            List<string> list = new List<string>();
            if (Directory.Exists("DB"))
            {
                if (File.Exists(@"DB\" + fileName + ".bin"))
                {
                    FileStream fs = new FileStream(@"DB\" + fileName + ".bin", FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    list = (List<string>)formatter.Deserialize(fs); fs.Close();
                }
            }
            return list;
        }
        public void SaveList(List<string> list, string listName)
        {
            if (!Directory.Exists("DB")) { Directory.CreateDirectory("DB"); }
            if (Directory.Exists("DB"))
            {
                var binaryFormatter = new BinaryFormatter(); var fi = new FileInfo(@"DB\" + listName + ".bin");
                using (var binaryFile = fi.Create())
                { binaryFormatter.Serialize(binaryFile, list); binaryFile.Flush(); }
            }
        }
        public string ReadTxt(string fileName)
        {
            string line = string.Empty;
            if (File.Exists(@"DB\" + fileName))
            {
                line = File.ReadAllText(@"DB\" + fileName, Encoding.UTF8);
                //line = File.ReadAllText(@"DB\" + fileName, Encoding.UTF7);
                //line = File.ReadAllText(@"DB\" + fileName);
            }
            return line;
        }
        public bool CanSave_Result()
        {
            bool a = false;
            if (MainViewModel.SearchViewModel.searchResults.Count() != 0)
            {
                a = true;
            }
            else
            {
                a = false;
            }
            return true;
        }
    }
}
