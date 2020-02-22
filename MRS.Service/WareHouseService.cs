using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface IWareHouseService
    {
        IEnumerable<WareHouse> GetWareHouses();
        IEnumerable<WareHouse> GetWareHouses(Expression<Func<WareHouse, bool>> where);
        WareHouse GetWareHouse(Guid id);
        void CreateWareHouse(WareHouse WareHouse);
        void EditWareHouse(WareHouse WareHouse);
        void RemoveWareHouse(WareHouse WareHouse);
        void SaveWareHouse();
    }

    public class WareHouseService : IWareHouseService
    {
        private readonly IWareHouseRepository _WareHouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WareHouseService(IWareHouseRepository WareHouseRepository, IUnitOfWork unitOfWork)
        {
            this._WareHouseRepository = WareHouseRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateWareHouse(WareHouse WareHouse)
        {
            _WareHouseRepository.Add(WareHouse);
        }

        public void EditWareHouse(WareHouse WareHouse)
        {
            _WareHouseRepository.Update(WareHouse);
        }

        public WareHouse GetWareHouse(Guid id)
        {
            return _WareHouseRepository.GetById(id);
        }

        public IEnumerable<WareHouse> GetWareHouses()
        {
            return _WareHouseRepository.GetAll();
        }

        public void SaveWareHouse()
        {
            _unitOfWork.Commit();
        }

        public void RemoveWareHouse(WareHouse WareHouse)
        {
            _WareHouseRepository.Delete(WareHouse);
        }

        public IEnumerable<WareHouse> GetWareHouses(Expression<Func<WareHouse, bool>> where)
        {
            return _WareHouseRepository.GetMany(where);
        }
    }
}
