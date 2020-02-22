using System;
using Mapster;
using MRS.Model;
using MRS.ViewModels;

namespace MRS.Adapter
{
    public class MapsterConfig
    {
        public void Run()
        {
            TypeAdapterConfig<WareHouse, WareHouseVM>.NewConfig()
                                .Map(dest => dest.Name, src => src.Product.Name);
            TypeAdapterConfig<Product, ProductDetailVM>.NewConfig()
                                .Map(dest => dest.Quantity, src => src.WareHouse != null ? src.WareHouse.Quantity : 0)
                                .Map(dest => dest.Avaiable, src => src.WareHouse != null ? src.WareHouse.Avaiable : 0)
                                .Map(dest => dest.Purchased, src => src.WareHouse != null ? src.WareHouse.Purchased : 0)
                                .Map(dest => dest.Ordered, src => src.WareHouse != null ? src.WareHouse.Ordered : 0);
            //TypeAdapterConfig<ProductUM, Product>.NewConfig()
            //                    .Map(dest => dest.WareHouse.Quantity, src => src.Quantity);
        }
    }
}
