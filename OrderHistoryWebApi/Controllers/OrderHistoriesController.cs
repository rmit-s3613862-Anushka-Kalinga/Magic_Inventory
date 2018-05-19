using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderHistoryWebApi.Data;
using OrderHistoryWebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderHistoryWebApi.Controllers
{
    [Route("api/[controller]")]
    
    public class OrderHistoriesController : Controller
    {
        private readonly OrderHistoryDbContext _context;
        
        public OrderHistoriesController(OrderHistoryDbContext context) {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<OrderHistory>> Get()
        {
            return await _context.OrderHistory.Include(s => s.Store).Include( o => o.Product).ToListAsync();
        }

        // GET api/<controller>/5
        [HttpGet("{name}")]
        public async Task<IEnumerable<OrderHistory>> GetById(string name)
        {
                var item = _context.OrderHistory.Include(s => s.Store).Include(o => o.Product).Where(u => u.UserName == name);
                    if (item == null)
                    {
                        //  return NotFound();
                    }
            return item;
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
