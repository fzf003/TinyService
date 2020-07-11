using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Sample_1.Domain
{
    public interface IInMemeoryRepository
    {
        void Delete(int id);
        Product Get(int id);
        IEnumerable<Product> GetAll();
        void Save(Product item);
    }


    public class InMemeoryRepository : IInMemeoryRepository
    {
        private List<Product> _employeesList;

        public InMemeoryRepository()
        {
            _employeesList = new List<Product>()
            {

            };
        }

        public IEnumerable<Product> GetAll()
        {
            return _employeesList.ToList();
        }

        public Product Get(int id)
        {
            return _employeesList.FirstOrDefault(e => e.Id == id);
        }


        public void Save(Product item)
        {
            // Check and if exists delete Employee with given id
            Delete(item.Id);
            _employeesList.Add(item);
        }

        public void Delete(int id)
        {
            Product emp = _employeesList.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                _employeesList.Remove(emp);
            }
        }

    }
}
