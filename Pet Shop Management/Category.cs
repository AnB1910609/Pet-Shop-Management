using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management
{
    public partial class Category : Form
    {
        public Category()
        {
            InitializeComponent();
            DisplayCategory();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=LAPTOP-3879ECGQ;Initial Catalog=PetShopManagement;Integrated Security=True");

        private void DisplayCategory()
        {
            Con.Open();
            string Query = "Select * from Categorytbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            CategoryDGV.DataSource = ds.Tables[0];
            CategoryDGV.Columns[0].HeaderCell.Value = "ID";
            CategoryDGV.Columns[1].HeaderCell.Value = "Category name";
            CategoryDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        private void Clear()
        {
            CatNameTb.Text = "";
            Key = 0;
        }

        int Key = 0;

        private void CategoryDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = CategoryDGV.CurrentRow.Index;
            CatNameTb.Text = CategoryDGV.Rows[i].Cells[1].Value.ToString();

            if (CatNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(CategoryDGV.Rows[i].Cells[0].Value.ToString());
            }
        }

        string namePattern = @"^[a-zA-Z\s]*$";
        private void Savebtn_Click(object sender, EventArgs e)
        {
            bool isNameValid = Regex.IsMatch(CatNameTb.Text, namePattern);

            if (CatNameTb.Text == "" || !isNameValid)
            {
                MessageBox.Show("Please enter a valid name!");
                CatNameTb.Focus();
            }
            else
            {
                try
                {
                    Con.Open();
                    string Query = "Select * from CategoryTbl where CatName = '" + CatNameTb.Text + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                    SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                    var ds = new DataSet();
                    Con.Close();

                    if (sda.Fill(ds) == 0)
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand("Insert into CategoryTbl (CatName) values (@CN)", Con);
                        cmd.Parameters.AddWithValue("@CN", CatNameTb.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Category Added!");
                        Con.Close();
                        DisplayCategory();
                        Clear();
                    }
                    else
                    {
                        MessageBox.Show("Category already exists!");
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
            bool isNameValid = Regex.IsMatch(CatNameTb.Text, namePattern);

            if (Key == 0)
            {
                MessageBox.Show("Select An Employee!");
            } else
            {
                if (CatNameTb.Text == "" || !isNameValid)
                {
                    MessageBox.Show("Please enter a valid name!");
                    CatNameTb.Focus();
                }
                else
                {
                    try
                    {
                        Con.Open();
                        string Query = "Select * from CategoryTbl where CatName = '" + CatNameTb.Text + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                        SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                        var ds = new DataSet();
                        Con.Close();

                        if (sda.Fill(ds) == 0)
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand("Update CategoryTbl set CatName=@CN where CatId=@EKey", Con);
                            cmd.Parameters.AddWithValue("@CN", CatNameTb.Text);
                            cmd.Parameters.AddWithValue("@EKey", Key);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Category Updated!");
                            Con.Close();
                            DisplayCategory();
                            Clear();
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
                MessageBox.Show("Select An Category!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from CategoryTbl where CatID=@EKey", Con);
                    cmd.Parameters.AddWithValue("@EKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Category Deleted!");
                    Con.Close();
                    DisplayCategory();
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
            string Query = "Select * from Categorytbl where CatName like '%" + CatSearchTb.Text + "%'";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            CategoryDGV.DataSource = ds.Tables[0];
            CategoryDGV.Columns[0].HeaderCell.Value = "ID";
            CategoryDGV.Columns[1].HeaderCell.Value = "Category name";
            CategoryDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        private void RSbtn_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {
            DisplaySearch();
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
