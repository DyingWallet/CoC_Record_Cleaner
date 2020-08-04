using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileTools
{
    public class FileHandler
    {
        #region 变量声明
        public string url { get; private set; }
        //文件路径
        public string fpath { get; private set; }
        //文件名
        public string fname { get; private set; }

        //文件流
        public FileStream fs { get; set; }
        //IO流
        public StreamReader sr { get; set; }
        public StreamWriter sw { get; set; }
        
        //计数器
        public int total_lines { get; private set; }

        //list用于存储有效发言
        public List<string> contents { get; private set; }
        //用于文本匹配
        //匹配发言者记录（id yyyy/MM/dd HH:mm:ss）
        private Regex regex_comments = new Regex(@"^\s*?[（|(].*([）|)|\w]|[\W]|([)|）][(|（]))$");
        //private Regex regex_comments = new Regex(@"^\s*?[（|(].*([）|)|\w]||([)|）][(|（]))$");
        //查找空行
        private Regex regex_space = new Regex(@"^\s*?$");
        //匹配文本行
        private Regex regex_charactor = new Regex(@"^.*?\s\d{4}(/\d{2}){2}\s(\d{2}:){2}\d{2}$");
        //用于自动获取文件名
        private Regex regex_fname = new Regex(@"\w*[.]\w*$");
        private Regex regex_fpath = new Regex(@"^\w:(\w*[\\])*");

        #endregion
        public FileHandler(string url)
        {
            this.url = url;
        }

        //匹配场外发言
        //private string regex_comments = @" ^\s{4}（.*）$";
        //匹配发言者记录（id yyyy/MM/dd HH:mm:ss）
        //private string regex_charactor = @"^.*?\s\d{4}(/\d{2}){2}\s(\d{2}:){2}\d{2}$";

        //将文件读入内存
        public bool OpenFile()
        {
            //打开文件流
            fs = new FileStream(url, FileMode.Open, FileAccess.Read);
            //开启文件读取流
            sr = new StreamReader(fs, Encoding.UTF8);
            //获取文件名
            Match getFname = regex_fname.Match(url);
            Match getFpath = regex_fpath.Match(url);
            if (getFname.Success) { fname = getFname.Value; }
            if (getFpath.Success) { fpath = getFpath.Value; }
            string temp = "";
            total_lines = 0;
            contents = new List<string>();

            while ((temp = sr.ReadLine()) != null)
            {
                total_lines++;
                contents.Add(temp + "\n");
            }
            sr.Close();
            fs.Close();
            if (total_lines == 0)
                return false;
            return true;
        }

        ///从文本中筛选有效发言
        public bool CleanAndWriteToFile()
        {
            fs = new FileStream(fpath + "Cleaned_" + fname, FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fs, Encoding.UTF8);
            string characotr = "";
            foreach(string line in contents)
            {
                if (regex_charactor.IsMatch(line))
                    characotr = line;
                else if(!regex_comments.IsMatch(line) && !regex_space.IsMatch(line))
                {
                    sw.Write(characotr + line);
                }
            }

            sw.Close();
            fs.Close();
            return true;
        }

        //生成预览
        public string preView(List<string> examples)
        {
            string temp = "";
            string charactor = "";

            foreach (string example in examples)
            {
                if (regex_charactor.IsMatch(example))
                    charactor = example;
                else if (!regex_comments.IsMatch(example) && !regex_space.IsMatch(example))
                {
                    temp += charactor + example;
                }
            }
            return temp;
        }
    }

}
