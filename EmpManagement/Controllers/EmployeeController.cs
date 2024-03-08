using EmpManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpManagement.Controllers
{
    public class EmployeeController : Controller
    {
        //-------------------------List Of Employee--------------------------
        // GET: Employee
        public ActionResult Index()
        {
            EmployeeDBHandle dbhandle = new EmployeeDBHandle();
            ModelState.Clear();
            return View(dbhandle.GetEmployees());
        }

        //-------------------------Create Empployee----------------------------
        // GET: Employee/Create
        public ActionResult Create()
        {
            EmployeeDBHandle dBHandle = new EmployeeDBHandle();
            List<PayHeadsModel> payHeads = dBHandle.GetPayHeads();
            ViewBag.PayHeads = payHeads;

            var departments = dBHandle.GetDepartments();

            var departmentList = new SelectList(departments, "Department_ID", "Department_Name");

            var viewModel = new CreateEmployeeViewModel
            {
                Employee = new EmployeeModel(),
                DepartmentList = departmentList,
                PayHeads = payHeads
            };
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(EmployeeModel employee, List<int> selectedPayHeads, CreateEmployeeViewModel viewModel)
        {
            EmployeeDBHandle employeeDBHandle = new EmployeeDBHandle();
            try
            {
                
                employee.Employee_ID = employeeDBHandle.AddEmployee(employee);
                List<int> paymentHeadId = selectedPayHeads ?? new List<int>();
                int selectedDepartmentId = viewModel.Employee.Departement_ID;

                if (employee.Employee_ID != 0)
                {
                    int employeeId = employee.Employee_ID;
                    foreach (int payHeadId in paymentHeadId)
                    {
                        employeeDBHandle.AddEmployeePaymentHead(employeeId, payHeadId);
                    }
                    ViewBag.Message = "Employee Details Added Successfully...";
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to add Employee";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while adding employee details: " + ex.Message;
            }
            EmployeeDBHandle dBHandle = new EmployeeDBHandle();
            List<PayHeadsModel> payHeads = dBHandle.GetPayHeads();
            ViewBag.PayHeads = payHeads;
            var departments = dBHandle.GetDepartments();
            viewModel.DepartmentList = new SelectList(departments, "Department_ID", "Department_Name");
            return View(employee);
        }

        //--------------------------Edit Employee--------------------------------------------
        // GET: Employee/Edit
        public ActionResult Edit(int id, int? payHeadId)
        {
            EmployeeDBHandle dbhandle = new EmployeeDBHandle();
            List<PayHeadsModel> selectedpayHeads = dbhandle.GetSelectedPayHeads(id, payHeadId ?? 0);
            //ViewBag.PayHeads = SelectedpayHeads;

            List<PayHeadsModel> allpayHeads = dbhandle.GetPayHeads();

            var departments = dbhandle.GetDepartments();

            var departmentList = new SelectList(departments, "Department_ID", "Department_Name");

            var employeeModel = dbhandle.GetEmployees().Find(employee => employee.Employee_ID == id);

            var editEmployee = new EditEmployeeViewModel
            {
                Employee = new EmployeeModel
                {
                    Employee_ID = employeeModel.Employee_ID,
                    Departement_ID = employeeModel.Departement_ID,
                    Name = employeeModel.Name,
                    Salary = employeeModel.Salary
                },
                DepartmentList = departmentList,
                AllpayHeads = allpayHeads,
                SelectedpayHeads = selectedpayHeads,
            };

            return View(editEmployee);
        }

        // POST: Employee/Edit
        [HttpPost]
        public ActionResult Edit(int id, EmployeeModel employee, List<int> selectedPayHeads, EditEmployeeViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedPaymentHeadsXml))
                {
                    EmployeeDBHandle employeeDBHandle = new EmployeeDBHandle();

                    employeeDBHandle.ParseAndInsertXmlData(viewModel);

                    employee.Employee_ID = employeeDBHandle.UpdateEmployee(employee, viewModel.SelectedPaymentHeadsXml);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = "Failed to update employee";
                EmployeeDBHandle dbHandle = new EmployeeDBHandle();
                List<PayHeadsModel> payHeads = dbHandle.GetPayHeads();
                ViewBag.PayHeads = payHeads;
                var departments = dbHandle.GetDepartments();
                viewModel.DepartmentList = new SelectList(departments, "Department_ID", "Department_Name");
                return View(viewModel);
            }

        }

        //--------------------------Delete Employee---------------------------------------
        // GET: Employee/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                EmployeeDBHandle dbhandle = new EmployeeDBHandle();
                if (dbhandle.DeleteEmployee(id))
                {
                    ViewBag.AlertMsg = "Empployee Deleted Successfully";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
