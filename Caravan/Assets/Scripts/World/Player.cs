using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Cargos;

namespace Assets.Scripts.World
{
    public class Player
    {
        public List<Bramin> Bramins { get; set; } = new List<Bramin>();

        public int BraminWeightSumm => Bramins.Sum(c => c.Bag.Weight);

        public int Tokens { get; set; }
    }

    public class Bramin
    {
        public BraminBag Bag { get; set; } = new BraminBag();
    }

    public class BraminBag
    {
        public const int MaxBraminBagWeight = 400;

        public List<Cargo> Cargos { get; set; } = new List<Cargo>();
        public int Weight => (int) Math.Round(Cargos.Sum(c => c.Weight));
    }
}