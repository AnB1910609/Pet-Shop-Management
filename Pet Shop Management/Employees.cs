using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management
{
    public partial class Employees : Form
    {
        public Employees()
        {
            InitializeComponent();
            DisplayEmployees();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=LAPTOP-3879ECGQ;Initial Catalog=PetShopManagement;Integrated Security=True");

        private void DisplayEmployees()
        {
            Con.Open();
            string Query = "Select * from Employeetbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            EmployeesDGV.DataSource = ds.Tables[0];
            EmployeesDGV.Columns[0].HeaderCell.Value = "ID";
            EmployeesDGV.Columns[1].HeaderCell.Value = "Name";
            EmployeesDGV.Columns[2].HeaderCell.Value = "Phone";
            EmployeesDGV.Columns[3].HeaderCell.Value = "Date of birth";
            EmployeesDGV.Columns[4].HeaderCell.Value = "Address";
            EmployeesDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        private void Clear()
        {
            EmpNameTb.Text = "";
            EmpPhoneTb.Text = "";
            EmpDOBTB.Value = DateTime.Now;
            EmpAddTb.Text = "";
            Key = 0;
        }

        int Key = 0;

        private void EmployeesDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = EmployeesDGV.CurrentRow.Index;
            EmpNameTb.Text = EmployeesDGV.Rows[i].Cells[1].Value.ToString();
            EmpPhoneTb.Text = EmployeesDGV.Rows[i].Cells[2].Value.ToString();
            EmpDOBTB.Text = EmployeesDGV.Rows[i].Cells[3].Value.ToString();
            EmpAddTb.Text = EmployeesDGV.Rows[i].Cells[4].Value.ToString();


            if (EmpNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(EmployeesDGV.Rows[i].Cells[0].Value.ToString());
            }
        }

        string namePattern = @"^[a-zA-Z\s]*$";
        string phonePattern = @"^[0-9][0-9]\d{8}$";
        string addPattern = @"^[a-zA-Z_\-\,ÀÁÂÃÈÉÊẾÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêếìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$";

        private void Savebtn_Click(object sender, EventArgs e)
        {
            bool isNameValid = Regex.IsMatch(EmpNameTb.Text, namePattern);
            bool isPhoneValid = Regex.IsMatch(EmpPhoneTb.Text, phonePattern);
            bool isAddValid = Regex.IsMatch(EmpAddTb.Text, addPattern);

            if (EmpNameTb.Text == "" || !isNameValid)
            {
                MessageBox.Show("Please enter a valid name!");
                EmpNameTb.Focus();
            }
            else if (!isPhoneValid)
            {
                MessageBox.Show("Please enter a valid phone number!");
                EmpPhoneTb.Focus();
            }
            else if (!isAddValid)
            {
                MessageBox.Show("Please enter a valid address!");
                EmpAddTb.Focus();
            }
            else
            {
                try
                {
                    Con.Open();
                    string Query = "Select * from EmployeeTbl where EmpName = '" + EmpNameTb.Text + "' and EmpPhone = '" + EmpPhoneTb.Text + "' and EmpDOB = '" + EmpDOBTB.Text + "' and EmpAdd = '" + EmpAddTb.Text + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                    SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                    var ds = new DataSet();

                    string phone = "Select * from EmployeeTbl where EmpPhone = '" + EmpPhoneTb.Text + "'";
                    SqlDataAdapter sdaP = new SqlDataAdapter(phone, Con);
                    SqlCommandBuilder BuilderP = new SqlCommandBuilder(sdaP);
                    var dsP = new DataSet();
                    Con.Close();

                    if (sda.Fill(ds) == 0)
                    {
                        if (sdaP.Fill(dsP) == 0)
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand("Insert into EmployeeTbl (EmpName, EmpPhone, EmpDOB, EmpAdd) values (@EN, @EP, @ED, @EA)", Con);
                            cmd.Parameters.AddWithValue("@EN", EmpNameTb.Text);
                            cmd.Parameters.AddWithValue("@EP", EmpPhoneTb.Text);
                            cmd.Parameters.AddWithValue("@ED", EmpDOBTB.Value.Date);
                            cmd.Parameters.AddWithValue("@EA", EmpAddTb.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Employee Added!");
                            Con.Close();
                            DisplayEmployees();
                            Clear();
                        }
                        else
                        {
                            MessageBox.Show("Phone number already exists!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Employee already exists!");
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
            bool isNameValid = Regex.IsMatch(EmpNameTb.Text, namePattern);
            bool isPhoneValid = Regex.IsMatch(EmpPhoneTb.Text, phonePattern);
            bool isAddValid = Regex.IsMatch(EmpAddTb.Text, addPattern);

            if (Key == 0)
            {
                MessageBox.Show("Select An Employee!");
            }
            else
            {
                if (EmpNameTb.Text == "" || !isNameValid)
                {
                    MessageBox.Show("Please enter a valid name!");
                    EmpNameTb.Focus();
                }
                else if (!isPhoneValid)
                {
                    MessageBox.Show("Please enter a valid phone number!");
                    EmpPhoneTb.Focus();
                }
                else if (!isAddValid)
                {
                    MessageBox.Show("Please enter a valid address!");
                    EmpAddTb.Focus();
                }
                else
                {
                    try
                    {
                        Con.Open();
                        string Query = "Select * from EmployeeTbl where EmpName = '" + EmpNameTb.Text + "' and EmpPhone = '" + EmpPhoneTb.Text + "' and EmpDOB = '" + EmpDOBTB.Text + "' and EmpAdd = '" + EmpAddTb.Text + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                        SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                        var ds = new DataSet();

                        string phones = "Select * from EmployeeTbl where EmpPhone = '" + EmpPhoneTb.Text + "'";
                        SqlDataAdapter sdaPs = new SqlDataAdapter(phones, Con);
                        SqlCommandBuilder BuilderPs = new SqlCommandBuilder(sdaPs);
                        var dsPs = new DataSet();

                        string phone = "Select * from EmployeeTbl where EmpPhone = '" + EmpPhoneTb.Text + "' and EmpNum = " + Key +"";
                        SqlDataAdapter sdaP = new SqlDataAdapter(phone, Con);
                        SqlCommandBuilder BuilderP = new SqlCommandBuilder(sdaP);
                        var dsP = new DataSet();
                        Con.Close();

                        if (sda.Fill(ds) == 0)
                        {
                            if (sdaPs.Fill(dsPs) == 0)
                            {
                                Con.Open();
                                SqlCommand cmd = new SqlCommand("Update EmployeeTbl set EmpName=@EN, EmpPhone=@EP, EmpDOB=@ED, EmpAdd=@EA where EmpNum=@EKey", Con);
                                cmd.Parameters.AddWithValue("@EN", EmpNameTb.Text);
                                cmd.Parameters.AddWithValue("@EP", EmpPhoneTb.Text);
                                cmd.Parameters.AddWithValue("@ED", EmpDOBTB.Value.Date);
                                cmd.Parameters.AddWithValue("@EA", EmpAddTb.Text);
                                cmd.Parameters.AddWithValue("@EKey", Key);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Employee Updated!");
                                Con.Close();
                                DisplayEmployees();
                            }
                            else 
                            {
                                if(sdaP.Fill(dsP) == 0)
                                {
                                    MessageBox.Show("Phone number already exists!");
                                } else
                                {
                                    Con.Open();
                                    SqlCommand cmd = new SqlCommand("Update EmployeeTbl set EmpName=@EN, EmpPhone=@EP, EmpDOB=@ED, EmpAdd=@EA where EmpNum=@EKey", Con);
                                    cmd.Parameters.AddWithValue("@EN", EmpNameTb.Text);
                                    cmd.Parameters.AddWithValue("@EP", EmpPhoneTb.Text);
                                    cmd.Parameters.AddWithValue("@ED", EmpDOBTB.Value.Date);
                                    cmd.Parameters.AddWithValue("@EA", EmpAddTb.Text);
                                    cmd.Parameters.AddWithValue("@EKey", Key);
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Employee Updated!");
                                    Con.Close();
                                    Clear();
                                    DisplayEmployees();
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
                MessageBox.Show("Select An Employee!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from EmployeeTbl where EmpNum=@EKey", Con);
                    cmd.Parameters.AddWithValue("@EKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Employee Deleted!");
                    Con.Close();
                    DisplayEmployees();
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
            string Query = "Select * from Employeetbl where EmpName like '%" + EmpSearchTb.Text + "%'";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            EmployeesDGV.DataSource = ds.Tables[0];
            EmployeesDGV.Columns[0].HeaderCell.Value = "ID";
            EmployeesDGV.Columns[1].HeaderCell.Value = "Name";
            EmployeesDGV.Columns[2].HeaderCell.Value = "Phone";
            EmployeesDGV.Columns[3].HeaderCell.Value = "Date of birth";
            EmployeesDGV.Columns[4].HeaderCell.Value = "Address";
            EmployeesDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }
        private void Searchbtn_Click(object sender, EventArgs e)
        {
            DisplaySearch();
        }

        private void EmpRSbtn_Click(object sender, EventArgs e)
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
