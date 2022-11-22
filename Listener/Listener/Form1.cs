using Listener.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Listener
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        UdpState s = new UdpState();
        IPEndPoint e;
        UdpClient u;
        AccurateTimer mTimer1;
        private void Form1_Load(object sender, EventArgs ex)
        {





            FileService.Listen("C:\\Users\\Msi\\Desktop\\");

            Control.CheckForIllegalCrossThreadCalls = false;

            using (StreamReader r = new StreamReader("C:\\Users\\Msi\\Desktop\\value.json"))
            {
                string json = r.ReadToEnd();
                StaticClass.Values = JsonConvert.DeserializeObject<ValueModel>(json);
            }


            e = new IPEndPoint(IPAddress.Parse("192.168.56.1"), Convert.ToInt32(9050));


            u = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.56.1"), Convert.ToInt32(9050)));
           
            
            s.e = e;
            s.u = u;

            int delay = 15;  
            mTimer1 = new AccurateTimer(this, new Action(callFunction), delay);
        }
        public void callFunction()
        {
            try
            {
                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            }
            catch (Exception e)
            {
                mTimer1.Stop();
                u.Close();
                throw;
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {



            try
            {


                byte[] receiveBytes = u.EndReceive(ar, ref e);





                List<string> text = new List<string>();


                byte[] idArray = new byte[4];

                Array.Copy(receiveBytes, idArray, 4);

                int id = BitConverter.ToInt32(idArray, 0);


                int counter = 4;

                var packet = StaticClass.Values.Packets.FirstOrDefault(x => x.id == id);
                foreach (var item in packet.Values)
                {
                    if (item.Type == "int32")
                    {
                        counter += 4;


                    }
                    else if (item.Type == "String")
                    {




                        counter += 50;
                    }
                    else if (item.Type == "bool")
                    {



                        counter += 1;
                    }

                }


                if (receiveBytes.Length == counter)
                {
                    if(!StaticClass.historyModels.Any(x=>x.Id==id))
                        StaticClass.historyModels.Add(new historyModel { Id = id ,Data=new FixedSizedQueue<byte[]>()});
                    StaticClass.historyModels.FirstOrDefault(x => x.Id == id).Data.Enqueue(receiveBytes);

                    Console.WriteLine(StaticClass.historyModels.Count+"   "+StaticClass.historyModels.FirstOrDefault(x=>x.Id==id).Data.getCount().ToString());


                    if (!StaticClass.lastvalues.Any(x => x.id == id))
                    {

                        StaticClass.lastvalues.Add(new lastValue { id = id, value = receiveBytes });


                    }
                    else
                    {
                        StaticClass.lastvalues.FirstOrDefault(x => x.id == id).value = receiveBytes;
                    }

                    if (StaticClass.list.Packets != null)
                    {
                        if (StaticClass.list.Packets.Any(a => a.id == id))
                        {
                         


                        }
                        else
                        {
                            //add staticclass and listbox
                            StaticClass.list.Packets.Add(StaticClass.Values.Packets.FirstOrDefault(y => y.id == id));
                            listBox1.Items.Add(StaticClass.Values.Packets.FirstOrDefault(y => y.id == id).PacketName);
                        }
                    }
                    else
                    {
                        StaticClass.list.Packets = new List<Packet>();
                        StaticClass.list.Packets.Add(StaticClass.Values.Packets.FirstOrDefault(y => y.id == id));
                        listBox1.Items.Add(StaticClass.Values.Packets.FirstOrDefault(y => y.id == id).PacketName);

                    }

                    if (listBox1.SelectedIndex != -1)
                    {
                       

                        if (StaticClass.list.Packets[listBox1.SelectedIndex].id == id)
                        {
                          


                            int index = 4;
                            var packets = StaticClass.Values.Packets.FirstOrDefault(a => a.id == id);

                            foreach (var item in packets.Values)
                            {
                                if (item.Type == "int32")
                                {
                                    byte[] array = new byte[4];
                                    Array.Copy(receiveBytes, index, array, 0, 4);
                                    string text1 = BitConverter.ToInt32(array, 0).ToString();
                                    text.Add(text1);
                                    index += 4;


                                }
                                else if (item.Type == "String")
                                {
                                    byte[] array = new byte[50];
                                    Array.Copy(receiveBytes, index, array, 0, 50);
                                    string text1 = System.Text.Encoding.Default.GetString(array);
                                    text1 = text1.Replace("\0", "");
                                    text.Add(text1);



                                    index += 50;
                                }
                                else if (item.Type == "bool")
                                {
                                    byte[] array = new byte[1];
                                    Array.Copy(receiveBytes, index, array, 0, 1);
                                    string text1 = BitConverter.ToBoolean(array, 0).ToString();
                                    text.Add(text1);


                                    index += 1;
                                }

                            }
                            bool flag = false;


                            int indextext = 0;
                            foreach (var listitem in listBox2.Items)
                            {
                                
                              


                                if (text[indextext]!=listitem.ToString())
                                {


                                    flag = true;
                                }
                                indextext++;
                            }
                            if (flag)
                            {

                                listBox2.Items.Clear();
                                foreach (var item in text)
                                {

                                    listBox2.Items.Add(item);
                                }
                            }




                        }

                    }

                }

            }
            catch (Exception)
            {




            }
            //Console.WriteLine(sw.ElapsedMilliseconds - swold);
            //if (sw.ElapsedMilliseconds >= 10000)
            //{
            //    sw.Stop();
            //}
            //swold = sw.ElapsedMilliseconds;

            //sayac++;
            //      u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StaticClass.list.Packets.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            listBox2.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StaticClass.list.Packets.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox2.Items.Clear();
                var receiveBytes = StaticClass.lastvalues.FirstOrDefault(a => a.id == StaticClass.list.Packets[listBox1.SelectedIndex].id).value;
                var id = StaticClass.lastvalues.FirstOrDefault(a => a.id == StaticClass.list.Packets[listBox1.SelectedIndex].id).id;
                var packets = StaticClass.Values.Packets.FirstOrDefault(a => a.id == id);

                int index = 4;
                List<string> text = new List<string>();
                foreach (var item in packets.Values)
                {
                    if (item.Type == "int32")
                    {
                        byte[] array = new byte[4];
                        Array.Copy(receiveBytes, index, array, 0, 4);
                        string text1 = BitConverter.ToInt32(array, 0).ToString();
                        text.Add(text1);
                        index += 4;


                    }
                    else if (item.Type == "String")
                    {
                        byte[] array = new byte[50];
                        Array.Copy(receiveBytes, index, array, 0, 50);
                        string text1 = System.Text.Encoding.Default.GetString(array);
                        text1 = text1.Replace("\0", "");
                        text.Add(text1);



                        index += 50;
                    }
                    else if (item.Type == "bool")
                    {
                        byte[] array = new byte[1];
                        Array.Copy(receiveBytes, index, array, 0, 1);
                        string text1 = BitConverter.ToBoolean(array, 0).ToString();
                        text.Add(text1);


                        index += 1;
                    }

                }
                foreach (var item in text)
                {
                    listBox2.Items.Add(item);
                }
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            history history = new history(listBox1.SelectedIndex);
            history.Show();
        }
    }


    class AccurateTimer
    {
        private delegate void TimerEventDel(int id, int msg, IntPtr user, int dw1, int dw2);
        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;// + 0x100;  // TIME_KILL_SYNCHRONOUS causes a hang ?!
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventDel handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        Action mAction;
        Form mForm;
        private int mTimerId;
        private TimerEventDel mHandler;  // NOTE: declare at class scope so garbage collector doesn't release it!!!

        public AccurateTimer(Form form, Action action, int delay)
        {
            mAction = action;
            mForm = form;
            timeBeginPeriod(1);
            mHandler = new TimerEventDel(TimerCallback);
            mTimerId = timeSetEvent(delay, 0, mHandler, IntPtr.Zero, EVENT_TYPE);
        }

        public void Stop()
        {
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1);
            System.Threading.Thread.Sleep(100);// Ensure callbacks are drained
        }

        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (mTimerId != 0)
                mForm.BeginInvoke(mAction);
        }
    }
}
