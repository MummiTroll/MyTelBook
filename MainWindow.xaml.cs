using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TelBook.Enums;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Data;

namespace TelBook
{
    public partial class MainWindow : System.Windows.Window
    {
        MainViewModel mainViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
            mainViewModel.MaximizeIt = new Command(() => ReshapeWindow(MainWindowFunctions.Maximize), () => YouCan());
            mainViewModel.NormalizeIt = new Command(() => ReshapeWindow(MainWindowFunctions.Normalize), () => YouCan());
            mainViewModel.MinimizeIt = new Command(() => ReshapeWindow(MainWindowFunctions.Minimize), () => YouCan());
            mainViewModel.CloseIt = new Command(() => ReshapeWindow(MainWindowFunctions.CloseWin), () => YouCan());
            this.MouseDown += delegate { DragMove(); };
            mainViewModel.textRange = new TextRange(ScreenTextBox.Document.ContentStart, ScreenTextBox.Document.ContentEnd);
            mainViewModel.textRange.Text = string.Empty;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Hits.ItemsSource = mainViewModel.HitsCombo;
            Hits.SelectedItem = mainViewModel.HitsCombo[3];
            Extensions.ItemsSource = mainViewModel.Extensions;
            Extensions.SelectedItem = mainViewModel.Extensions[0];
            mainViewModel.Info = "";
            DBCombo.Text = "";
            mainViewModel.DBItem = "";
            if (File.Exists(@"DB\DBs.bin"))
            {
                mainViewModel.ReadWrite.LoadList("DBs");
                DBCombo.ItemsSource = mainViewModel.DBsList;
                mainViewModel.DBItem = mainViewModel.DBsList[0];
                DBCombo.SelectedItem = mainViewModel.DBsList[0];
                DBCombo.Text = mainViewModel.DBsList[0];
            }
            else
            {
                DBCombo.Text = "No databanks available!";
            }
            string caption = "\n\nThe following files are missing, please, restore:";
            string warning = "\n\n";
            foreach (string db in mainViewModel.DBsList)
            {
                if (!File.Exists(@"DB\" + db + ".bin"))
                {
                    warning += db + ".bin\n";
                }
                if (!File.Exists(@"DB\" + db + "Fields.bin"))
                {
                    warning += db + "Fields.bin\n";
                }
            }
            if (warning.Length > 2)
            {
                mainViewModel.ShowResults(caption, warning);
            }
        }
        //public void InitializeCombo(int nameIndex)
        //{
        //    Hits.ItemsSource = mainViewModel.HitsCombo;
        //    Hits.SelectedItem = mainViewModel.HitsCombo[3];
        //    Extensions.ItemsSource = mainViewModel.Extensions;
        //    Extensions.SelectedItem = mainViewModel.Extensions[0];
        //    mainViewModel.Info = "";
        //    DBCombo.Text = "";
        //    mainViewModel.DBItem = "";
        //    if (File.Exists(@"DB\DBs.bin"))
        //    {
        //        mainViewModel.ReadWrite.LoadList("DBs");
        //        DBCombo.ItemsSource = mainViewModel.DBsList;
        //        mainViewModel.DBItem = mainViewModel.DBsList[nameIndex];
        //        DBCombo.SelectedItem = mainViewModel.DBsList[nameIndex];
        //        DBCombo.Text = mainViewModel.DBsList[nameIndex];
        //    }
        //    else
        //    {
        //        DBCombo.Text = "No databanks available!";
        //    }
        //}
        private void TxtChanged(object sender, RoutedEventArgs e)
        {
            ManualUpdate.IsEnabled = true;
            CorrectItem.IsEnabled = true;
        }
        private void TextBoxDoubleClick(object obj, EventArgs e)
        {
            mainViewModel.SelectedText = string.Empty;
            TextBox tb = obj as TextBox;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                tb.Text = filename;
            }
        }
        private void Combo_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var textbox = (TextBox)DBCombo.Template.FindName("PART_EditableTextBox", DBCombo);
            //if (textbox != null)
            //{
            //    var parent = (Border)textbox.Parent;
            //    var converter = new BrushConverter();
            //    parent.Background = (Brush)converter.ConvertFromString("#F8F8FF");
            //}
        }
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(mainViewModel.DBItem))
            {
                if (DBCombo.SelectedItem != null)
                {
                    DBCombo.Text = DBCombo.SelectedItem.ToString();
                }
                else
                {
                    DBCombo.Text = "";
                }
            }
        }
        public void MousePreviewDown(object sender, MouseEventArgs e)
        {
            string root = System.IO.Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ImportDB_.Text = root + @"Tmp\";
        }
        private void ScreenTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            mainViewModel.SelectedText = ScreenTextBox.Selection.Text;
            if (mainViewModel.SelectedText != "")
            {
                mainViewModel.Info1 = "Save as DB? Hit 'Import DB'";
            }
        }
        public void ReshapeWindow(MainWindowFunctions mode)
        {
            switch (mode)
            {
                case MainWindowFunctions.Maximize:
                    this.WindowState = WindowState.Maximized;
                    mainViewModel.Visible = mode;
                    break;
                case MainWindowFunctions.Normalize:
                    this.WindowState = WindowState.Normal;
                    mainViewModel.Visible = mode;
                    break;
                case MainWindowFunctions.Minimize:
                    this.WindowState = WindowState.Minimized;
                    break;
                case MainWindowFunctions.CloseWin:
                    System.Windows.Application.Current.Shutdown();
                    break;
                default:
                    throw new NotImplementedException(string.Format($"{mode.ToString()} not implemented"));
            }
        }
        private bool YouCan()
        {
            return true;
        }
    }
}