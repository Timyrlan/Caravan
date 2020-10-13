using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;

namespace Assets.Scripts.World
{
    public class Player : IGameProcessibleObject
    {
        public List<Bramin> Bramins { get; set; } = new List<Bramin>();

        public int BraminWeightSumm => Bramins.Sum(c => c.Weight);

        public int Tokens { get; set; }

        public void Process()
        {
            foreach (var bramin in Bramins) bramin.Process();
        }
    }
}