using EasyModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModbusDeneme
{
    public partial class Form1 : Form
    {
        ModbusClient modbusClient;
        bool surekliOkumaDevrede = false;
        bool aqEdit = false;
        List<CheckBox> cbDIList;
        List<CheckBox> cbDQList;
        List<Label> lblDI;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            lblDI = new List<Label>
            {
                lblDI0,
                lblDI1,
                lblDI2,
                lblDI3,
                lblDI4,
                lblDI5,
                lblDI6,
                lblDI7


            };
            cbDQList = new List<CheckBox>
            {
                checkBoxDQ0,
                checkBoxDQ1,
                checkBoxDQ2,
                checkBoxDQ3,
                checkBoxDQ4,
                checkBoxDQ5,
                checkBoxDQ6,
                checkBoxDQ7
            };

            RbBarkodAl.Text = "";
            LblBarkodUz.Text = "";
            timer1.Enabled = true;

        }

        private void BtnOku_Click(object sender, EventArgs e)
        {
            surekliOkumaDevrede = !surekliOkumaDevrede;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            BtnOku.ForeColor = surekliOkumaDevrede ? Color.Green : SystemColors.ControlText;

            if (modbusClient!=null && modbusClient.Connected)
            {
                BtnOku.Enabled = true;
                panelDigital.Enabled = true;
                panelAnalog.Enabled = true;
                BtnBaglanTcp.Enabled = false;
                BtnBaglanRtu.Enabled = false;
                BtnKapatTcp.Enabled = true;
                BtnKapatRtu.Enabled = true;
                BtnGonder.Enabled = true;
                if (surekliOkumaDevrede == true)
                {
                    if(comboBoxCihaz.Text == "DIGITAL")
                    {
                        bool[] inputs = modbusClient.ReadDiscreteInputs(0, 8);
                        for (var i = 0; i < inputs.Length; i++)
                        {
                            if (inputs[i] == true)
                            {
                                lblDI[i].BackColor = Color.Lime;
                                
                            }
                            else
                            {
                                lblDI[i].BackColor = Color.Red;
                            }                 
                        }

                        bool[] outputs = modbusClient.ReadCoils(0, 8);
                        for (var i = 0; i < outputs.Length; i++)
                        {
                            cbDQList[i].Checked = outputs[i];
                        }
                    }
                    if (comboBoxCihaz.Text == "ANALOG")
                      
                    {
                        int[] regsAnalog = modbusClient.ReadHoldingRegisters(0, 10);
                        if (aqEdit == false)
                        {
                           
                            textBoxAq0.ReadOnly = true;
                            textBoxAq1.ReadOnly = true;
                            textBoxAq0.Text = regsAnalog[0].ToString();
                            textBoxAq1.Text = regsAnalog[1].ToString();
                            buttonAQ_Yaz.Enabled = false;
                        }
                        else
                        {
                            textBoxAq0.ReadOnly = false;
                            textBoxAq1.ReadOnly = false;
                            buttonAQ_Yaz.Enabled = true;
                        }
                        textBoxAI0.Text = regsAnalog[2].ToString();
                        textBoxAI1.Text = regsAnalog[3].ToString();
                        textBoxAI2.Text = regsAnalog[4].ToString();
                        textBoxAI3.Text = regsAnalog[5].ToString();
                        textBoxAI4.Text = regsAnalog[6].ToString();
                        textBoxAI5.Text = regsAnalog[7].ToString();
                        textBoxAI6.Text = regsAnalog[8].ToString();
                        textBoxAI7.Text = regsAnalog[9].ToString();
                    }
                    int[] readHoldingRegisters = modbusClient.ReadHoldingRegisters(10, 121);
                    if (readHoldingRegisters[0] > 0)
                    {
                        var adet = readHoldingRegisters[0] / 2 + 1;
                        //int[] readHoldingRegisters2 = modbusClient.ReadHoldingRegisters(11, adet);
                        var str = ModbusClient.ConvertRegistersToString(readHoldingRegisters, 1, readHoldingRegisters[0] + 1);
                        var str2 = str.Replace("\0", string.Empty);
                        Console.WriteLine("Okunan Barkod: {0}", str2);
                        LblBarkodUz.Text = str2.Length.ToString();
                        RbBarkodAl.Text = str2;
                    }

                }
                
            }
            else
            {
                panelDigital.Enabled = false;
                panelAnalog.Enabled = false;
                BtnBaglanTcp.Enabled = true;
                BtnBaglanRtu.Enabled = true;
                BtnKapatTcp.Enabled = false;
                BtnKapatRtu.Enabled = false;
                BtnOku.Enabled = false;
                BtnGonder.Enabled = false;
            }
            timer1.Enabled = true;
        }

        private void BtnGonder_Click(object sender, EventArgs e)
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                string str = RbBarkodGonder.Text;
                var bList = ModbusClient.ConvertStringToRegisters(str).ToList();
                bList.Insert(0, str.Length);
                modbusClient.WriteMultipleRegisters(131, bList.ToArray());
            }
        }

        private void buttonAQ_Yaz_Click(object sender, EventArgs e)
        {
            int[] sayilar = { 0, 0 };


            if (int.TryParse(textBoxAq0.Text, out int deger1) && int.TryParse(textBoxAq1.Text, out int deger2))
            {
                sayilar[0] = deger1;
                sayilar[1] = deger2;
                modbusClient.WriteMultipleRegisters(0, sayilar);
                aqEdit = false;
            }
        }

        private void BtnBaglan_Click(object sender, EventArgs e)
        {
            try
            {
                modbusClient = new ModbusClient(textBoxIP.Text, (int)numericUpDownPort.Value);
                modbusClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void BtnKapat_Click(object sender, EventArgs e)
        {
            modbusClient.Disconnect();
            modbusClient = null;
        }

        private void BtnBaglanRtu_Click(object sender, EventArgs e)
        {
            try
            {
                modbusClient = new ModbusClient(textBoxCom.Text)
                {
                    Baudrate = (int)numericUpDownBoud.Value,
                    Parity = System.IO.Ports.Parity.None,
                    StopBits = System.IO.Ports.StopBits.One,
                    UnitIdentifier = 1
                };
                modbusClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnKapatRtu_Click(object sender, EventArgs e)
        {
            modbusClient.Disconnect();
            modbusClient = null;    
        }


        private void buttonAQ_Edit_Click(object sender, EventArgs e)
        {
            aqEdit = !aqEdit;
        }



        private void CheckBoxDQ_CheckedChanged(object sender, EventArgs e)
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                var cbDQ = sender as CheckBox;
                if(int.TryParse(cbDQ.Tag.ToString(),out int startingAddress))
                {
                    modbusClient.WriteSingleCoil(startingAddress, cbDQ.Checked);
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBoxFark_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBoxAi_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxCihaz_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelDigital.Visible = comboBoxCihaz.Text == "DIGITAL";
            panelAnalog.Visible = comboBoxCihaz.Text == "ANALOG";
        }

    
    }
}
