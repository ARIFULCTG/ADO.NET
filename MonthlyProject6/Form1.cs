using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonthlyProject6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        MemoryStream ms;

        private void BlankTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            dataGridView1.Rows.Clear();
            textBox1.Focus();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView1);
            newRow.Cells[0].Value = textBox6.Text;
            newRow.Cells[1].Value = textBox7.Text;
            newRow.Cells[2].Value = textBox8.Text;
            dataGridView1.Rows.Add(newRow);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"delete from Patient where DonorID='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from Donor where ID='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();



                            string fn = Path.GetFileName(textBox9.Text);
                            string path = AppDomain.CurrentDomain.BaseDirectory + @"Images\" + fn.ToString();
                            if (!File.Exists(path))
                            {
                                File.Copy(textBox9.Text, path);
                            }
                            string dt = dateTimePicker1.Value.ToShortDateString();

                            cmd.CommandText = $"insert into Donor values(@id, @name, @bloodgroup, @number, @dt, @pic, @picstring)";
                            cmd.Parameters.Add(new SqlParameter("@id", textBox1.Text));
                            cmd.Parameters.Add(new SqlParameter("@name", textBox2.Text));
                            cmd.Parameters.Add(new SqlParameter("@bloodgroup", textBox3.Text));
                            cmd.Parameters.Add(new SqlParameter("@number", textBox4.Text));
                            cmd.Parameters.Add(new SqlParameter("@dt", dt));
                            cmd.Parameters.Add(new SqlParameter("@pic", conv_photo()));
                            cmd.Parameters.Add(new SqlParameter("@picstring", "Images\\" + fn.ToString()));
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                            {
                                cmd.CommandText = $"insert into Patient values({int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString())}, '{dataGridView1.Rows[i].Cells[1].Value.ToString()}', '{dataGridView1.Rows[i].Cells[2].Value.ToString()}', '{textBox1.Text}')";
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();


                            }
                            tran.Commit();

                        }

                        dataGridView1.Rows.Clear();
                    }
                    catch (Exception xcp)
                    {
                        tran.Rollback();
                        MessageBox.Show(xcp.ToString());
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionDB a = new ConnectionDB();
            a.conn1($"select * from Donor where ID='{comboBox1.Text}'");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())
            {
                textBox1.Text = rdr["ID"].ToString();
                textBox2.Text = rdr["Name"].ToString();
                textBox3.Text = rdr["BloodGroup"].ToString();
                textBox4.Text = rdr["Number"].ToString();
                dateTimePicker1.Value = DateTime.Parse(rdr["DonationDate"].ToString());

                pictureBox1.Image = null;
                if (rdr["Photo"] != System.DBNull.Value)
                {
                    Byte[] byteBLOBData = new Byte[0];
                    byteBLOBData = (Byte[])((byte[])rdr["Photo"]);
                    MemoryStream ms = new MemoryStream(byteBLOBData);
                    ms.Write(byteBLOBData, 0, byteBLOBData.Length);
                    ms.Position = 0; //insert this line
                    pictureBox1.Image = Image.FromStream(ms);
                }
                pictureBox2.ImageLocation = AppDomain.CurrentDomain.BaseDirectory + rdr["stringphoto"].ToString();

            }
            a.conn1($"select * from Patient where DonorID='{comboBox1.Text}' order by ID");
            SqlDataReader rdr2 = a.cmd1.ExecuteReader();//select

            int i = 0;
            while (rdr2.Read())//until last data
            {
                //textBox6.Text = rdr2["vno"].ToString();
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dataGridView1);
                newRow.Cells[0].Value = rdr2["ID"];
                newRow.Cells[1].Value = rdr2["Name"];
                newRow.Cells[2].Value = rdr2["Number"];
                dataGridView1.Rows.Add(newRow);
                i++;
            }

            textBox5.Text = pictureBox1.ImageLocation;
            textBox9.Text = pictureBox2.ImageLocation;

            //SqlDataReader rdr2 = a.cmd1.ExecuteReader();
            //DataTable dt = new DataTable();//disconnected class
            //dt.Load(rdr2, LoadOption.Upsert);
            //dataGridView1.DataSource = dt;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Foreign Key table columns, in our project student is primary key table, fees is foregn key table
            dataGridView1.Columns.Add("ID", "Patient ID");
            dataGridView1.Columns.Add("Name", "Patient Name");
            dataGridView1.Columns.Add("Number", "Number");
        }

        byte[] conv_photo()
        {
            byte[] photo_aray = { };
            //converting photo to binary data
            if (pictureBox1.Image != null)
            {
                ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
            }
            return photo_aray;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                textBox5.Text = openFileDialog1.FileName;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox2.ImageLocation = openFileDialog1.FileName;
                textBox9.Text = openFileDialog1.FileName;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            BlankTextBoxes();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ConnectionDB a = new ConnectionDB();
            a.conn1("select distinct ID from Donor");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())//until last data
            {
                comboBox1.Items.Add(rdr[0].ToString());//0=> first field, Id
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"delete from Patient where DonorID='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from Donor where ID='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            dataGridView1.Rows.Clear();

                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            a.Show();
        }

        public static string GetComBo = "";
        private void button7_Click(object sender, EventArgs e)
        {
            GetComBo = comboBox1.Text;
            Form3 a = new Form3();
            a.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
