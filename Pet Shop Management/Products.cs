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
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
            DisplayProducts();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=LAPTOP-3879ECGQ;Initial Catalog=PetShopManagement;Integrated Security=True");

        private void DisplayProducts()
        {
            Con.Open();
            string Query = "SELECT ProductTbl.PrId, ProductTbl.PrName, CategoryTbl.CatName, ProductTbl.PrQty, ProductTbl.PrPrice\r\nFROM ProductTbl, CategoryTbl\r\nWHERE ProductTbl.CatId=CategoryTbl.CatId";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            ProductsDGV.DataSource = ds.Tables[0];
            ProductsDGV.Columns[0].HeaderCell.Value = "ID";
            ProductsDGV.Columns[1].HeaderCell.Value = "Name";
            ProductsDGV.Columns[2].HeaderCell.Value = "Category";
            ProductsDGV.Columns[3].HeaderCell.Value = "Quantity";
            ProductsDGV.Columns[4].HeaderCell.Value = "Price";
            ProductsDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }

        int Key = 0;
        private void ResetFields()
        {
            PrNameTb.Clear();
            CatCb.SelectedIndex = -1;
            QtyTb.Clear();
            PriceTb.Clear();
            Key = 0;
        }

        private void ProductsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = ProductsDGV.CurrentRow.Index;
            PrNameTb.Text = ProductsDGV.Rows[i].Cells[1].Value.ToString();
            CatCb.Text = ProductsDGV.Rows[i].Cells[2].Value.ToString();
            QtyTb.Text = ProductsDGV.Rows[i].Cells[3].Value.ToString();
            PriceTb.Text = ProductsDGV.Rows[i].Cells[4].Value.ToString();

            if (PrNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(ProductsDGV.Rows[i].Cells[0].Value.ToString());
            }
        }

        string namePattern = @"^[a-zA-Z\s]*$";
        private void Savebtn_Click(object sender, EventArgs e)
        {
            bool isNameValid = Regex.IsMatch(PrNameTb.Text, namePattern);

            if (PrNameTb.Text == "" || !isNameValid)
            {
                MessageBox.Show("Please enter a valid name!");
                PrNameTb.Focus();
            }
            else if (QtyTb.Text == "")
            {
                MessageBox.Show("Please enter a valid quantity number!");
                QtyTb.Focus();
            }
            else if (PriceTb.Text == "")
            {
                MessageBox.Show("Please enter a valid price!");
                PriceTb.Focus();
            } else if(CatCb.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category!");
            }
            else
            {
                try
                {
                    Con.Open();
                    string Query = "Select * from ProductTbl where PrName = '" + PrNameTb.Text + "' and CatId = '" + CatCb.SelectedValue + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                    SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                    var ds = new DataSet();
                    Con.Close();

                    if (sda.Fill(ds) == 0)
                    {
                        Con.Open();
                        string strInsert = "Insert into ProductTbl (PrName, CatId, PrQty, PrPrice) values (@PN, @PC, @PQ, @PP)";
                        SqlCommand com = new SqlCommand(strInsert, Con);

                        SqlParameter p1 = new SqlParameter("@PN", SqlDbType.NVarChar);
                        p1.Value = PrNameTb.Text;
                        SqlParameter p2 = new SqlParameter("@PC", SqlDbType.Int);
                        p2.Value = CatCb.SelectedValue;
                        SqlParameter p3 = new SqlParameter("@PQ", SqlDbType.Int);
                        p3.Value = QtyTb.Text;
                        SqlParameter p4 = new SqlParameter("@PP", SqlDbType.Int);
                        p4.Value = PriceTb.Text;

                        com.Parameters.Add(p1);
                        com.Parameters.Add(p2);
                        com.Parameters.Add(p3);
                        com.Parameters.Add(p4);
                        com.ExecuteNonQuery();

                        MessageBox.Show("Product Added!!");
                        Con.Close();
                        ResetFields();
                        DisplayProducts();
                    }
                    else
                    {
                        MessageBox.Show("Products already exists!");
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
            bool isNameValid = Regex.IsMatch(PrNameTb.Text, namePattern);

            if (Key == 0)
            {
                MessageBox.Show("Select an product!");
            }
            else
            {
                if (PrNameTb.Text == "" || !isNameValid)
                {
                    MessageBox.Show("Please enter a valid name!");
                    PrNameTb.Focus();
                }
                else if (QtyTb.Text == "")
                {
                    MessageBox.Show("Please enter a valid quantity number!");
                    QtyTb.Focus();
                }
                else if (PriceTb.Text == "")
                {
                    MessageBox.Show("Please enter a valid price!");
                    PriceTb.Focus();
                }
                else if (CatCb.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a category!");
                }
                else
                {
                    try
                    {
                        Con.Open();
                        string Query = "Select * from ProductTbl where PrName = '" + PrNameTb.Text + "' and CatId = '" + CatCb.SelectedValue + "' and PrQty = '" + QtyTb.Text + "' and PrPrice = '" + PriceTb.Text + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                        SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
                        var ds = new DataSet();
                        Con.Close();

                        if (sda.Fill(ds) == 0)
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand("Update ProductTbl set PrName=@PN , CatId=@PC ,PrQty=@PQ ,PrPrice=@PP where PrId=@EKey", Con);
                            cmd.Parameters.AddWithValue("@PN", PrNameTb.Text);
                            cmd.Parameters.AddWithValue("@PC", CatCb.SelectedValue);
                            cmd.Parameters.AddWithValue("@PQ", QtyTb.Text);
                            cmd.Parameters.AddWithValue("@PP", PriceTb.Text);
                            cmd.Parameters.AddWithValue("@EKey", Key);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Product Updated!");
                            Con.Close();
                            DisplayProducts();
                            ResetFields();
                        }
                        else
                        {
                            MessageBox.Show("Products already exists!");
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
                MessageBox.Show("Select an product!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from ProductTbl where PrId=@EKey", Con);
                    cmd.Parameters.AddWithValue("@EKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product Deleted!");
                    Con.Close();
                    DisplayProducts();
                    ResetFields();
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
            string Query = "Select * from ProductTbl where PrName like '%" + SearchTb.Text + "%'";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            ProductsDGV.DataSource = ds.Tables[0];
            ProductsDGV.Columns[0].HeaderCell.Value = "ID";
            ProductsDGV.Columns[1].HeaderCell.Value = "Name";
            ProductsDGV.Columns[2].HeaderCell.Value = "Category";
            ProductsDGV.Columns[3].HeaderCell.Value = "Quantity";
            ProductsDGV.Columns[4].HeaderCell.Value = "Price";
            ProductsDGV.Columns[0].FillWeight = 20;
            Con.Close();
        }


        private void QtyTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void PriceTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Products_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'petShopManagementDataSet2.CategoryTbl' table. You can move, or remove it, as needed.
            this.categoryTblTableAdapter.Fill(this.petShopManagementDataSet2.CategoryTbl);

        }

        private void RSbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
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

        private void label16_Click(object sender, EventArgs e)
        {
            Login Obj = new Login();
            Obj.Show();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
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
