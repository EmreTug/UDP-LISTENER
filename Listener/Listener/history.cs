using Listener.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Listener
{
    public partial class history : Form
    {
        public history(int index)
        {
            InitializeComponent();
            this.indexMember= index;
        }
        int indexMember=0;
        private void history_Load(object sender, EventArgs e)
        {
            int id = StaticClass.lastvalues[indexMember].id;
            var data = StaticClass.historyModels.FirstOrDefault(x => x.Id == id).Data.getList();
            var packet = StaticClass.Values.Packets.FirstOrDefault(x => x.id == id);
          
            foreach(var receiveBytes in data)
            {
                int index = 4;

                foreach (var item in packet.Values)
                {
                    if (item.Type == "int32")
                    {
                        byte[] array = new byte[4];
                        Array.Copy(receiveBytes, index, array, 0, 4);
                        string text1 = BitConverter.ToInt32(array, 0).ToString();
                        richTextBox1.Text += text1+"\n";
                        index += 4;


                    }
                    else if (item.Type == "String")
                    {
                        byte[] array = new byte[50];
                        Array.Copy(receiveBytes, index, array, 0, 50);
                        string text1 = System.Text.Encoding.Default.GetString(array);
                        text1 = text1.Replace("\0", "");

                        richTextBox1.Text += text1 + "\n";




                        index += 50;
                    }
                    else if (item.Type == "bool")
                    {
                        byte[] array = new byte[1];
                        Array.Copy(receiveBytes, index, array, 0, 1);
                        string text1 = BitConverter.ToBoolean(array, 0).ToString();
                        richTextBox1.Text += text1 + "\n";




                        index += 1;
                    }
                
                }
                richTextBox1.Text += "\n\n--------------------------------\n";

            }


        }
    }
}
