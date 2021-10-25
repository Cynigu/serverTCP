using Microsoft.VisualStudio.TestTools.UnitTesting;
using serverTCP;

namespace TestProjectForSerber
{
    [TestClass]
    public class UnitTestLogicGAme
    {
        
        [TestMethod]
        public void TestMethodWinGame() // Проверка победной таблицы
        {
            bool fact = true;

            ITable table = LogicGame.CreateWinTable();

            bool res = LogicGame.AreYouWinner(table);

            Assert.AreEqual(fact, res);
        }
    }
}
