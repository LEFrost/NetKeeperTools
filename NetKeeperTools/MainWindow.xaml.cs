using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Windows.Shapes;
using DotRas;
using System.Net;

namespace NetKeeperTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        bool RunCmd(string cmdExe, string cmdStr = null)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();

                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }
        public MainWindow()
        {
            InitializeComponent();
            //cmdMethod(@"NetKeeper.exe");
        }

        private static async Task cmdMethod(string path)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
            myProcess.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            myProcess.StartInfo.RedirectStandardOutput = false;  //由调用程序获取输出信息
            myProcess.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            myProcess.StartInfo.CreateNoWindow = true;          //显示
            myProcess.Start();
            myProcess.StandardInput.WriteLine(path);
            myProcess.StandardInput.AutoFlush = true;
            myProcess.WaitForExit();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Text == string.Empty)
                MessageBox.Show("请先输入密码");
            else
            {
                Task.Run(()=> cmdMethod(@"C:&cd C:\Program Files (x86)\NetKeeper\&NetKeeper.exe"));

                await bhAsync();
                Task.WaitAll();
            }
        }

        private async Task bhAsync()
        {
            
            Listerner.main();
            string account = string.Empty;
            bool hasAccount = false;
            hasAccount = await Task.Run(() => getAccount(ref account));

            Message.Text += "获取的账号是：" + account + "\n";
            string password = Password.Text;
            account = "\r" + account;
            Dail("Netkeeper", account, password);
        }

        private  bool getAccount(ref string account)
        {
            bool result = false;
            while (!result)
            {
                account = Listerner.card;
                if (account != null)
                {
                    result = true;
                   bool isKill= KillNK();
                    if (!isKill)
                        Message.Text += "毒瘤没被杀死,请从试一次\n";

                }
            }
            return result;
        }
        private bool KillNK()
        {

            bool result = false;
            Process[] process = Process.GetProcesses();
            foreach (var item in process)
            {
                Console.WriteLine(item.ProcessName);
                if (item.ProcessName.Count() >= 36)
                    if (item.ProcessName.Contains('-'))
                    {
                        IReadOnlyCollection<RasConnection> rasList = RasConnection.GetActiveConnections();
                        foreach (var conn in rasList)
                            conn.HangUp();
                        item.Kill();
                        result = true;
                    }

            }
            return result;
        }
        private void Dail(string PPPOEname, string username, string password)
        {
            try
            {
                using (RasDialer dialer = new RasDialer())
                {
                    dialer.EntryName = PPPOEname;
                    dialer.AllowUseStoredCredentials = true;
                    dialer.Timeout = 1000;
                    dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                    dialer.Credentials = new NetworkCredential(username, password);
                    dialer.Dial();
                }
                Message.Text += "连接成功\n";
            }
            catch (RasException)
            {
                Message.Text += "请重试!!!\n";
            }
        }

    }
}
