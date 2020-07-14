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
        private List<Product> _productList;

        public InMemeoryRepository()
        {
            _productList = new List<Product>()
            {

            };
        }

        public IEnumerable<Product> GetAll()
        {
            return _productList.ToList();
        }

        public Product Get(int id)
        {
            return _productList.FirstOrDefault(e => e.Id == id);
        }


        public void Save(Product item)
        {
            
            Delete(item.Id);
            _productList.Add(item);
        }

        public void Delete(int id)
        {
            Product emp = _productList.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                _productList.Remove(emp);
            }
        }

    }
}
