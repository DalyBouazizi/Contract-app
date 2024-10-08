﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projet_Data.Features;
using Projet_Data.ModelsEF;
using Projet_Stage.Models;
using Projet_Stage.Services.Classes;
using Projet_Stage.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Projet_Stage.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
            private readonly IEmployeeService _employeeService;
            public EmployeeController(IEmployeeService employeeService)
            {
                _employeeService = employeeService;
            }
            [HttpPost]
            [Route("AddEmployee")]
            public async Task<IActionResult> AddEmployee([FromBody] EmployeeModel employee)
            {
                bool res = false;

                res = await _employeeService.AddEmployeeAsync(employee);
                if (res)
                {
                    return Ok("employee added");
                }
                else
                {
                    return BadRequest("Matricule exists already");
                }
            }
            [Route("GetAllEmployees")]
            [HttpGet]
            public async Task<ActionResult<List<EmployeeModel>>> GetAllEmployeesAsync()
            {
                List<EmployeeModel> employees = new List<EmployeeModel>();
                try
                {
                    employees = await _employeeService.GetAllEmployeesAsync();
                    return Ok(employees);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            [Route("GetEmpployeeById")]
            [HttpGet]
            public async Task<ActionResult<EmployeeGetModel>> GetEmployeeByIdAsync([Required] int IdEmployee)
            {

            EmployeeGetModel user = new EmployeeGetModel();
                try
                {
                    user = await _employeeService.GetEmployeeByIdAsync(IdEmployee);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (user == null)
                {

                    return NotFound("User with id " + IdEmployee + " Not found");
                }
                else
                {
                    return Ok(user);

                }
            }
        [Route("GetEmpployeeByRealId")]
        [HttpGet]
        public async Task<ActionResult<EmployeeGetModel>> GetEmployeeByRealIdAsync([Required] int IdEmployee)
        {

            EmployeeGetModel user = new EmployeeGetModel();
            try
            {
                user = await _employeeService.GetEmployeeByRealIdAsync(IdEmployee);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (user == null)
            {

                return NotFound("User with id " + IdEmployee + " Not found");
            }
            else
            {
                return Ok(user);

            }
        }
        [Route("GetEmpployeeByCin")]
        [HttpGet]
        public async Task<ActionResult<EmployeeModel>> GetEmployeeByCinAsync([Required] string Cin)
        {

            EmployeeModel emp = new EmployeeModel();
            try
            {
                emp = await _employeeService.GetEmployeeByCinAsync(Cin);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (emp == null)
            {

                return NotFound("User with Cin " + Cin + " Not found");
            }
            else
            {
                return Ok(emp);

            }
        }
        [Route("DeleteEmployee")]
            [HttpDelete]
            public async Task<ActionResult<String>> DeleteEmployeeAsync([Required] int IdEmployee)
            {
                bool res = false;

                res = await _employeeService.DeleteEmployeeAsync(IdEmployee);
                if (res)
                {
                    return Ok("Employee deleted");
                }
                else
                {
                    return BadRequest("\"User not with id " + IdEmployee + " not found\"");
                }
            }
            [Route("AddListEmployees")]
            [HttpPost]
            public async Task<ActionResult<string>> AddListEmployeesAsync([FromBody] List<EmployeeModel> employees)
            {
                if (employees == null)
                {
                    return BadRequest("No employees were provided");
                }
                var (isSuccess, failedIds) = await _employeeService.AddListEmployeesAsync(employees);

                if (isSuccess)
                {
                    if (failedIds.Count > 0)
                    {
                        return Ok($"employees added successfully, but employees with the following IDs were not added because they already exist: {string.Join(", ", failedIds)}");
                    }
                    return Ok("All employees added successfully.");
                }
                else
                {
                    return BadRequest("Failed to add employees.");
                }
            }
            [Route("Updateemployee")]
            [HttpPut]
            public async Task<ActionResult<string>> UpdateEmployeeAsync([FromBody] EmployeeModel employee)
            {
                bool res = await _employeeService.UpdateEmployeeAsync(employee);
                if (res)
                {
                    return Ok("Employee updated successfully");
                }
                else
                {
                    return BadRequest("Employee not found or update failed");
                }
            }
            [HttpGet("GetEmployeesByPoste")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployeesByPoste(string role)
            {
                var employees = await _employeeService.GetEmployeesByPosteAsync(role);
                if (employees == null || !employees.Any())
                {
                    return NotFound("No employees found with the specified role.");
                }
                return Ok(employees);
            }
            [HttpGet("SortEmployeesById")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> SortEmployeesById(bool ascending)
            {
                var employees = await _employeeService.SortEmployeesByIdAsync(ascending);
                return Ok(employees);
            }
            [HttpGet("SortEmployeesByPoste")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> SortEmployeesByPoste(bool ascending)
            {
                var employees = await _employeeService.SortEmployeesByPosteAsync(ascending);
                return Ok(employees);
            }
            [HttpGet("SortEmployeesBySalary")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> SortEmployeesBySalary(bool over, decimal salaryValue)
            {
                var employees = await _employeeService.SortEmployeesBySalaryAsync(salaryValue, over);
                return Ok(employees);
            }
            [HttpGet("SortEmployeesByFName")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> SortEmployeesByFirstName(bool ascending)
            {
                var employees = await _employeeService.SortEmployeesByFirstNameAsync(ascending);
                return Ok(employees);
            }
            [HttpGet("SortEmployeesByLName")]
            public async Task<ActionResult<IEnumerable<EmployeeModel>>> SortEmployeesByLastName(bool ascending)
        {
            var employees = await _employeeService.SortEmployeesByLastNameAsync(ascending);
            return Ok(employees);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> GetEmployeesByFilters([FromQuery] FilterCriteria criteria)
        {
            if (criteria == null)
            {

                return BadRequest("Invalid filter criteria.");
            }

            var employees = await _employeeService.GetEmployeesByFiltersAsync(criteria);

            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found matching the criteria.");
            }

            return Ok(employees);
        }



        




    }
}


