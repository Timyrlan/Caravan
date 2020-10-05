using Assets.LogicScripts.Buildings;
using Assets.LogicScripts.Buildings.Factories;
using Assets.LogicScripts.Cargos;
using NUnit.Framework;

namespace Assets.LogicScripts.LogicScriptsTests
{
    public class LogicScriptsTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FactoryNotAllowAddWrongCargo()
        {
            var target = new SaltEvaporationFactory();

            target.AddCargo(new FreshWater());

            Assert.AreEqual(0, target.Cargos.Count);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void FactoryAllowAddWrightCargo()
        {
            var target = new SaltEvaporationFactory();

            target.AddCargo(new SaltWater());

            Assert.AreEqual(1, target.Cargos.Count);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SaltEvaporationFactoryTest_Process()
        {
            var target = new SaltEvaporationFactory();

            target.AddCargo(new SaltWater {Count = 1});

            target.Process();

            Assert.AreEqual(0.95m, target.GetCargoOfType<FreshWater>().Count);
            Assert.AreEqual(0.05m, target.GetCargoOfType<Salt>().Count);
        }

        //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        //// `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator FactoryNotAllowAddWrongCargo()
        //{
        //    var target = new SaltEvaporationFactory();

        //    target.AddCargo(new FreshWater());

        //    Assert.AreEqual(0, target.Cargos.Count);

        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}