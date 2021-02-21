using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Models;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillRepository _repository;
        private readonly ILogger<SkillsController> _logger;

        public SkillsController(ISkillRepository repository, ILogger<SkillsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/<Skills>
        [HttpGet]
        public async Task<ActionResult<List<Skill>>> Get()
        {
            try
            {
                _logger.LogInformation("Listando todas as habilidades");
                return Ok(await _repository.GetAllAsync());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao pesquisar as habilidades");
                return Problem(ex.Message);
            }
        }
    }
}
