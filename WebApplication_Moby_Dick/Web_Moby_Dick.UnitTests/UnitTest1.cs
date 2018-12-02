using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication_Moby_Dick.Controllers;
using System.Xml;

namespace Web_Moby_Dick.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            //arrange
            HomeController controller = new HomeController();

            //act
            ViewResult result = controller.Index() as ViewResult;

            //assert
            Assert.IsNotNull(result); // metodun null d�nmedi�ini g�ster
            Assert.IsInstanceOfType(result, typeof(ViewResult)); //metodun d�nd��� veri tipinin uygun oldu�unu g�ster
            
        }
    }
}
