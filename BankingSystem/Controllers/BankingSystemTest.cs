using BankingSystem.DAL;
using Microsoft.AspNetCore.Mvc;
using BankingSystem.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BankingSystemTest : ControllerBase
{
    // GET: api/<BankingSystemTest>/djohn
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankUser))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Login(String userName)
    {
        DataAccessLayer dal = DataAccessLayer.Instance;
        var user = dal.GetUser(userName);
        return (user == null) ? NotFound() : Ok(user);
    }

/*
    // GET api/<BankingSystemTest>/5
    [HttpGet("{username}")]
    public string Get(String username)
    {
        return "value";
    }

    // POST api/<BankingSystemTest>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<BankingSystemTest>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<BankingSystemTest>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
*/
}
