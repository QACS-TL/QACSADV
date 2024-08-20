using JWTCarsAuth.WebApi.Models;

namespace JWTCarsAuth.WebApi.Interface
{
    public interface ICarRepository
    {
        public List<Car> GetCarDetails();

        public Car GetCarDetails(int id);

        public void AddCar(Car employee);

        public void UpdateCar(Car employee);

        public Car DeleteCar(int id);

        public bool CheckCar(int id);
    }
}
