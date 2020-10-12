using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;

namespace Assets.Scripts.World
{
    public class Player : IGameProcessibleObject
    {
        public List<Bramin> Bramins { get; set; } = new List<Bramin>();

        public int BraminWeightSumm => Bramins.Sum(c => c.Bag.Weight);

        public int Tokens { get; set; }

        public void Process()
        {
            foreach (var bramin in Bramins) bramin.Process();
        }
    }

    public class Bramin : IGameProcessibleObject
    {
        public BraminBag Bag { get; set; } = new BraminBag();

        public long Age { get; set; }

        public void Process()
        {
            Age++;
        }
    }

    public class BraminBag
    {
        public const int MaxBraminBagWeight = 400;

        public List<Cargo> Cargos { get; set; } = new List<Cargo>();
        public int Weight => (int) Math.Round(Cargos.Sum(c => c.Weight));
    }
}