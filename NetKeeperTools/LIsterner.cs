using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetKeeperTools
{
    internal class Listerner
    {
        public static string card;

        public static string message;

        private static AutoResetEvent signal;

        public static void main()
        {
            Listerner.signal = new AutoResetEvent(false);
            EventLog expr_10 = new EventLog();
            expr_10.Source = "RasClient";
            expr_10.EntryWritten += new EntryWrittenEventHandler(Listerner.EntryWritten);
            expr_10.EnableRaisingEvents = true;
            Listerner.signal.WaitOne();
        }

        public static void EntryWritten(object source, EntryWrittenEventArgs e)
        {
            if (e.Entry.InstanceId == 20221L)
            {
                string expr_1E = e.Entry.Message;
                int start = expr_1E.IndexOf("User = ");
                int length = expr_1E.IndexOf("VpnStrategy");
                Listerner.card = expr_1E.Substring(start + 8, length - start - 9);
            }
            Listerner.signal.Set();
        }
    }
}
