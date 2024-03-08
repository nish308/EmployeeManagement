using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;

namespace EmpManagement.Models
{
    public class EmployeeDBHandle
    {
        private SqlConnection con;
        private void connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["EmployeeConn"].ToString();
            con = new SqlConnection(constring);
        }

        //-----------------------------Add Employee------------------------------------------

        public int AddEmployee(EmployeeModel employee)
        {
            connection();
            SqlCommand cmd = new SqlCommand("AddEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Department_ID", employee.Departement_ID);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Salary", employee.Salary);

            SqlParameter Outparam1 = new SqlParameter();
            Outparam1.SqlDbType = SqlDbType.Int;
            Outparam1.ParameterName = "@Employee_ID";
            Outparam1.Size = 20;
            Outparam1.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(Outparam1);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            int employeeId = Convert.ToInt32(cmd.Parameters["@Employee_ID"].Value);

            return employeeId;
        }

        //-----------------------------View Details Of Employee------------------------------------

        public List<EmployeeModel> GetEmployees()
        {
            connection();
            List<EmployeeModel> employeeList = new List<EmployeeModel>();

            SqlCommand cmd = new SqlCommand("GetAllEmployess", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                EmployeeModel employee = new EmployeeModel();

                if (dr["Employee_ID"] != DBNull.Value)
                {
                    employee.Employee_ID = Convert.ToInt32(dr["Employee_ID"]);
                }
                if (dr["Department_ID"] != DBNull.Value)
                {
                    employee.Departement_ID = Convert.ToInt32(dr["Department_ID"]);
                }
                if (dr["Name"] != DBNull.Value)
                {
                    employee.Name = Convert.ToString(dr["Name"]);
                }
                if (dr["Salary"] != DBNull.Value)
                {
                    employee.Salary = Convert.ToString(dr["Salary"]);
                }
                employeeList.Add(employee);
            }
            return employeeList;
        }

        //-----------------------------Update Employee------------------------------------

        public int UpdateEmployee(EmployeeModel employee, string SelectedPaymentHeadsXml)
        {
            connection();
            SqlCommand cmd = new SqlCommand("UpdateEmp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Employee_ID", employee.Employee_ID).Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("@Department_ID", employee.Departement_ID);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Salary", employee.Salary);

            string xmlData = string.Join(",", SelectedPaymentHeadsXml);
            cmd.Parameters.AddWithValue("@xmlData", xmlData);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            // Retrieve the updated Employee_ID value
            int employeeId = Convert.ToInt32(cmd.Parameters["@Employee_ID"].Value);

            return employeeId;
        }

        //--------------------------------Parse Xml Data----------------------------------

        public void ParseAndInsertXmlData(EditEmployeeViewModel viewModel)
        {
            System.Diagnostics.Trace.WriteLine("Recieved xml data: " + viewModel.SelectedPaymentHeadsXml);
            connection();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(viewModel.SelectedPaymentHeadsXml);

            con.Open();
            foreach (XmlNode node in xmlDoc.SelectNodes("//payhead"))
            {
                string xmlData = Convert.ToString(node.Attributes["id"].Value);

                SqlCommand cmd = new SqlCommand("spInsertPayHeads", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@xmlData", xmlData);
                cmd.Parameters.AddWithValue("@Employee_ID", viewModel.Employee.Employee_ID);
                cmd.ExecuteNonQuery();
            }
        }


        //-----------------------------Delete Employee------------------------------------

        public bool DeleteEmployee(int id)
        {
            connection();
            SqlCommand cmd = new SqlCommand("DeleteEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Employee_ID", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //-------------------------Add Multiple Payheads------------------------------------

        public List<PayHeadsModel> GetPayHeads()
        {
            connection();
            List<PayHeadsModel> payheadList = new List<PayHeadsModel>();

            SqlCommand cmd = new SqlCommand("GetAllPayHeads", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                PayHeadsModel payHeads = new PayHeadsModel();

                if (dr["Payment_Head_ID"] != DBNull.Value)
                {
                    payHeads.Payment_Head_ID = Convert.ToInt32(dr["Payment_Head_ID"]);
                }
                if (dr["Payment_Head_Name"] != DBNull.Value)
                {
                    payHeads.Payment_Head_Name = Convert.ToString(dr["Payment_Head_Name"]);
                }
                payheadList.Add(payHeads);
            }
            return payheadList;
        }

        //-------------------------Add EmployeePayhead---------------------------------------------------------------

        public void AddEmployeePaymentHead(int employeeId, int payHeadId)
        {
            try
            {
                con.Open();

                string query = "insert into EmployeePaymentHead (Employee_ID, Payment_Head_ID) values (@Employee_ID, @Payment_Head_ID)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Employee_ID", employeeId);
                    cmd.Parameters.AddWithValue("@Payment_Head_ID", payHeadId);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                con.Close();
            }
        }

        //-------------------------Get Department List---------------------------------------------------------------

        public List<Department> GetDepartments()
        {
            connection();
            List<Department> departments = new List<Department>();

            SqlCommand cmd = new SqlCommand("GetAllDepartments", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Department department = new Department
                {
                    Department_ID = Convert.ToInt32(reader["Department_ID"]),
                    Department_Name = reader["Department_Name"].ToString()
                };
                departments.Add(department);
            }
            reader.Close();
            return departments;
        }

        //-------------------------Get Employee By Id ---------------------------------------------------------------

        public EmployeeModel GetEmployeeByID(int employeeId)
        {
            EmployeeModel employeeModel = new EmployeeModel();

            SqlCommand cmd = new SqlCommand("GetEmployeeByID", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Employee_ID", employeeId);

            con.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    employeeModel = new EmployeeModel
                    {
                        Employee_ID = Convert.ToInt32(reader["Employee_ID"]),
                        Departement_ID = Convert.ToInt32(reader["Department_ID"]),
                        Name = reader["Name"].ToString(),
                        Salary = reader["Salary"].ToString()
                    };
                }
            }
            return employeeModel;

        }

        //-------------------------Get Selected PayHead List--------------------------------------------------------------

        public List<PayHeadsModel> GetSelectedPayHeads(int employeeId, int payHeadId)
        {
            connection();
            List<PayHeadsModel> payHeads = new List<PayHeadsModel>();

            con.Open();
            SqlCommand cmd = new SqlCommand("GetSelectedPayHead", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Employee_ID", employeeId);
            cmd.Parameters.AddWithValue("@Payment_Head_ID", payHeadId);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PayHeadsModel payHeadsModel = new PayHeadsModel
                        {
                            Payment_Head_ID = Convert.ToInt32(reader["Payment_Head_ID"]),
                            Payment_Head_Name = reader["Payment_Head_Name"].ToString()

                        };
                        payHeads.Add(payHeadsModel);
                    }
                }
                else
                {
                    Console.WriteLine("No Rows Returned from the query");
                }

            }
            return payHeads;
        }
    }
}