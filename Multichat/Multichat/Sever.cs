using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multichat
{
    public partial class Sever : Form
    {
        public Sever()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private string Specialtegn = ConstClass.Specialtegn;
        private string EndArray = ConstClass.EndArray;

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Socket item in clientList)
            {
                Send(item);               
            }
            AddMessage(txbMessage.Text);
            txbMessage.Clear();
        }

        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;

        void Connect()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 9797);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            server.Bind(IP);

            Thread listen = new Thread(() => {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9797);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
            });
            listen.IsBackground = true;
            listen.Start();
        }

        void Send(Socket client)
        {
            if (txbMessage.Text != string.Empty && client != null)
                client.Send(Encoding.UTF8.GetBytes(txbMessage.Text));
        }

        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[ConstClass.DivArraySend];
                    client.Receive(data);

                    string message = Encoding.UTF8.GetString(data);

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                            item.Send(data);
                    }
                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }
        }

        void AddMessage(string s)
        {
            lsvView.Items.Add(new ListViewItem() { Text = s });
        }

        new void Close()
        {
            server.Close();
        }

        private void Sever_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ipSend = "127.0.0.1";
            string specialtegn = "%%%%";
            string ipReceive = "127.0.0.2";

            byte[] bipSend = Encoding.UTF8.GetBytes(ipSend);
            byte[] bspecialtegn = Encoding.UTF8.GetBytes(specialtegn);
            byte[] bipReceive = Encoding.UTF8.GetBytes(ipReceive);
            byte[] bmessage = Encoding.UTF8.GetBytes(txbMessage.Text);

            List<byte> data = new List<byte>();
            data.AddRange(bipSend);
            data.AddRange(bspecialtegn);
            data.AddRange(bipReceive);
            data.AddRange(bspecialtegn);
            data.AddRange(bmessage);
            data.AddRange(bspecialtegn);
            data.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, });

            byte[] bdata = data.ToArray();
            int ipS = 0, ipR = 0, end = 0;
            int count = 0;

            List<byte> data2 = bdata.ToList();

            for (int i = 0; i < data2.Count - bspecialtegn.Length; i++)
            {
                string temp = Encoding.UTF8.GetString(data2.GetRange(i, bspecialtegn.Length).ToArray());
                if (temp == specialtegn)
                {
                    count++;
                }
                if (count == 1 && ipS == 0)
                    ipS = i;
                if (count == 2 && ipR == 0)
                {
                    ipR = i;
                    break;
                }
            }

            for (int i = data2.Count - 1; i > bspecialtegn.Length; i--)
            {
                if (data2[i] == 0)
                {
                    continue;
                }
                else
                {
                    if (Encoding.UTF8.GetString(data2.GetRange(i - bspecialtegn.Length + 1, bspecialtegn.Length).ToArray()) == specialtegn)
                        end = i;

                    break;
                }
            }

            MessageBox.Show(Encoding.UTF8.GetString(data2.GetRange(0, ipS).ToArray()));
            MessageBox.Show(Encoding.UTF8.GetString(data2.GetRange(ipS + bspecialtegn.Length, ipR - (ipS + bspecialtegn.Length)).ToArray()));
            MessageBox.Show(Encoding.UTF8.GetString(data2.GetRange(ipR + bspecialtegn.Length, end + 1 - ipR - 2 * bspecialtegn.Length).ToArray()));
        }

        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        object DeSerialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }
    }
}
