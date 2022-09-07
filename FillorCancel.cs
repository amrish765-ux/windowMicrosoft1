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
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace WindowsFormMicrosoft1
{
    public partial class FillorCancel : Form
    {
        private int parseOrderID;
        public FillorCancel()
        {
            InitializeComponent();
        }

        private bool IsvalidOrderId()
        {
            if (txtOrderID.Text=="")
            {
                MessageBox.Show("please specify the Order ID.");
                return false;
            }
            else if (Regex.IsMatch(txtOrderID.Text, @"^\D*$"))
            {
                MessageBox.Show("Order Id must contains numbers only");
                txtOrderID.Clear();
                return false;
            }
            parseOrderID = int.Parse(txtOrderID.Text);
            return true;
        }

        private void FillorCancel_Load(object sender, EventArgs e)
        {

        }

        private void btnFinishUpdates_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsvalidOrderId())
            {
                using(SqlConnection connection=new SqlConnection(Properties.Settings.Default.connString))
                {
                    const string sql= "SELECT * FROM Sale.Orders WHERE orderID = @orderID";
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int));
                        sqlCommand.Parameters["@OrderId"].Value = parseOrderID;

                        try
                        {
                            connection.Open();
                            using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);
                                this.dgvCustomerOrders.DataSource = dataTable;
                                dataReader.Close();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Requested data could not be loaded into database");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsvalidOrderId())
            {
                using(SqlConnection connection=new SqlConnection(Properties.Settings.Default.connString))
                {
                    using(SqlCommand sqlCommand=new SqlCommand("Sale.uspCancelOrder",connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
                        sqlCommand.Parameters["@orderId"].Value = parseOrderID;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Cancel operation was not completed");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnFillOrder_Click(object sender, EventArgs e)
        {
            if (IsvalidOrderId())
            {
                using(SqlConnection connection=new SqlConnection(Properties.Settings.Default.connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("Sale.uspFillOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
                        sqlCommand.Parameters["@orderId"].Value = parseOrderID;

                        sqlCommand.Parameters.Add(new SqlParameter("@filledDate", SqlDbType.DateTime));
                        sqlCommand.Parameters["@filledDate"].Value = dtpFillDate.Value;
                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("The Fill operation was not completed");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }
    }
}
