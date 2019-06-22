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
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //Connect();

            comboBox1.Items.AddRange(IPAddressTest.ToArray());
            txbMyIP.Text = "127.0.0.1";
        }

        List<string> IPAddressTest = new List<string>()
        {
            "127.0.0.1",
            "127.0.0.2",
            "127.0.0.3"
        };

        private string Specialtegn = ConstClass.Specialtegn;
        private string EndArray = ConstClass.EndArray;

        private void button1_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage("You: " + txbMessage.Text);
        }

        IPEndPoint IP;
        Socket client;

        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse(txbMyIP.Text), 9797);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Error Connection");
                return;
            }

            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        int divArray = ConstClass.DivArraySend;

        void Send()
        {
            //if (txbMessage.Text != string.Empty)
            //    client.Send(Serialize(DataSend(txbMessage.Text)));

            if (txbMessage.Text != string.Empty)
            {
                byte[] dataSend = Encoding.UTF8.GetBytes(txbMessage.Text);
                byte[] data = DataSend(dataSend);
                int count = data.Length / divArray;

                for (int i = 0; i <= count; i++)
                {
                    if (i == count)
                    {
                        client.Send(data.ToList().GetRange(i * divArray, data.Length - i * divArray).ToArray());
                    }
                    else
                    {
                        client.Send(data.ToList().GetRange(i * divArray, divArray).ToArray());
                    }
                }
            }

        }

        List<byte> BigData = new List<byte>();
        List<FileSendReceive> ListFile = new List<FileSendReceive>();
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[divArray];
                    client.Receive(data);
                    BigData.AddRange(data);

                    if (!CheckEndSend(BigData))
                    {
                        continue;
                    }
                    else
                    {
                        int s, r, sg, eg;
                        string fileName;
                        ConvertIP(BigData, out s, out r, out sg, out eg, out fileName);

                        if (fileName == string.Empty && Encoding.UTF8.GetString(BigData.GetRange(r, sg - r - Specialtegn.Length).ToArray()) == IP.Address.ToString())
                        {
                            AddMessage(Encoding.UTF8.GetString(BigData.GetRange(s, r - s - Specialtegn.Length).ToArray()) + " : " + Encoding.UTF8.GetString(BigData.GetRange(sg, eg - sg + 1).ToArray()));
                        }

                        if (Encoding.UTF8.GetString(BigData.GetRange(r, sg - r - fileName.Length - 2 * Specialtegn.Length).ToArray()) == IP.Address.ToString())
                        {
                            cbFile.Items.Add(fileName);
                            AddMessage(Encoding.UTF8.GetString(BigData.GetRange(s, r - s - Specialtegn.Length).ToArray()) + " : " + fileName);
                            FileBase = new FileSendReceive(fileName, BigData.GetRange(sg, eg - sg + 1));
                            ListFile.Add(FileBase);
                        }
                        BigData.RemoveRange(0, BigData.Count);
                    }
                }
            }
            catch
            {
                Close();
            }
        }       

        void AddMessage(string s)
        {
            lsvView.Items.Add(new ListViewItem() { Text = s });
            txbMessage.Clear();
        }

        new void Close()
        {
            if (IP != null)
                client.Close();
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connect();
            this.Text = "IP Address: " + IP.Address.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog Ofd = new OpenFileDialog();
            if (Ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = Ofd.FileName;
                txbMessage.Text = Ofd.SafeFileName;
                List<string> subType = fileName.Split('.').ToList();

                List<byte> Data = File.ReadAllBytes(fileName).ToList();

                FileBase = new FileSendReceive(Ofd.SafeFileName, subType[subType.Count - 1], Data);
            }
        }

        FileSendReceive FileBase;

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = cbFile.Text;
            FileSendReceive fileSendReceive = ListFile.Where(f => f.FileName == cbFile.Text).First();

            save.Filter = $"{FileBase.TypeName} File (*.{fileSendReceive.TypeName})|*.{fileSendReceive.TypeName}|Text File (*.txt)|*.txt";

            if (save.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(save.FileName, fileSendReceive.DataFile.ToArray());
            }
        }

        //Xu ly gui theo IP
        byte[] DataSend(byte[] data)
        {
            string ipSend = IP.Address.ToString();
            string ipReceive = comboBox1.Text;

            byte[] bipSend = Encoding.UTF8.GetBytes(ipSend);
            byte[] bspecialtegn = Encoding.UTF8.GetBytes(Specialtegn);
            byte[] bipReceive = Encoding.UTF8.GetBytes(ipReceive);
            byte[] bEndArray = Encoding.UTF8.GetBytes(EndArray);

            List<byte> Bigdata = new List<byte>();

            Bigdata.AddRange(bspecialtegn);
            Bigdata.AddRange(bipSend);
            Bigdata.AddRange(bspecialtegn);
            Bigdata.AddRange(bipReceive);
            Bigdata.AddRange(bspecialtegn);

            //
            if (FileBase != null && txbMessage.Text == FileBase.FileName)
            {
                Bigdata.AddRange(data);
                Bigdata.AddRange(bspecialtegn);
                Bigdata.AddRange(FileBase.DataFile.ToArray());
            }
            else
            {
                Bigdata.AddRange(data);
            }
            //

            //Bigdata.AddRange(data);
            Bigdata.AddRange(bEndArray);

            byte[] bdata = Bigdata.ToArray();

            return bdata;
        }

        bool CheckEndSend(List<byte> data)
        {
            for (int i = data.Count - 1; i > 0; i--)
            {
                if (data[i] == 0)
                    continue;

                //MessageBox.Show(Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()));
                if (Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()) == EndArray)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        //void ConvertIP(List<byte> data, out int ipS, out int ipR, out int start, out int end)
        //{
        //    int _tempCount = 0;
        //    int _IPSend = 0, _IPReceive = 0, _StartGame = 0, _EndGame = 0;
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        if(Encoding.UTF8.GetString(data.GetRange(i, Specialtegn.Length).ToArray()) == Specialtegn)
        //        {
        //            _tempCount++;
        //        }
        //        if(_tempCount == 1 && _IPSend == 0)
        //        {
        //            _IPSend = i + Specialtegn.Length;
        //            i = _IPSend;
        //        }
        //        if(_tempCount == 2 && _IPReceive == 0)
        //        {
        //            _IPReceive = i + Specialtegn.Length;
        //            i = _IPReceive;
        //        }
        //        if(_tempCount == 3 && _StartGame == 0)
        //        {
        //            _StartGame = i + Specialtegn.Length;
        //            break;
        //        }
        //    }

        //    for (int i = data.Count - 1; i > 0; i--)
        //    {
        //        if (data[i] == 0)
        //            continue;

        //        //MessageBox.Show(Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()));
        //        if (Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()) == EndArray)
        //        {
        //            _EndGame = i - EndArray.Length;
        //            break;
        //        }
        //    }

        //    ipS = _IPSend;
        //    ipR = _IPReceive;
        //    start = _StartGame;
        //    end = _EndGame;
        //}

        void ConvertIP(List<byte> data, out int ipS, out int ipR, out int start, out int end, out string filename)
        {
            int _tempCount = 0;
            int _IPSend = 0, _IPReceive = 0, _StartGame = 0, _EndGame = 0, _file = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (Encoding.UTF8.GetString(data.GetRange(i, Specialtegn.Length).ToArray()) == Specialtegn)
                {
                    _tempCount++;
                }
                if (_tempCount == 1 && _IPSend == 0)
                {
                    _IPSend = i + Specialtegn.Length;
                    i = _IPSend;
                }
                if (_tempCount == 2 && _IPReceive == 0)
                {
                    _IPReceive = i + Specialtegn.Length;
                    i = _IPReceive;
                }
                if (_tempCount == 3 && _StartGame == 0)
                {
                    _StartGame = i + Specialtegn.Length;
                    i = _StartGame;
                }
                if (i > _StartGame + 50)
                {
                    break;
                }
                if (_tempCount == 4 && _file == 0)
                {
                    _file = i + Specialtegn.Length;
                    break;
                }
            }

            if (_tempCount == 4)
            {
                int tempt = _file;
                _file = _StartGame;
                _StartGame = tempt;
                filename = Encoding.UTF8.GetString(data.GetRange(_file, _StartGame - _file - Specialtegn.Length).ToArray());
            }
            else
            {
                filename = string.Empty;
            }

            for (int i = data.Count - 1; i > 0; i--)
            {
                if (data[i] == 0)
                    continue;

                //MessageBox.Show(Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()));
                if (Encoding.UTF8.GetString(data.GetRange(i + 1 - EndArray.Length, EndArray.Length).ToArray()) == EndArray)
                {
                    _EndGame = i - EndArray.Length;
                    break;
                }
            }

            ipS = _IPSend;
            ipR = _IPReceive;
            start = _StartGame;
            end = _EndGame;
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

        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
                
        //}
    }
}
