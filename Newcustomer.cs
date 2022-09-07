using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormMicrosoft1
{
    public partial class Newcustomer : Form
    {
        private int parsedCustomerID;
        private int orderID;

        public Newcustomer()
        {
            InitializeComponent();
        }

        private bool IsCustomerNameValid()
        {
            if (txtCustomerName.Text == "")
            {
                MessageBox.Show("please enter the name");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsOrderDataValid()
        {
            if (txtCustomerID.Text == "")
            {
                MessageBox.Show("please create customer account before placing order");
                return false;
            }
            else if (numOrderAmount.Value < 1)
            {
                MessageBox.Show("please specify order amount");
                return false;
            }
            else
            {
                return true;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAddAnotherAccount_Click(object sender, EventArgs e)
        {
            this.ClearForm();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (IsCustomerNameValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    using(SqlCommand sqlCommand=new SqlCommand("Sale.uspNewCustomer", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerName",SqlDbType.NVarChar,40));
                        sqlCommand.Parameters["@CustomerName"].Value = txtCustomerName.Text;

                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Direction = ParameterDirection.Output;
                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                            this.parsedCustomerID = (int)sqlCommand.Parameters["@CustomerID"].Value;
                            this.txtCustomerID.Text = Convert.ToString(parsedCustomerID);

                        }
                        catch
                        {
                            MessageBox.Show("customer ID was not returned, Account could not be created");
                            
                        }
                        finally
                        {
                            connection.Close();
                        }

                    }
                }
            }
        }
        private void ClearForm()
        {
            txtCustomerName.Clear();
            txtCustomerID.Clear();
            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;
            this.parsedCustomerID = 0;
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderDataValid())
            {
                using(SqlConnection connection=new SqlConnection(Properties.Settings.Default.connString))
                {
                    using(SqlCommand sqlCommand=new SqlCommand("Sale.uspPlaceNewOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID",SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Value = this.parsedCustomerID;

                        sqlCommand.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime,8));
                        sqlCommand.Parameters["OrderDate"].Value = dtpOrderDate.Value;

                        sqlCommand.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Int));
                        sqlCommand.Parameters["@Amount"].Value = numOrderAmount.Value;

                        sqlCommand.Parameters.Add(new SqlParameter("@Status", SqlDbType.Char, 1));
                        sqlCommand.Parameters["@Status"].Value = "o";

                        sqlCommand.Parameters.Add(new SqlParameter("@RC", SqlDbType.Int));
                        sqlCommand.Parameters["@RC"].Direction = ParameterDirection.ReturnValue;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();

                            this.orderID = (int)sqlCommand.Parameters["@RC"].Value;
                            MessageBox.Show("Order number "+this.orderID+" has been submitted.");
                        }
                        catch
                        {
                            MessageBox.Show("Order could not be placed");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnAddFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
