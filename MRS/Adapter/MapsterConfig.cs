using System;
using System.Collections.Generic;
using MRS.ViewModels;
using Mapster;
using MRS.Model;
using MRS.ViewModels;
using Newtonsoft.Json;

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
                                .Map(dest => dest.Ordered, src => src.WareHouse != null ? src.WareHouse.Ordered : 0)
                                .Map(dest => dest.Images, src => src.Images != null ? JsonConvert.DeserializeObject<List<string>>(src.Images) : null);
            TypeAdapterConfig<User, AccountVM>.NewConfig()
                                .Map(dest => dest.Rank, src => GetUserLevelString(src.Level));
            TypeAdapterConfig<User, AccountVMById>.NewConfig()
                                .Map(dest => dest.Rank, src => GetUserLevelString(src.Level));
            TypeAdapterConfig<Order, OrderVM>.NewConfig()
                                .Map(dest => dest.Status, src => GetOrderString(src.Status));
            TypeAdapterConfig<Order, OrderDetailsVM>.NewConfig()
                                .Map(dest => dest.Status, src => GetOrderString(src.Status));
        }
        public string GetUserLevelString(int i)
        {
            if (i == (int)UserLevel.Gold)
            {
                return nameof(UserLevel.Gold);
            }
            else if (i == (int)UserLevel.Member)
            {
                return nameof(UserLevel.Member);
            }
            else
            {
                return nameof(UserLevel.Platinum);
            }
        }
        public string GetOrderString(int i)
        {
            if (i == (int)OrderStatus.processing)
            {
                return nameof(OrderStatus.processing);
            }
            else if (i == (int)OrderStatus.confirmed)
            {
                return nameof(OrderStatus.confirmed);
            }
            else if(i == (int)OrderStatus.done)
            {
                return nameof(OrderStatus.done);
            }
            else if(i == (int)OrderStatus.refuse)
            {
                return nameof(OrderStatus.refuse);
            }
            else
            {
                return "Order Status fail";
            }
        }
    }
}
