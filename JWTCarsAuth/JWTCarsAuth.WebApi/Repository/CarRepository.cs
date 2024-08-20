using JWTCarsAuth.WebApi.Interface;
using JWTCarsAuth.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTCarsAuth.WebApi.Repository
{
    public class CarRepository : ICarRepository
    {
        readonly DatabaseContext _dbContext = new();

        public CarRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Car> GetCarDetails()
        {
            try
            {
                return _dbContext.Cars.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Car GetCarDetails(int id)
        {
            try
            {
                Car? car = _dbContext.Cars.Find(id);
                if (car != null)
                {
                    return car;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddCar(Car car)
        {
            try
            {
                _dbContext.Cars.Add(car);
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCar(Car car)
        {
            try
            {
                _dbContext.Entry(car).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public Car DeleteCar(int id)
        {
            try
            {
                Car? car = _dbContext.Cars.Find(id);

                if (car != null)
                {
                    _dbContext.Cars.Remove(car);
                    _dbContext.SaveChanges();
                    return car;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckCar(int id)
        {
            return _dbContext.Cars.Any(e => e.ID == id);
        }
    }
}
