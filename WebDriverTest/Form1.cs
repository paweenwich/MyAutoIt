using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebDriverTest
{
    public partial class Form1 : Form
    {
        //public IWebDriver driver;
        IWebDriver driver;
        public Form1()
        {
            InitializeComponent();
            // driver = new ChromeDriver();
        }

        public void Login(String user,String password,String account)
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:8080/admin/login.html");
            //driver.Navigate().Refresh();
            driver.FindElement(By.XPath(".//*[@id='email']")).SendKeys(user);
            driver.FindElement(By.XPath(".//*[@id='pwd']")).SendKeys(password);
            driver.FindElement(By.XPath(".//*[@id='continueDiv']")).Click();
            //# Make sure all object activated
            Thread.Sleep(3000);
            //< div title = "" class="chosen-container chosen-container-single" id="accountSelect_chosen" style="width: 325px;"><a class="chosen-single chosen-default"><span>Select an Account...</span><div><b></b></div></a><div class="chosen-drop"><div class="chosen-search"><input type = "text" autocomplete="off"></div><ul class="chosen-results"></ul></div></div>
            var a = driver.FindElement(By.Id("accountSelect_chosen"));
            a.Click();
            var AllDropDownList = a.FindElements(By.XPath(".//li"));
            foreach (var opt in AllDropDownList)
            {
                if (opt.Text == account)
                {
                    opt.Click();
                    driver.FindElement(By.XPath(".//*[@id='continueDiv']")).Click();
                    return;
                }
            }
            throw new Exception("Login Fail");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(driver == null)
            {
                driver = new ChromeDriver();
            }
            Login("tt994613+180124@gmail.com", "password", "coatest031203s_com");
        }
            
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            driver.Quit();
        }
    }
}
