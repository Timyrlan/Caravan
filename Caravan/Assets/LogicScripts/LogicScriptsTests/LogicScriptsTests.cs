using Assets.LogicScripts.Buildings.Factories;
using Assets.LogicScripts.DifferentCargos;
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

            target.AddCargo(new FreshWater {Count = 1});

            Assert.AreEqual(0, target.GetFullCargoCount(nameof(FreshWater)));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void FactoryAllowAddWrightCargo()
        {
            var target = new SaltEvaporationFactory();

            target.AddCargo(new SaltWater {Count = 1});

            Assert.AreEqual(1, target.GetFullCargoCount(nameof(SaltWater)));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SaltEvaporationFactoryTest_Process()
        {
            var target = new SaltEvaporationFactory();

            target.AddCargo(new SaltWater {Count = 1});

            target.Process();

            Assert.AreEqual(0.95m, target.GetFullCargoCount(nameof(FreshWater)));
            Assert.AreEqual(0.05m, target.GetFullCargoCount(nameof(Salt)));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SaltWaterWellTest_Process()
        {
            var target = new SaltWaterWell();

            target.Process();

            Assert.AreEqual(0.01m, target.GetFullCargoCount(nameof(SaltWater)));
        }

        [Test]
        public void LivingHouseTest_Process()
        {
            var target = new LivingHouse();
            target.AddCargo(new FreshWater {Count = 1});

            target.Process();

            Assert.AreEqual(0.99m, target.GetFullCargoCount(nameof(FreshWater)));
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