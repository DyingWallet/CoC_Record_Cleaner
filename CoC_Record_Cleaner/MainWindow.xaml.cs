using FileTools;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;

namespace CoC_Record_Cleaner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileHandler handler;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "文本文件|*.txt";
            if (!(bool)openFileDialog.ShowDialog())
            {
                return;
            }

            handler = new FileHandler(openFileDialog.FileName);
            if (!handler.OpenFile())
            {
                MessageBox.Show("打开文件失败...");
                return;
            }
            int i = 0;
            string temp = "";
            List<string> examples = new List<string>();

            foreach (string line in handler.contents)
            {
                if (i++ >= 100)
                {
                    temp += "...";
                    break;
                }
                examples.Add(line);
                temp += line;
            }
            //更新文本框
            this.OriginalText.Text = temp;
            this.totLine.Text = handler.total_lines.ToString();
            this.PriviewText.Text = handler.preView(examples);
        }

        private void Extract(object sender, RoutedEventArgs e)
        {
            if (handler == null)
            {
                MessageBox.Show("请先打开文本文件！", "注意！");
                return;
            }
            if (handler.CleanAndWriteToFile())
            {
                MessageBox.Show("文件生成完成！", "注意！");
                System.Diagnostics.Process.Start("Explorer", "/select," + handler.fpath + "Cleaned_" + handler.fname);
            }
            else
                MessageBox.Show("似乎出错了！", "Error");
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
