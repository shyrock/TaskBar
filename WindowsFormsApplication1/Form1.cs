using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private MultiCastClient Client;
        private MultiCastServer Server;
        private Command Cmd;
        private Thread RcvCmdThread;
        private bool ShouldClose = false;
        private System.Timers.Timer Timer2 = new System.Timers.Timer();

        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        public static int WM_CLIPBOARDUPDATE = 0x031D; 
        protected override void DefWndProc(ref Message m)  
        {  
            if (m.Msg == WM_CLIPBOARDUPDATE)  
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (Clipboard.GetText().Substring(0, 10) == "$$DFWOUT$$")
                        {
                            if (ClipboardManager.Recover())
                            {
                                Timer2.Stop();
                                Console.WriteLine("Timer2 stop");
                            }
                            break;
                        }
                    }
                    catch (ExternalException e)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    catch(ArgumentOutOfRangeException e)
                    {
                        
                    }
                    catch
                    {
                        throw;
                    }
                }
            }  
            else  
            {  
                base.DefWndProc(ref m);  
            }  
        }  
        static private string IPFromName(string Name)
        {
            IPHostEntry Entry = Dns.GetHostEntry(Name);
            int V4count = 0;
            string IP="";
            foreach (IPAddress adr in Entry.AddressList)
            {
                if (adr.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = adr.ToString();
                    V4count++;
                }
            }
            if (V4count == 1)
            {
                return IP;
            }
            else
            {
                return "Mulitple IP Address!";
            }
        }
        public Form1()
        {
            InitializeComponent();
            if (!AddClipboardFormatListener(this.Handle))
            {
                MessageBox.Show("Add failed");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Object selected = listBox1.SelectedItem;
            if (selected == null)
                return;
            else
            {
                if (!Cmd.SendCommand(Command.typeClipboard, IPFromName(selected.ToString())))
                {
                    MessageBox.Show("Failed to send command!");
                    return;
                }
            }
            this.WindowState = FormWindowState.Minimized;
        }
        private void Receive()
        {
            while (true)
            {
                string Received = null;
                if (Cmd.ReceiveCommand(out Received))
                {
                    if(Received.IsCB())
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (ClipboardManager.Set(Received.GetClipboard()))
                            {
                                Timer2.Elapsed += timer1_Tick;
                                Timer2.Interval = 10;
                                Timer2.Start();
                                Console.WriteLine("Timer2 start, interval{0}", timer1.Interval);
                                break;
                            }
                        }
                    }
                    else if (Received.IsCL())
                    {
                        try
                        {
                            Received.ExecuteCmdLine();
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            break;
                        }
                    }
                    //MessageBox.Show("Set!!!!!!!!!");
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Get PC list
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
            Configuration.ReadConfig("config.txt");
            foreach (Configuration.ConfigItem it in Configuration.cfgItems)
            {
                listBox1.Items.Add(it.Name);
            }
            //Start getting command
            Cmd = new Command();

            //Set working directory
            RunAtStartup.Enable(Application.ExecutablePath);
            RcvCmdThread = new Thread(new ThreadStart(Receive));
            RcvCmdThread.SetApartmentState(ApartmentState.STA);
            RcvCmdThread.IsBackground = true;
            RcvCmdThread.Start();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            Object selected = listBox1.SelectedItem;
            if (selected == null)
                return;
            else
            {
                if (!Cmd.SendCommand(Command.typeCommandLine, IPFromName(selected.ToString())))
                {
                    MessageBox.Show("Failed to send command!");
                    return;
                }
            }
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Clipboard.GetText());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("testxxxfa");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //WIN-SI2DKDAE8KN
            IPHostEntry Entry = Dns.GetHostEntry("CD-RD-NJG");
            for (int i = 0; i < Entry.AddressList.Length; i++)
            {
                MessageBox.Show(Entry.AddressList[i].ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Configuration.ReadConfig("config.txt");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!RemoveClipboardFormatListener(this.Handle))
            {
                MessageBox.Show("Remove failed");
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {

            if (FormWindowState.Minimized == this.WindowState)
            {
                //mynotifyicon.Visible = true;
                mynotifyicon.ShowBalloonTip(5000);
                this.Hide();
            }
            
            else if (FormWindowState.Normal == this.WindowState)
            {
                //mynotifyicon.Visible = false;
            }
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            label2.Hide();
            button2.Show();

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            ShouldClose = true;
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ShouldClose)
            {
                return;
            }
            else
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //线程中设置剪贴板不成功。
            Timer2.Stop();
            Console.WriteLine("Timer2 Tick");
            //Reset clipboard
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Console.WriteLine("Timer2: Reset Clipboard");
                    if(ClipboardManager.Recover())
                        break;
                }
                catch (ExternalException ex)
                {
                    System.Threading.Thread.Sleep(10);
                }
                catch (ArgumentOutOfRangeException ex)
                {

                }
                catch
                {
                    throw;
                }
            }
        }
    }

    
    public class Config
    {
        public static string MulticastIP = "233.1.2.3";
        public static int MulticastPort = 10001;
        public static int SndCommandPort = 10023;
        public static int RcvCommandPort = 10024;
        public static string CommandCfgFile = "command.txt";
        public static string CommandLineFile = "commandline.txt";
    }
}
