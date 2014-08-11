using System.Net;
using System.Net.Sockets;
using System.Windows;
using System;
using WindowsFormsApplication1;
using System.Windows.Forms;
using System.Text;

namespace WindowsFormsApplication1
{
    class MultiCastServer
    {
        private IPAddress multicastIP = IPAddress.Parse(Config.MulticastIP);
        private int port = Config.MulticastPort;

        public bool TryConnect()
        {
            Console.WriteLine("Reciever Start");
            UdpClient receiveUdp = new UdpClient(this.port);
            try
            {
                receiveUdp.JoinMulticastGroup(this.multicastIP, 10);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message.ToString());
                return false;
            }
            finally
            {
                receiveUdp.Close();
            }
            return true;
        }

        public void SendData()
        {
            Console.WriteLine("Sender Start");
            IPEndPoint multicastIep = new IPEndPoint(multicastIP, port);
            UdpClient sendUdpClient = new UdpClient();

            sendUdpClient.EnableBroadcast = true;

            string sendString = "How   are   you";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sendString);

            try
            {
                sendUdpClient.Send(bytes, bytes.Length, multicastIep);
            }
            catch
            {
                MessageBox.Show("send error");
                Console.WriteLine("send error");
            }
            finally
            {
                sendUdpClient.Close();
                MessageBox.Show("send close");
                Console.WriteLine("Sender Close");
            }
        }
    }

    class MultiCastClient
    {

        private IPAddress multicastIP = IPAddress.Parse(Config.MulticastIP);
        private int port = Config.MulticastPort;
        public bool TryConnect()
        {
            Console.WriteLine("Reciever Start");
            UdpClient receiveUdp = new UdpClient(this.port);
            try
            {
                receiveUdp.JoinMulticastGroup(this.multicastIP, 10);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message.ToString());
                return false;
            }
            finally
            {
                receiveUdp.Close();
            }
            return true;
        }
        public void ReceiveData()
        {
            Console.WriteLine("Reciever Start");
            UdpClient receiveUdp = new UdpClient(this.port);
            try
            {
                receiveUdp.JoinMulticastGroup(this.multicastIP, 10);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message.ToString());
            }

            IPEndPoint remoteHost = null;

            while (true)
            {
                try
                {
                    byte[] bytes = receiveUdp.Receive(ref remoteHost);
                    string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    Console.WriteLine(str);
                }
                catch
                {
                    Console.WriteLine("Reciever Close");
                    break;
                }
            }
        }
    }

    class Command
    {
        public static string typeClipboard = "CB";
        public static string typeCommandLine = "CL";

        public bool SendCommand(string type, string toIP)
        {
            UdpClient Client = new UdpClient(Config.SndCommandPort);
            string cmd = null;
            try
            {
                if (type == typeClipboard)
                {
                    cmd = CommandCfg.GetClipboard();
                }
                else if (type == typeCommandLine)
                {
                    cmd = CommandCfg.GetCommand();
                }
                if (cmd == null)
                    return false;
                cmd = type + cmd;
                byte[] bytes = Encoding.UTF8.GetBytes(cmd);
                if (bytes.Length != Client.Send(bytes, bytes.Length, toIP, Config.RcvCommandPort))
                {
                    return false;
                }
            }
            catch (SocketException e)
            {
                return false;
            }
            finally
            {
                Client.Close();
            }
            return true;
        }
        public bool ReceiveCommand(out string RcvString)
        {
                UdpClient Client = new UdpClient(Config.RcvCommandPort);
                try
                {
                    IPEndPoint remoteHost = null;
                    byte[] bytes = Client.Receive(ref remoteHost);
                    RcvString = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    RcvString = null;
                    return false;
                }
                finally
                {
                    Client.Close();
                }

            return true;
        }
    }
}