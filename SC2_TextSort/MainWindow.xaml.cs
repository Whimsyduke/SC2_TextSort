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

namespace SC2_TextSort
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
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
        /// 点击选择Galaxy文件的路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectGalaxyPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_GalaxyPath, "Galaxy File|*.galaxy", "Select Galaxy File");
        }

        /// <summary>
        /// 点击选择txt文件的路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectOutputPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogGetOpenFile(TextBox_TextPath, "Text File|*.txt|CSV File|*.csv", "Select Test File");
        }

        /// <summary>
        /// 点击选择输出路径按钮
        /// </summary>
        /// <param name="sender">想用控件</param>
        /// <param name="e">响应事件</param>
        private void Button_SelectOutputPath_Click_1(object sender, RoutedEventArgs e)
        {

            OpenFileDialogGetSavePath(TextBox_OutputPath, "Text File|*.txt|CSV File|*.csv", "Select Save Path");
        }
    }
}
