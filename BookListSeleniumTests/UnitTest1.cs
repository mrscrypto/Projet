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
            // Configuration Chrome en mode headless
            var options = new ChromeOptions();
            options.AddArgument("--headless"); 
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void Accueil_AfficheTitreCorrect()
        {
            _driver.Navigate().GoToUrl("https://localhost:44315");

            var titre = _driver.Title;
            Assert.Contains("Home", titre);
        }


        [Fact]
        public void AjouterLivre_FormulaireFonctionne()
        {
            _driver.Navigate().GoToUrl("https://localhost:44315/Books/Upsert");

            _driver.FindElement(By.Id("Name")).SendKeys("Test1");
            _driver.FindElement(By.Id("Author")).SendKeys("Auteur test2");
            _driver.FindElement(By.Id("ISBN")).SendKeys("1234567890�");

            _driver.FindElement(By.CssSelector("form")).Submit();
            System.Threading.Thread.Sleep(2000);


            var body = _driver.PageSource;

            Assert.Contains("Test1", body);
        }


        [Fact]
        public void ModifierLivre_Fonctionne()
        {
            // Aller � la page des livres
            _driver.Navigate().GoToUrl("https://localhost:44315/Books");

            System.Threading.Thread.Sleep(5000);

            // Trouver le livre par son nom
            var lignesLivres = _driver.FindElements(By.CssSelector("table tbody tr"));
            foreach (var ligne in lignesLivres)
            {
                if (ligne.Text.Contains("Test1"))
                {
                    // Cliquer sur le bouton Edit
                    var boutonEdit = ligne.FindElement(By.CssSelector(".btn-success")); // bouton vert "Edit"
                    boutonEdit.Click();
                    break;
                }
            }

            // Attendre que la page du formulaire se charge
            System.Threading.Thread.Sleep(5000);

            // Modifier l'auteur
            var champAuteur = _driver.FindElement(By.Id("Author"));
            champAuteur.Clear(); // efface l'ancien texte
            champAuteur.SendKeys("Auteur modifi� avec succes!!!");

            // Soumettre
            _driver.FindElement(By.CssSelector("form")).Submit();
            System.Threading.Thread.Sleep(2000);

            // V�rifier que le nouvel auteur est pr�sent
            var body = _driver.PageSource;
            Assert.Contains("Auteur modifi� avec succes!!!", body);
        }

        [Fact]
        public void SupprimerLivre_Fonctionne()
        {
            // Aller � la page des livres
            _driver.Navigate().GoToUrl("https://localhost:44315/Books");
            System.Threading.Thread.Sleep(5000);


            // Trouver la ligne contenant le livre et cliquer sur Delete
            var lignesLivres = _driver.FindElements(By.CssSelector("table tbody tr"));
            foreach (var ligne in lignesLivres)
            {
                if (ligne.Text.Contains("Livre � modifier"))
                {
                    var boutonDelete = ligne.FindElement(By.CssSelector(".btn-danger"));
                    boutonDelete.Click();
                    break;
                }
            }

            // Attendre que la bo�te de dialogue SweetAlert apparaisse
            System.Threading.Thread.Sleep(5000);

            // Cliquer sur le bouton OK dans le pop-up SweetAlert
            var boutonOK = _driver.FindElement(By.CssSelector(".swal-button--confirm.swal-button--danger"));
            boutonOK.Click();

            // Attendre le rafra�chissement
            System.Threading.Thread.Sleep(5000);

            //V�rifier que le livre n�est plus dans la page
            var body = _driver.PageSource;
            Assert.DoesNotContain("Livre � modifier", body);
        }



        public void Dispose()
        {
            // Fermer le navigateur apr�s chaque test
            _driver.Quit();
        }

       


    }
}