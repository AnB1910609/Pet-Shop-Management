using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management
{
    public partial class Customers : Form
    {
        public Customers()
        {
            InitializeComponent();
            DisplayCustomers();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=LAPTOP-3879ECGQ;Initial Catalog=PetShopManagement;Integrated Security=True");

        private void DisplayCustomers()
        {
            Con.Open();
            string Query = "Select * from CustomerTbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            CustomersDGV.DataSource = ds.Tables[0];
            CustomersDGV.Columns[0].HeaderCell.Value = "ID";
            CustomersDGV.Columns[1].HeaderCell.Value = "Name";
            CustomersDGV.Columns[2].HeaderCell.Value = "Phone";
            CustomersDGV.Columns[3].HeaderCell.Value = "Address";
            CustomersDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        private void Clear()
        {
            CustNameTb.Text = "";
            CustPhoneTb.Text = "";
            CustAddTb.Text = "";
            Key = 0;
        }

        int Key = 0;
        private void CustomersDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            int i;
            i = CustomersDGV.CurrentRow.Index;
            CustNameTb.Text = CustomersDGV.Rows[i].Cells[1].Value.ToString();
            CustPhoneTb.Text = CustomersDGV.Rows[i].Cells[2].Value.ToString();
            CustAddTb.Text = CustomersDGV.Rows[i].Cells[3].Value.ToString();

            if (CustNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(CustomersDGV.Rows[i].Cells[0].Value.ToString());
            }
        }

        string namePattern = @"^[a-zA-Z\s]*$";
        string phonePattern = @"^[0-9][0-9]\d{8}$";
        string addPattern = @"^[a-zA-Z_\-\,ÀÁÂÃÈÉÊẾÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêếìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$";
        
        private void Savebtn_Click(object sender, EventArgs e)
        {
            bool isNameValid = Regex.IsMatch(CustNameTb.Text, namePattern);
            bool isPhoneValid = Regex.IsMatch(CustPhoneTb.Text, phonePattern);
            bool isAddValid = Regex.IsMatch(CustAddTb.Text, addPattern);

            if (CustNameTb.Text == "" || !isNameValid)
            {
                MessageBox.Show("Please enter a valid name!");
                CustNameTb.Focus();
            }
            else if (!isPhoneValid)
            {
                MessageBox.Show("Please enter a valid phone number!");
                CustPhoneTb.Focus();
            }
            else if (!isAddValid)
            {
                MessageBox.Show("Please enter a valid address!");
                CustAddTb.Focus();
            }
            else
            {
                try
                {
                    Con.Open();
                    string Query = "Select * from CustomerTbl where CustName = '" + CustNameTb.Text + "' and CustPhone = '" + CustPhoneTb.Text + "' and CustAdd = '" + CustAddTb.Text + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                    SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                    var ds = new DataSet();

                    string phone = "Select * from CustomerTbl where CustPhone = '" + CustPhoneTb.Text + "'";
                    SqlDataAdapter sdaP = new SqlDataAdapter(phone, Con);
                    SqlCommandBuilder BuilderP = new SqlCommandBuilder(sdaP);
                    var dsP = new DataSet();
                    Con.Close();

                    if (sda.Fill(ds) == 0)
                    {
                        if (sdaP.Fill(dsP) == 0)
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand("Insert into CustomerTbl (CustName, CustPhone, CustAdd) values (@CN, @CP, @CA)", Con);
                            cmd.Parameters.AddWithValue("@CN", CustNameTb.Text);
                            cmd.Parameters.AddWithValue("@CP", CustPhoneTb.Text);
                            cmd.Parameters.AddWithValue("@CA", CustAddTb.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Customer Added!");
                            Con.Close();
                            DisplayCustomers();
                            Clear();
                        }
                        else
                        {
                            MessageBox.Show("Phone number already exists!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Customer already exists!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            bool isNameValid = Regex.IsMatch(CustNameTb.Text, namePattern);
            bool isPhoneValid = Regex.IsMatch(CustPhoneTb.Text, phonePattern);
            bool isAddValid = Regex.IsMatch(CustAddTb.Text, addPattern);

            if (Key == 0)
            {
                MessageBox.Show("Select an customer!");
            }
            else
            {
                if (CustNameTb.Text == "" || !isNameValid)
                {
                    MessageBox.Show("Please enter a valid name!");
                    CustNameTb.Focus();
                }
                else if (!isPhoneValid)
                {
                    MessageBox.Show("Please enter a valid phone number!");
                    CustPhoneTb.Focus();
                }
                else if (!isAddValid)
                {
                    MessageBox.Show("Please enter a valid address!");
                    CustAddTb.Focus();
                }
                else
                {
                    try
                    {
                        Con.Open();
                        string Query = "Select * from CustomerTbl where CustName = '" + CustNameTb.Text + "' and CustPhone = '" + CustPhoneTb.Text + "' and CustAdd = '" + CustAddTb.Text + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                        SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                        var ds = new DataSet();

                        string phones = "Select * from CustomerTbl where CustPhone = '" + CustPhoneTb.Text + "'";
                        SqlDataAdapter sdaPs = new SqlDataAdapter(phones, Con);
                        SqlCommandBuilder BuilderPs = new SqlCommandBuilder(sdaPs);
                        var dsPs = new DataSet();

                        string phone = "Select * from CustomerTbl where CustPhone = '" + CustPhoneTb.Text + "' and CustId = " + Key + "";
                        SqlDataAdapter sdaP = new SqlDataAdapter(phone, Con);
                        SqlCommandBuilder BuilderP = new SqlCommandBuilder(sdaP);
                        var dsP = new DataSet();
                        Con.Close();

                        if (sda.Fill(ds) == 0)
                        {
                            if (sdaPs.Fill(dsPs) == 0)
                            {
                                Con.Open();
                                SqlCommand cmd = new SqlCommand("Update CustomerTbl set CustName=@CN , CustPhone=@CP ,CustAdd=@CA where CustId=@EKey", Con);
                                cmd.Parameters.AddWithValue("@CN", CustNameTb.Text);
                                cmd.Parameters.AddWithValue("@CP", CustPhoneTb.Text);
                                cmd.Parameters.AddWithValue("@CA", CustAddTb.Text);
                                cmd.Parameters.AddWithValue("@EKey", Key);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Customer Updated!");
                                Con.Close();
                                DisplayCustomers();
                                Clear();
                            }
                            else
                            {
                                if (sdaP.Fill(dsP) == 0)
                                {
                                    MessageBox.Show("Phone number already exists!");
                                }
                                else
                                {
                                    Con.Open();
                                    SqlCommand cmd = new SqlCommand("Update CustomerTbl set CustName=@CN , CustPhone=@CP ,CustAdd=@CA where CustId=@EKey", Con);
                                    cmd.Parameters.AddWithValue("@CN", CustNameTb.Text);
                                    cmd.Parameters.AddWithValue("@CP", CustPhoneTb.Text);
                                    cmd.Parameters.AddWithValue("@CA", CustAddTb.Text);
                                    cmd.Parameters.AddWithValue("@EKey", Key);
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Customer Updated!");
                                    Con.Close();
                                    DisplayCustomers();
                                    Clear();
                                }

                            }
                        }
                        else
                        {
                            MessageBox.Show("Change information to update!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (Key == 0)
            {
                MessageBox.Show("Select an customer!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from CustomerTbl where CustId=@EKey", Con);
                    cmd.Parameters.AddWithValue("@EKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Customer Deleted!");
                    Con.Close();
                    DisplayCustomers();
                    Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisplaySearch()
        {
            Con.Open();
            string Query = "Select * from CustomerTbl where CustName like '%" + CustSearchTb.Text + "%'";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            CustomersDGV.DataSource = ds.Tables[0];
            CustomersDGV.Columns[0].HeaderCell.Value = "ID";
            CustomersDGV.Columns[1].HeaderCell.Value = "Name";
            CustomersDGV.Columns[2].HeaderCell.Value = "Phone";
            CustomersDGV.Columns[3].HeaderCell.Value = "Address";
            CustomersDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {
            DisplaySearch();
        }

        private void CustRSbtn_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Category Obj = new Category();
            Obj.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Category Obj = new Category();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Category Obj = new Category();
            Obj.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Products Obj = new Products();
            Obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Products Obj = new Products();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Products Obj = new Products();
            Obj.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Employees Obj = new Employees();
            Obj.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Employees Obj = new Employees();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Employees Obj = new Employees();
            Obj.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Customers Obj = new Customers();
            Obj.Show();
            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Customers Obj = new Customers();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Customers Obj = new Customers();
            Obj.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bill Obj = new Bill();
            Obj.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Bill Obj = new Bill();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Bill Obj = new Bill();
            Obj.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Login Obj = new Login();
            Obj.Show();
            this.Close();
        }

        private void label16_Click(object sender, EventArgs e)
        {
            Login Obj = new Login();
            Obj.Show();
            this.Close();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Login Obj = new Login();
            Obj.Show();
            this.Close();
        }
    }
}
