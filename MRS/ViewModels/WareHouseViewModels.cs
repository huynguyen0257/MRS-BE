using System;
namespace MRS.ViewModels
{
    //public Guid Id { get; set; }
    //public int Quantity { get; set; }
    //public int Avaiable { get; set; }
    //public int Ordered { get; set; }
    //public int Purchased { get; set; }
    public class WareHouseVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public Image Image { get; set; }
        public int Quantity { get; set; }
        public int Avaiable { get; set; }
        public int Ordered { get; set; }
        public int Purchased { get; set; }
    }

    //public class WareHouseUM
    //{
    //    public Guid Id { get; set; }
    //    public int Quantity { get; set; }
    //    public int Avaiable { get; set; }
    //    public int Ordered { get; set; }
    //    public int Purchased { get; set; }
    //}
}
