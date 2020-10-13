using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;

namespace Assets.Scripts.World
{
    public class Bramin : IGameProcessibleObject
    {
        public const int MaxBraminBagWeight = 400;

        public List<Cargo> Cargos { get; set; } = new List<Cargo>();
        public int Weight => (int)Math.Round(Cargos.Sum(c => c.Weight));

        public long Age { get; set; }

        public void Process()
        {
            Age++;
        }
    }
}