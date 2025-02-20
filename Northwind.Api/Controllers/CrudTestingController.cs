using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Northwind.Api.Models;
using Northwind.Core.Services;

namespace Northwind.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrudTestingController(INorthwindService<TestClass> northwind) : ControllerBase
    {
        private readonly INorthwindService<TestClass> _northwind = northwind;

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TestClass testEntity)
        {
            var result = await _northwind.CreateAsync(testEntity);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _northwind.DeleteQuery().Where(x => x.Id == id).ExecuteAsync();
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, string newName)
        {
            var result = await _northwind.UpdateQuery()
                .Where(x => x.Id == id)
                .Set(x => x.Name, newName)
                .ExecuteAsync();
            return Ok(result);
        }

        [HttpGet("Read/{id}")]
        public async Task<IActionResult> Read(string id)
        {
            var result = await _northwind.Query().Where(x => x.Name == id).ToListAsync();
            return Ok(result);
        }
    }
}
