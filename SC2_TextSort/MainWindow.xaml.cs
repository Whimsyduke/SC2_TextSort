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
            ToRefresh,
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
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void RadioButton_ToCsvFile_Checked(object sender, RoutedEventArgs e)
        {
            Grid_GalaxyPath.Visibility = Visibility.Visible;
            Grid_TextPath.Visibility = Visibility.Visible;
            Grid_InputPath.Visibility = Visibility.Collapsed;
            Grid_PatchPath.Visibility = Visibility.Visible;
            Grid_OutputPath.Visibility = Visibility.Visible;
            CheckBox_KeepZH_CN.IsEnabled = false;
            CheckBox_RefreshZH_CN.IsEnabled = false;
            CheckBox_KeepZHId_CN.IsEnabled = false;
            TextBox_OutputPath.Text = "";
            opMode = OperationMode.ToCSV;
        }

        /// <summary>
        /// 点击生成TXT模式
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void RadioButton_ToTxtFile_Checked(object sender, RoutedEventArgs e)
        {
            Grid_GalaxyPath.Visibility = Visibility.Collapsed;
            Grid_TextPath.Visibility = Visibility.Visible;
            Grid_InputPath.Visibility = Visibility.Visible;
            Grid_PatchPath.Visibility = Visibility.Collapsed;
            Grid_OutputPath.Visibility = Visibility.Visible;
            CheckBox_KeepZH_CN.IsEnabled = true;
            if (CheckBox_KeepZH_CN.IsChecked == true)
            {
                CheckBox_RefreshZH_CN.IsEnabled = true;
                CheckBox_KeepZHId_CN.IsEnabled = true;
            }
            else
            {
                CheckBox_RefreshZH_CN.IsEnabled = false;
                CheckBox_KeepZHId_CN.IsEnabled = false;
            }
            TextBox_OutputPath.Text = "";
            opMode = OperationMode.ToTXT;
        }

        /// <summary>
        /// 点击刷新中文模式
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void RadioButton_ToRefreshFile_Click(object sender, RoutedEventArgs e)
        {
            Grid_GalaxyPath.Visibility = Visibility.Collapsed;
            Grid_TextPath.Visibility = Visibility.Visible;
            Grid_InputPath.Visibility = Visibility.Visible;
            Grid_PatchPath.Visibility = Visibility.Collapsed;
            Grid_OutputPath.Visibility = Visibility.Visible;
            CheckBox_KeepZH_CN.IsEnabled = false;
            CheckBox_RefreshZH_CN.IsEnabled = false;
            CheckBox_KeepZHId_CN.IsEnabled = false;
            TextBox_OutputPath.Text = "";
            opMode = OperationMode.ToRefresh;
        }

        /// <summary>
        /// 点击选择txt文件的路径按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectTextPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_TextPath, "Text File|*.txt", "Select Text File");
        }

        /// <summary>
        /// 点击选择Galaxy文件的路径按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectGalaxyPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_GalaxyPath, "Galaxy File|*.galaxy", "Select Galaxy File");
            this.Height = 224.5;
        }

        /// <summary>
        /// 点击选择输入路径按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectInputPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_InputPath, "CSV File|*.csv", "Select Input File");
        }

        /// <summary>
        /// 点击选择补丁路径按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_PatchPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_PatchPath, "CSV File|*.csv", "Select Patch File");
        }

        /// <summary>
        /// 点击选择输出路径按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectOutputPath_Click(object sender, RoutedEventArgs e)
        {
            switch (opMode)
            {
                case OperationMode.ToCSV:
                    OpenFileDialogGetSavePath(TextBox_OutputPath, "CSV File|*.csv", "Select Output Path");
                    break;
                case OperationMode.ToTXT:
                case OperationMode.ToRefresh:
                    OpenFileDialogGetSavePath(TextBox_OutputPath, "Text File|*.txt", "Select Output Path");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 点击确认生成按钮
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            char[] splitString = new char[2] { '\n', '\r' };
            List<TextInStringTxt> textList = new List<TextInStringTxt>();
            List<TextInStringTxt> patchList = new List<TextInStringTxt>();
            try
            {
                StreamReader textStringReader = new StreamReader(TextBox_TextPath.Text);
                int i = 0;
                textList.AddRange(textStringReader.ReadToEnd().Split(splitString).Where(r => r != "" && r.Contains("")).Select(r => new TextInStringTxt(i++, r)));
                textStringReader.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("Fail with *String.txt file " + TextBox_TextPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                return;
            }
            List<TextInGalaxyCodeLine> galaxyTextList = new List<TextInGalaxyCodeLine>();
            if (opMode == OperationMode.ToCSV)
            {
                //分析补丁CSV
                if (File.Exists(TextBox_PatchPath.Text))
                {
                    try
                    {
                        StreamReader csvSR = new StreamReader(TextBox_PatchPath.Text, new System.Text.UTF8Encoding(true));
                        CsvReader csvReader = new CsvReader(csvSR);
                        while (csvReader.Read())
                        {
                            string id = csvReader.GetField<string>("TextId");
                            string en_US = csvReader.GetField<string>("EN-US");
                            string zh_CN = csvReader.GetField<string>("NoRepeatZH-CN");
                            bool isRepeat = csvReader.GetField<string>("RepeatTextInZH-CN") != "";
                            TextInStringTxt text = new TextInStringTxt(id, zh_CN, en_US);
                            if (isRepeat)
                            {
                                var originTextAll = patchList.Where(r => r.Id == zh_CN);
                                if (originTextAll.Count() == 0)
                                {
                                    if (MessageBox.Show("在" + TextBox_PatchPath.Text + "找不到ID为" + zh_CN + "的文本记录，作为ID为" + id + "的文本记录的同内容重复原始文本，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                    {
                                        return;
                                    }
                                }
                                TextInStringTxt originText = originTextAll.First();
                                text.ZH_CN = originText.ZH_CN;
                                text.EN_US = originText.EN_US;
                            }
                            else
                            {
                                text.ZH_CN = zh_CN;
                                text.EN_US = en_US;
                            }
                            patchList.Add(text);
                            var patchTextAll = textList.Where(r => r.Id == text.Id);
                            if (patchTextAll.Count() != 0)
                            {
                                TextInStringTxt patchText = patchTextAll.First();
                                patchText.EN_US = text.EN_US;
                                if (patchText.ZH_CN != text.ZH_CN)
                                {
                                    patchText.Patch_CN = text.ZH_CN;
                                }
                            }
                        }
                        csvSR.Close();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Fail with CSV file " + TextBox_GalaxyPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                        return;
                    }
                }
                //分析Galaxy
                try
                {
                    StreamReader galaxyCodeReader = new StreamReader(TextBox_GalaxyPath.Text);
                    int i = 0;
                    galaxyTextList.AddRange(galaxyCodeReader.ReadToEnd().Split(splitString).Where(r => r != "").Select(r => TextInGalaxyCodeLine.GetTextListByGalaxyLine(i++, r, textList)).Where(r => r != null));
                    galaxyCodeReader.Close();
                }
                catch (Exception error)
                {
                    MessageBox.Show("Fail with Galaxy file " + TextBox_GalaxyPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                    return;
                }
            }
            if (opMode == OperationMode.ToTXT)
            {
                try
                {
                    StreamReader csvSR = new StreamReader(TextBox_InputPath.Text, new System.Text.UTF8Encoding(true));
                    CsvReader csvReader = new CsvReader(csvSR);
                    while (csvReader.Read())
                    {
                        string id = csvReader.GetField<string>("TextId");
                        string en_US = csvReader.GetField<string>("EN-US");
                        string zh_CN = csvReader.GetField<string>("NoRepeatZH-CN");
                        bool isRepeat = csvReader.GetField<string>("RepeatTextInZH-CN") != "";
                        var texts = textList.Where(r => r.Id == id);
                        if (texts.Count() <= 0)
                        {
                            if (MessageBox.Show("在" + TextBox_TextPath.Text + "找不到ID为" + id + "的文本记录!，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        TextInStringTxt text = texts.First();
                        if (text.HaveEN_US) continue;
                        if (isRepeat)
                        {
                            var originTextAll = textList.Where(r => r.Id == zh_CN);
                            if (originTextAll.Count() == 0)
                            {
                                if (MessageBox.Show("在" + TextBox_TextPath.Text + "找不到ID为" + zh_CN + "的文本记录，作为ID为" + id + "的文本记录的同内容重复原始文本，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            TextInStringTxt originText = originTextAll.First();
                            if (originText.HaveEN_US == false)
                            {
                                if (MessageBox.Show("在" + TextBox_InputPath.Text + "中不存在ID为" + zh_CN + "的文本记录，它是ID为" + id + "的文本记录的重复原始文本，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            if (CheckBox_KeepZH_CN.IsChecked == true && CheckBox_RefreshZH_CN.IsChecked == true)
                            {
                                text.ZH_CN = originText.ZH_CN;
                            }
                            text.EN_US = originText.EN_US;
                        }
                        else
                        {
                            if (CheckBox_KeepZH_CN.IsChecked == true && CheckBox_RefreshZH_CN.IsChecked == true)
                            {
                                text.ZH_CN = zh_CN;
                            }
                            text.EN_US = en_US;
                            text.HaveEN_US = true;
                        }
                    }
                    csvSR.Close();
                }
                catch (Exception error)
                {
                    MessageBox.Show("Fail with CSV file " + TextBox_GalaxyPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                    return;
                }
            }
            if (opMode == OperationMode.ToRefresh)
            {
                try
                {
                    StreamReader csvSR = new StreamReader(TextBox_InputPath.Text, new System.Text.UTF8Encoding(true));
                    CsvReader csvReader = new CsvReader(csvSR);
                    while (csvReader.Read())
                    {
                        string id = csvReader.GetField<string>("TextId");
                        string zh_CN = csvReader.GetField<string>("NoRepeatZH-CN");
                        bool isRepeat = csvReader.GetField<string>("RepeatTextInZH-CN") != "";
                        var texts = textList.Where(r => r.Id == id);
                        if (texts.Count() <= 0)
                        {
                            if (MessageBox.Show("在" + TextBox_TextPath.Text + "找不到ID为" + id + "的文本记录!，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        TextInStringTxt text = texts.First();
                        if (isRepeat)
                        {
                            var originTextAll = textList.Where(r => r.Id == zh_CN);
                            if (originTextAll.Count() == 0)
                            {
                                if (MessageBox.Show("在" + TextBox_TextPath.Text + "找不到ID为" + zh_CN + "的文本记录，作为ID为" + id + "的文本记录的同内容重复原始文本，是否继续？", "错误", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            TextInStringTxt originText = textList.Where(r => r.Id == zh_CN).First();
                            text.ZH_CN = originText.ZH_CN;
                        }
                        else
                        {
                            text.ZH_CN = zh_CN;
                        }
                    }
                    csvSR.Close();
                }
                catch (Exception error)
                {
                    MessageBox.Show("Fail with CSV file " + TextBox_GalaxyPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                    return;
                }
            }
            try
            {
                switch (opMode)
                {
                    case OperationMode.ToCSV:
                        StreamWriter csvSW = new StreamWriter(TextBox_OutputPath.Text, false, new System.Text.UTF8Encoding(true));
                        CsvWriter csvWriter = new CsvWriter(csvSW);
                        csvWriter.WriteField("GalaxyLine");
                        csvWriter.WriteField("GalaxyCode");
                        csvWriter.WriteField("GalaxyTextIndex");
                        csvWriter.WriteField("TextId");
                        csvWriter.WriteField("TextInGalaxyCount");
                        csvWriter.WriteField("FirstLineNumberInCsv");
                        csvWriter.WriteField("RepeatTextInZH-CN");
                        csvWriter.WriteField("PatchTextInZH-CN");
                        csvWriter.WriteField("NoRepeatZH-CN");
                        csvWriter.WriteField("EN-US");
                        csvWriter.NextRecord();
                        csvWriter.Configuration.Encoding = Encoding.UTF8;
                        int writeLine = 2;
                        foreach (TextInGalaxyCodeLine select in galaxyTextList)
                        {
                            select.CsvWrite(csvWriter, textList, ref writeLine);
                        }
                        csvSW.Close();
                        MessageBox.Show(TextBox_OutputPath.Text + " generation success.");
                        break;
                    case OperationMode.ToTXT:
                        StreamWriter txtWriter = new StreamWriter(TextBox_OutputPath.Text, false, new System.Text.UTF8Encoding(true));
                        foreach (TextInStringTxt select in textList)
                        {
                            select.WriteTxt(txtWriter, CheckBox_KeepZH_CN.IsChecked == true, CheckBox_KeepZHId_CN.IsChecked == true);
                        }
                        txtWriter.Close();
                        MessageBox.Show(TextBox_OutputPath.Text + " generation success.");
                        break;
                    case OperationMode.ToRefresh:
                        StreamWriter refreshWriter = new StreamWriter(TextBox_OutputPath.Text, false, new System.Text.UTF8Encoding(true));
                        foreach (TextInStringTxt select in textList)
                        {
                            select.WriteTxt(refreshWriter);
                        }
                        refreshWriter.Close();
                        MessageBox.Show(TextBox_OutputPath.Text + " generation success.");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Fail with output file " + TextBox_OutputPath.Text + ".\r\nError message is:" + error.Message, "Text File Error!", MessageBoxButton.OK);
                return;
            }
        }

        /// <summary>
        /// 保留中文选择True
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void CheckBox_KeepZH_CN_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox_RefreshZH_CN.IsEnabled = true;
            CheckBox_KeepZHId_CN.IsEnabled = true;
        }

        /// <summary>
        /// 保留中文选择False
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">响应事件</param>
        private void CheckBox_KeepZH_CN_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox_RefreshZH_CN.IsEnabled = false;
            CheckBox_KeepZHId_CN.IsEnabled = false;
        }
    }
}
