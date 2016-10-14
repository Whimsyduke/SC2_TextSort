using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CsvHelper;

namespace SC2_TextSort
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum OperationMode
        {
            ToCSV,
            ToTXT,
        }

        private OperationMode opMode = OperationMode.ToCSV;
        private List<TextInGalaxyCodeLine> galaxyCodeText;
        /// <summary>
        /// Galaxy脚本及包含Text列表
        /// </summary>
        public List<TextInGalaxyCodeLine> GalaxyCodeText
        {
            get
            {
                return galaxyCodeText;
            }

            set
            {
                galaxyCodeText = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            RadioButton_ToCsvFile.IsChecked = true;
        }

        /// <summary>
        /// 打开文件浏览器选择文件
        /// </summary>
        /// <param name="textBox">设置文件地址的TextBox控件</param>
        /// <param name="filter">文件类型筛选字符串</param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        FileInfo OpenFileDialogGetOpenFile(TextBox textBox, string filter, string title)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();  
            if (File.Exists(textBox.Text))
            {
                fileDialog.InitialDirectory = new FileInfo(textBox.Text).DirectoryName;
            }
            else
            {
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            fileDialog.Filter = filter;
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = fileDialog.FileName;
                return new FileInfo(fileDialog.FileName);
            }
            else
            {
                textBox.Text = "";
                return null;
            }
        }
        /// <summary>
         /// 保存文件选择路径
         /// </summary>
         /// <param name="textBox">设置文件地址的TextBox控件</param>
         /// <param name="filter">文件类型筛选字符串</param>
         /// <param name="title">标题</param>
         /// <returns></returns>
        FileInfo OpenFileDialogGetSavePath(TextBox textBox, string filter, string title)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            FileInfo saveFile = null;
            bool isPathOK = true;
            try
            {
                saveFile = new FileInfo(textBox.Text);
            }
            catch
            {
                isPathOK = false;
            }
            if (isPathOK)
            {
                fileDialog.InitialDirectory = saveFile.DirectoryName;
            }
            else
            {
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            fileDialog.Filter = filter;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = fileDialog.FileName;
                return new FileInfo(fileDialog.FileName);
            }
            else
            {
                textBox.Text = "";
                return null;
            }
        }
        
        /// <summary>
        /// 点击生成CSV模式
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void RadioButton_ToCsvFile_Checked(object sender, RoutedEventArgs e)
        {
            Grid_GalaxyPath.Visibility = Visibility.Visible;
            Grid_TextPath.Visibility = Visibility.Visible;
            Grid_InputPath.Visibility = Visibility.Collapsed;
            Grid_OutputPath.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 点击生成TXT模式
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void RadioButton_ToTxtFile_Checked(object sender, RoutedEventArgs e)
        {
            Grid_GalaxyPath.Visibility = Visibility.Collapsed;
            Grid_TextPath.Visibility = Visibility.Visible;
            Grid_InputPath.Visibility = Visibility.Visible;
            Grid_OutputPath.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 点击选择txt文件的路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectTextPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_TextPath, "Text File|*.txt", "Select Text File");
        }

        /// <summary>
        /// 点击选择Galaxy文件的路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectGalaxyPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_GalaxyPath, "Galaxy File|*.galaxy", "Select Galaxy File");
            this.Height = 224.5;
        }

        /// <summary>
        /// 点击选择输入路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectInputPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_InputPath, "CSV File|*.csv", "Select Input File");
        }

        /// <summary>
        /// 点击选择输出路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectOutputPath_Click(object sender, RoutedEventArgs e)
        {
            switch (opMode)
            {
                case OperationMode.ToCSV:
                    OpenFileDialogGetSavePath(TextBox_OutputPath, "CSV File|*.csv", "Select Output Path");
                    break;
                case OperationMode.ToTXT:
                    OpenFileDialogGetSavePath(TextBox_OutputPath, "Text File|*.txt", "Select Output Path");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 点击确认生成按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            char[] splitString = new char[2] { '\n', '\r' };
            List<TextInStringTxt> textListTemp = new List<TextInStringTxt>();
            List<TextInStringTxt> textList = new List<TextInStringTxt>();
            //try
            {
                StreamReader textStringReader = new StreamReader(TextBox_TextPath.Text);
                int i = 0;
                textListTemp.AddRange(textStringReader.ReadToEnd().Split(splitString).Where(r => r != "" && r.Contains("")).Select(r => new TextInStringTxt(i++, r)));
                textStringReader.Close();
                textList = textListTemp.Select(r => r.ReplaceSameTextToID(textListTemp)).ToList();
            }
            //catch (Exception error)
            //{
            //    MessageBox.Show("Fail with *String.txt file " + TextBox_TextPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
            //    return;
            //}

            List<TextInGalaxyCodeLine> galaxyTextList = new List<TextInGalaxyCodeLine>();
            if (opMode == OperationMode.ToCSV)
            {
                //try
                {
                    StreamReader galaxyCodeReader = new StreamReader(TextBox_GalaxyPath.Text);
                    int i = 0;
                    galaxyTextList.AddRange(galaxyCodeReader.ReadToEnd().Split(splitString).Where(r => r != "").Select(r => TextInGalaxyCodeLine.GetTextListByGalaxyLine(i++, r, textList)).Where(r => r != null));
                    galaxyCodeReader.Close();
                }
                //catch (Exception error)
                //{
                //    MessageBox.Show("Fail with Galaxy file " + TextBox_GalaxyPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                //    return;
                //}
            }
            if (opMode == OperationMode.ToTXT)
            {

            }
            //try
            {
                switch (opMode)
                {
                    case OperationMode.ToCSV:
                        StreamWriter csvSW = new StreamWriter(TextBox_OutputPath.Text, false, new System.Text.UTF8Encoding(true));
                        CsvWriter csvWriter = new CsvWriter(csvSW);
                        csvWriter.Configuration.Encoding = Encoding.UTF8;
                        foreach (TextInGalaxyCodeLine select in galaxyTextList)
                        {
                            select.CsvWrite(csvWriter);
                        }
                        csvSW.Close();
                        MessageBox.Show(TextBox_OutputPath.Text + " generation success.");
                        break;
                    case OperationMode.ToTXT:
                        break;
                    default:
                        break;
                }
            }
            //catch (Exception error)
            //{
            //    MessageBox.Show("Fail with output file " + TextBox_OutputPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
            //    return;
            //}
        }
    }
}
