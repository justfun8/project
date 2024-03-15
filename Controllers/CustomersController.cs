using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Services;
using Microsoft.AspNetCore.Mvc;
using customers.Models;

namespace customers.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _customersService;

        public CustomersController(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [HttpPost("customer/{customerId}/score/{score}")]
        public ActionResult<int> AddOrUpdateScore(long customerId, int score)
        {
            try
            {
                var currentScore = _customersService.AddOrUpdateCustomerScore(customerId, score);
                return Ok(currentScore); 
            }
            catch (ArgumentOutOfRangeException ex)
            {
                
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while updating score: {ex.Message}");
            }
        }

        [HttpGet("Leaderboard")]
        public ActionResult<List<CustomerDto>> Get([FromQuery] int? start, [FromQuery] int? end)
        {
            try
            {
                var customerDtos = _customersService.GetCustomersByRange(start, end);
                return Ok(customerDtos);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Leaderboard/{customerId}")]
        public ActionResult<List<CustomerDto>> Get(
            int customerId,
            [FromQuery] int high,
            [FromQuery] int low
        )
        {
            try
            {
                var customerDtos = _customersService.GetCustomersAroundCustomerId(
                    customerId,
                    high,
                    low
                );
                return Ok(customerDtos);
            }
            // catch (CustomerNotFoundException ex)
            // {
            //     return NotFound(ex.Message);
            // }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}


