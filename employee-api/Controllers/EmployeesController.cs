using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Manager.Models;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeRepository repository, ILogger<EmployeesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get([FromQuery(Name = "byAge")]int? age, [FromQuery(Name = "byGender")] Gender? gender, [FromQuery(Name = "byName")] string name)
        {
            using var scope = _logger.BeginScope(new { age, gender, name });

            try
            {
                _logger.LogInformation("Listando funcionários");

                if (age.HasValue)
                    return await _repository.GetByAge(age.Value);

                if (gender.HasValue)
                    return await _repository.GetByGender(gender.Value);

                if (!string.IsNullOrWhiteSpace(name))
                    return await _repository.GetByName(name);

                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao pesquisar os funcionários");
                return Problem(ex.Message);
            }
        }

        // GET employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                _logger.LogInformation($"Recuperando o funcionário de id={id}");

                if (id > 0)
                {
                    var employee = await _repository.GetAsync(id);
                    if (employee == null)
                    {
                        return NotFound();
                    }

                    return Ok(employee);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao pesquisar o funcionário com id={id}");
                return Problem(ex.Message);
            }
        }

        // POST employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using var scope = CreateScope(employee);

            try
            {
                _logger.LogInformation("Criando um funcionário");

                if (ModelState.IsValid)
                {
                    employee.EmployeeId = 0;
                    await _repository.AddAsync(employee);                    
                    return Ok(employee);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao criar um funcionário");
                return Problem(ex.Message);
            }
        }

        // PUT employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            using var scope = CreateScope(employee);

            try
            {
                if (id <= 0)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"Atualizando o funcionário de id {id}");

                    if (!await _repository.ExistAsync(id))
                        return NotFound();

                    employee.EmployeeId = id;
                    await _repository.UpdateAsync(employee);

                    return Ok(employee);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar um funcionário");
                return Problem(ex.Message);
            }
        }

        // DELETE employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    _logger.LogInformation($"Deletando o funcionário de id {id}");

                    var result = await _repository.DeleteAsync(id);
                    if (result == 0)
                    {
                        return NotFound();
                    }

                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao deletar o funcionário de id={id}");
                return Problem(ex.Message);
            }
        }

        private IDisposable CreateScope(Employee employee)
            => _logger.BeginScope(JsonSerializer.Serialize(employee));
    }
}
