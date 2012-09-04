using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThingMagic;

namespace KillTag
{
    public partial class Form1 : Form
    {
        TagData tagData = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portlist = System.IO.Ports.SerialPort.GetPortNames();
            comboBox_PortSelect.Items.AddRange(portlist);      //populate the comboBox with the port names
            comboBox_PortSelect.Text = portlist[portlist.Length-1];
            textBox4.Text =
                "Kill Tag Instructions" + Environment.NewLine +
                "Step 1. Read EPC" + Environment.NewLine +
                "Step 2. Read kill password" + Environment.NewLine +
                "Step 3. If kill password is 0x00000000, write new kill password" + Environment.NewLine +
                "Step 4. Kill tag";
        }
        //private string pvtReadTagData(int addressToRead, int byteCount, Gen2.Bank bankTORead)
        //{
        //    string readData = null;
        //    if (0 != (byteCount % 2))
        //    {
        //        throw new ArgumentException("Byte count must be an even number");
        //    }
        //    try
        //    {
        //        using(Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
        //        {
        //            DateTime timeBeforeRead = DateTime.Now;
                    
        //            readData = ByteFormat.ToHex(reader.ReadTagMemBytes(tagData, (int)bankTORead, addressToRead, byteCount));
        //            DateTime timeAfterRead = DateTime.Now;
        //            TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
        //            commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
        //            return readData;
        //        }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        MessageBox.Show(exp.Message);
        //        return null;
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime timeBeforeRead = DateTime.Now;
            try
            {
                using (Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
                {
                    reader.Connect();

                    tagData = reader.Read(100)[0].Tag;

                    textBox1.Text = tagData.EpcString;
                }
                DateTime timeAfterRead = DateTime.Now;
                TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
                commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Index was outside"))
                {
                    MessageBox.Show("No Tag Found");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = ReadPassword(4, 4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = ReadPassword(0, 4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WritePassword(0, textBox3.Text);
        }
        public string ReadPassword(uint byteIndex, uint length)
        {
            try
            {
                DateTime timeBeforeRead = DateTime.Now;
                byte[] data = null;
                using (Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
                {
                    reader.Connect();
                    TagOp tagOp = new Gen2.ReadData(Gen2.Bank.RESERVED, byteIndex, (byte)length);
                    SimpleReadPlan plan = new SimpleReadPlan(null, TagProtocol.GEN2, null, tagOp, 1000);
                    reader.ParamSet("/reader/read/plan", plan);
                    data = reader.ReadTagMemBytes(tagData, (int)Gen2.Bank.RESERVED, (int)byteIndex, (int)length);
                }
                DateTime timeAfterRead = DateTime.Now;
                TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
                commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
                return ByteFormat.ToHex(data);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
        public void WritePassword(uint byteIndex, string password)
        {
            try
            {
                DateTime timeBeforeRead = DateTime.Now;
                ushort[] data = ByteConv.ToU16s(ByteFormat.FromHex(password));
                using (Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
                {
                    reader.Connect();
                    TagOp tagOp = new Gen2.WriteData(Gen2.Bank.RESERVED, byteIndex, data);
                    SimpleReadPlan plan = new SimpleReadPlan(null, TagProtocol.GEN2, null, tagOp, 1000);
                    reader.ParamSet("/reader/read/plan", plan);
                    reader.ExecuteTagOp(tagOp, tagData);
                }
                DateTime timeAfterRead = DateTime.Now;
                TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
                commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Kill()
        {
            try
            {
                DateTime timeBeforeRead = DateTime.Now;
                using (Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
                {
                    reader.Connect();
                    uint killPassword = ByteConv.ToU32(ByteFormat.FromHex(textBox3.Text),0);
                    TagOp tagOp = new Gen2.Kill(killPassword);
                    SimpleReadPlan plan = new SimpleReadPlan(null, TagProtocol.GEN2, null, tagOp, 1000);
                    reader.ParamSet("/reader/read/plan", plan);
                    reader.ExecuteTagOp(tagOp, tagData);
                }
                DateTime timeAfterRead = DateTime.Now;
                TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
                commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboBox_PortSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            Vars.comport = comboBox_PortSelect.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime timeBeforeRead = DateTime.Now;
            try
            {
                using (Reader reader = Reader.Create("tmr:///" + Vars.comport.ToLower()))
                {
                    reader.Connect();
                    TagData epc = new TagData(textBox1.Text);
                    reader.WriteTag(tagData, epc);
                }
                DateTime timeAfterRead = DateTime.Now;
                TimeSpan timeElapsed = timeAfterRead - timeBeforeRead;
                commandTotalTimeTextBox.Text = timeElapsed.TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("General Tag Error"))
                {
                    MessageBox.Show("No Tag Found");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WritePassword(2, textBox2.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Kill();
        }
    }
    public class Vars
    {
        public static string comport { get; set; }          //Variable to store COM port
    }
}
