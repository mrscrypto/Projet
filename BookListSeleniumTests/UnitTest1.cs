using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BookListSeleniumTests
{
    public class UnitTest1 : IDisposable
    {

        private readonly IWebDriver _driver;

        public UnitTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");


            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void Accueil()
        {
            _driver.Navigate().GoToUrl("http://localhost:5000");

            var titre = _driver.Title;
            Assert.Contains("Home", titre);
        }


        [Fact]
        public void AjouterLivre()
        {
            _driver.Navigate().GoToUrl("http://localhost:5000/Books/Upsert");

            System.Threading.Thread.Sleep(5000);


            _driver.FindElement(By.Id("Name")).SendKeys("To Kill a Mockingbird");
            _driver.FindElement(By.Id("Author")).SendKeys("Harper Leaate");
            _driver.FindElement(By.Id("ISBN")).SendKeys("9780061120084");

            _driver.FindElement(By.CssSelector("form")).Submit();
            System.Threading.Thread.Sleep(5000);


            var body = _driver.PageSource;

            Assert.Contains("To Kill a Mockingbird", body);
        }


        [Fact]
        public void ModifierLivre()
        {
            // Aller à la page des livres
            _driver.Navigate().GoToUrl("http://localhost:5000/Books");

            System.Threading.Thread.Sleep(5000);

            // Trouver le livre par son nom
            var lignesLivres = _driver.FindElements(By.CssSelector("table tbody tr"));
            foreach (var ligne in lignesLivres)
            {
                if (ligne.Text.Contains("To Kill a Mockingbird"))
                {
                    // Cliquer sur le bouton Edit
                    var boutonEdit = ligne.FindElement(By.CssSelector(".btn-success"));
                    boutonEdit.Click();
                    break;
                }
            }

            // Attendre que la page du formulaire se charge
            System.Threading.Thread.Sleep(5000);

            // Modifier l'auteur
            var champAuteur = _driver.FindElement(By.Id("Author"));
            champAuteur.Clear(); 
            champAuteur.SendKeys("Harper Lee");

            // Soumettre
            _driver.FindElement(By.CssSelector("form")).Submit();
            System.Threading.Thread.Sleep(5000);

            // Vérifier que le nouvel auteur est présent
            var body = _driver.PageSource;
            Assert.Contains("Harper Lee", body);
        }

        [Fact]
        public void SupprimerLivre()
        {
            // Aller à la page des livres
            _driver.Navigate().GoToUrl("http://localhost:5000/Books");
            System.Threading.Thread.Sleep(5000);


            // Trouver la ligne contenant le livre et cliquer sur Delete
            var lignesLivres = _driver.FindElements(By.CssSelector("table tbody tr"));
            foreach (var ligne in lignesLivres)
            {
                if (ligne.Text.Contains("L'Étranger"))
                {
                    var boutonDelete = ligne.FindElement(By.CssSelector(".btn-danger"));
                    boutonDelete.Click();
                    break;
                }
            }

            System.Threading.Thread.Sleep(5000);

            var boutonOK = _driver.FindElement(By.CssSelector(".swal-button--confirm.swal-button--danger"));
            boutonOK.Click();

            System.Threading.Thread.Sleep(5000);

           
        }



        public void Dispose()
        {
            // Fermer le navigateur après chaque test
            _driver.Quit();
        }

       


    }
}
