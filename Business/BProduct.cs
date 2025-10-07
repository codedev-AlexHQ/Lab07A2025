using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Entity;

namespace Business
{
    public class BProduct
    {
        private readonly DProduct _data = new DProduct();

        public List<Product> Read(bool? onlyActive = null) => _data.Read(onlyActive);
        public Product? GetById(int id) => _data.GetById(id);
        public void Create(Product product) => _data.Create(product);
        public void Update(Product product) => _data.Update(product);
        public int Delete(int id) => _data.Delete(id); // lógica
        public int HardDelete(int id, bool force = false) => _data.HardDelete(id, force); // física
        public void Reactivate(int id) => _data.Reactivate(id);
    }
}
