using System;
using System.Collections.Generic;
using System.Linq;
using Assets.LogicScripts.Buildings.Factories;
using Assets.LogicScripts.ServerLogic.ClientSide;
using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.Processors
{
    public static class ServerStaticData
    {
        static ServerStaticData()
        {
            var livingHouseProduce = new ProduceClientSideEntity();
            livingHouseProduce.From.LoadCollection(new IProduceIngredient[]
            {
                new ProduceIngredientClientSideEntity
                {
                    Cargo = new FreshWaterClientSideEntity {Count = 1}
                }
            });

            Produce.Add(nameof(LivingHouse),);
            {
                From = new List<ProduceIngredient>
                {
                    new ProduceIngredient {Cargo = new IFreshWater(), Value = 1}
                },
                Speed = 0.01m
            });
        }

        public static Dictionary<string, IProduce> Produce { get; set; } = new Dictionary<string, IProduce>();
    }

    public abstract class ProcessBuildingProcessor : BuildingProcessor
    {
    }

    public abstract class BuildingProcessor : ProcessorBase<IBuilding>
    {
        public virtual Dictionary<string, decimal> CanStoreCargos { get; set; } = new Dictionary<string, decimal>();

        public override void Process(IBuilding entity)
        {
            throw new NotImplementedException();
        }

        public virtual decimal CanAddCargoMore(IBuilding entity, string type)
        {
            if (!CanStoreCargos.TryGetValue(type, out var canStoreAtAll)) return 0;

            var count = GetFullCargoCount(entity, type);

            var result = canStoreAtAll - count;

            return result > 0 ? result : 0;
        }

        public virtual bool AddCargo(IBuilding entity, ICargo cargo)
        {
            if (CanAddCargoMore(entity, cargo.Type) < cargo.Count) return false;


            var existedCargo = entity.Cargos.Collection.FirstOrDefault(c => c.Type == cargo.Type);

            if (existedCargo != null)
                existedCargo.Count += cargo.Count;
            else
                entity.Cargos.AddToCollection(cargo);

            return true;
        }

        public decimal GetFullCargoCount(IBuilding entity, string type)
        {
            return entity.Cargos.Collection.Where(c => c.Type == type).Sum(c => c.Count);
        }

        public ICargo GetCargoOfType(IBuilding entity, string type)
        {
            return entity.Cargos.Collection.FirstOrDefault(c => c.Type == type);
        }

        public ICargo GetCargoOfType<TType>(IBuilding entity)
        {
            return entity.Cargos.Collection.FirstOrDefault(c => c.Type == typeof(TType).Name);
        }
    }
}