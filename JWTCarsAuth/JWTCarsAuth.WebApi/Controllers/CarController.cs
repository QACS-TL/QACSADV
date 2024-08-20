using JWTCarsAuth.WebApi.Interface;
using JWTCarsAuth.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Controllers
{
    [Authorize]
    [Route("api/cars")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _car;

        public CarController(ICarRepository car)
        {
            _car = car;
        }

        // GET: api/car>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> Get()
        {
            return await Task.FromResult(_car.GetCarDetails());
        }

        // GET api/car/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> Get(int id)
        {
            var cars = await Task.FromResult(_car.GetCarDetails(id));
            if (cars == null)
            {
                return NotFound();
            }
            return cars;
        }

        // POST api/car
        [HttpPost]
        public async Task<ActionResult<Car>> Post(Car car)
        {
            _car.AddCar(car);
            //return await Task.FromResult(CreatedAtAction("GetCars", new { id = car.CarID }, car));
            return await Task.FromResult(car);
        }

        // PUT api/car/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Car>> Put(int id, Car car)
        {
            if (id != car.ID)
            {
                return BadRequest();
            }
            try
            {
                _car.UpdateCar(car);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return await Task.FromResult(car);
        }

        // DELETE api/car/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Car>> Delete(int id)
        {
            var car = _car.DeleteCar(id);
            return await Task.FromResult(car);
        }

        private bool CarExists(int id)
        {
            return _car.CheckCar(id);
        }
    }
}
