using System;
using System.Collections;
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
    public partial class Bill : Form
    {
        public Bill()
        {
            InitializeComponent();
            DisplayProduct();
            GetCustomers();
            GetEmployees();
            DisplayTransaction();
        }

        SqlConnection Con = new SqlConnection(@"Data Source=LAPTOP-3879ECGQ;Initial Catalog=PetShopManagement;Integrated Security=True");

        private void GetCustomers()
        {
            Con.Open();
            SqlCommand cmd = new SqlCommand("Select CustId from CustomerTbl", Con);
            SqlDataReader Rdr;
            Rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("CustId", typeof(int));
            dt.Load(Rdr);
            CustIdCb.ValueMember = "CustId";
            CustIdCb.DataSource = dt;
            Con.Close();
        }

        private void GetCustName()
        {
            
            string Query = "Select * from CustomerTbl where CustId='" + CustIdCb.SelectedValue.ToString() + "'";
            SqlCommand cmd = new SqlCommand(Query, Con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                CustNameTb.Text = dr["CustName"].ToString();
            }
            Con.Close();
        }

        private void CustIdCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCustName();
        }


        private void GetEmployees()
        {
            Con.Open();
            SqlCommand cmd = new SqlCommand("Select EmpNum from EmployeeTbl", Con);
            SqlDataReader Rdr;
            Rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("EmpNum", typeof(int));
            dt.Load(Rdr);
            EmpIdCb.ValueMember = "EmpNum";
            EmpIdCb.DataSource = dt;
            Con.Close();
        }

        private void GetEmpName()
        {
            string Query = "Select * from EmployeeTbl where EmpNum='" + EmpIdCb.SelectedValue.ToString() + "'";
            SqlCommand cmd = new SqlCommand(Query, Con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                EmpNameTb.Text = dr["EmpName"].ToString();
            }
            Con.Close();
        }

        private void EmpIdCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetEmpName();
        }

        private void DisplayProduct()
        {
            Con.Open();
            string Query = "SELECT ProductTbl.PrId, ProductTbl.PrName, CategoryTbl.CatName, ProductTbl.PrQty, ProductTbl.PrPrice\r\nFROM ProductTbl, CategoryTbl\r\nWHERE ProductTbl.CatId=CategoryTbl.CatId";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            ProductDGV.DataSource = ds.Tables[0];
            ProductDGV.Columns[0].HeaderCell.Value = "ID";
            ProductDGV.Columns[1].HeaderCell.Value = "Name";
            ProductDGV.Columns[2].HeaderCell.Value = "Category";
            ProductDGV.Columns[3].HeaderCell.Value = "Quantity";
            ProductDGV.Columns[4].HeaderCell.Value = "Price";
            ProductDGV.Columns[0].FillWeight = 30;
            Con.Close();
        }

        private void DisplayTransaction()
        {
            Con.Open();
            string Query = "SELECT BillTbl.BNum, CustomerTbl.CustName, ProductTbl.PrName, EmployeeTbl.EmpName, BillTbl.Qty, BillTbl.Price, BillTbl.BDateOP, BillTbl.Total\r\nFROM BillTbl, CustomerTbl, ProductTbl, EmployeeTbl\r\nWHERE BillTbl.CustId=CustomerTbl.CustId and BillTbl.PrId = ProductTbl.PrId and BillTbl.EmpNum = EmployeeTbl.EmpNum";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder Builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            TransactionDGV.DataSource = ds.Tables[0];
            TransactionDGV.Columns[0].HeaderCell.Value = "ID";
            TransactionDGV.Columns[1].HeaderCell.Value = "Customer";
            TransactionDGV.Columns[2].HeaderCell.Value = "Product";
            TransactionDGV.Columns[3].HeaderCell.Value = "Employee";
            TransactionDGV.Columns[4].HeaderCell.Value = "Quantity";
            TransactionDGV.Columns[5].HeaderCell.Value = "Price";
            TransactionDGV.Columns[6].HeaderCell.Value = "Date of payment";
            TransactionDGV.Columns[7].HeaderCell.Value = "Total";
            TransactionDGV.Columns[0].FillWeight = 30;
            Con.Close();
        }

        int Key = 0, Stock = 0;
        int n = 0, GrdTotal = 0;
        private void ResetFields()
        {
            PrNameTb.Text = "";
            QtyTb.Text = "";
            PrPriceTb.Text = "";
            Stock = 0;
            Key = 0;
        }

        private void UpdateStock()
        {
            try
            {
                int NewQty = Stock - Convert.ToInt32(QtyTb.Text);
                Con.Open();
                SqlCommand cmd = new SqlCommand("Update ProductTbl set prQty=@PQ where PrId=@PKey", Con);
                cmd.Parameters.AddWithValue("@PQ", NewQty);
                cmd.Parameters.AddWithValue("@PKey", Key);

                cmd.ExecuteNonQuery();

                Con.Close();
                DisplayProduct();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (Key == 0)
            {
                MessageBox.Show("Select an product!");
            }
            else
            {
                if (QtyTb.Text == "")
                {
                    MessageBox.Show("Please enter quantity!");
                }
                else if (Convert.ToInt32(QtyTb.Text) > Stock && Stock != 0)
                {
                    MessageBox.Show("The remaining quantity is not enough!");
                }
                else if (Stock == 0)
                {
                    MessageBox.Show("Out of stock!");
                } else if(QtyTb.Text == "0")
                {
                    MessageBox.Show("Quantity must be larger than 0!");
                }
                else
                {
                    try
                    {
                        int total = Convert.ToInt32(QtyTb.Text) * Convert.ToInt32(PrPriceTb.Text);
                        GrdTotal = GrdTotal + total;
                        BillGridDGV.Rows.Add(n + 1, PrNameTb.Text, QtyTb.Text, PrPriceTb.Text, total);

                        n++;
                        TotalLbl.Text = "Rs: " + GrdTotal;
                        UpdateStock();

                        Con.Open();
                        SqlCommand cmd = new SqlCommand("Insert into BillTbl (CustId, PrId, EmpNum, Qty, Price, BDateOP, Total) values (@CI, @PI, @EN, @QT, @PR, @BD, @TT)", Con);
                        cmd.Parameters.AddWithValue("@CI", CustIdCb.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@PI", Key);
                        cmd.Parameters.AddWithValue("@EN", EmpIdCb.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@QT", QtyTb.Text);
                        cmd.Parameters.AddWithValue("@PR", PrPriceTb.Text);
                        cmd.Parameters.AddWithValue("@BD", DateTime.Today.Date);
                        cmd.Parameters.AddWithValue("@TT", total);
                        cmd.ExecuteNonQuery();
                        Con.Close();
                        DisplayTransaction();
                        ResetFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void PrintBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bill saved successfully!");
            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("pprnm", 400, 800);

            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void QtyTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        int prodid, prodqty, prodprice, tottal, pos = 80;

        string prodname;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Pet Shop", new Font("Century Gothic", 20, FontStyle.Bold), Brushes.Red, new Point(150));
            e.Graphics.DrawString("ID   PRODUCT                    QUANTITY             PRICE              TOTAL", new Font("Century Gothic", 9, FontStyle.Bold), Brushes.Red, new Point(8, 40));
            foreach (DataGridViewRow row in BillGridDGV.Rows)
            {

                prodid = Convert.ToInt32(row.Cells["Column1"].Value);
                prodname = "" + row.Cells["Column2"].Value;
                prodprice = Convert.ToInt32(row.Cells["Column3"].Value);
                prodqty = Convert.ToInt32(row.Cells["Column4"].Value);
                tottal = Convert.ToInt32(row.Cells["Column5"].Value);
                e.Graphics.DrawString("" + prodid, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(8, pos));
                e.Graphics.DrawString("" + prodname, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(25, pos));
                e.Graphics.DrawString("" + prodprice, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(185, pos));
                e.Graphics.DrawString("" + prodqty, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(260, pos));
                e.Graphics.DrawString("" + tottal, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(345, pos));
                pos += 30;
            }
            e.Graphics.DrawString("Grand Total : Rs " + GrdTotal, new Font("Century Gothic", 12, FontStyle.Bold), Brushes.Crimson, new Point(100, pos + 50));
            e.Graphics.DrawString("*********************PetShop*********************", new Font("Century Gothic", 12, FontStyle.Bold), Brushes.Blue, new Point(10, pos + 185));
            BillGridDGV.Rows.Clear();
            BillGridDGV.Refresh();
            pos += 80;
            GrdTotal = 0;
            n = 0;
        }

        private void ProductDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = ProductDGV.CurrentRow.Index;
            PrNameTb.Text = ProductDGV.Rows[i].Cells[1].Value.ToString();
            Stock = Convert.ToInt32(ProductDGV.Rows[i].Cells[3].Value.ToString());
            PrPriceTb.Text = ProductDGV.Rows[i].Cells[4].Value.ToString();

            if (PrNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(ProductDGV.Rows[i].Cells[0].Value.ToString());
            }
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
    }
}
