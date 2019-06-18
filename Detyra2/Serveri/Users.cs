using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Serveri
{
    public class Users
    {  
        private static string generateSalt()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(100000, 1000000).ToString();
        }

        public static string generateSaltedHash(string password,string salt)
        {
            string passwordSalt = password + salt;
            SHA1CryptoServiceProvider objHash = new SHA1CryptoServiceProvider();

            byte[] bytePasswordSalt = Encoding.UTF8.GetBytes(passwordSalt);
            byte[] byteSaltedPasswordHash = objHash.ComputeHash(bytePasswordSalt);

            return Convert.ToBase64String(byteSaltedPasswordHash);
        }

        public static void insert(string name,string surname,string username,string password,string pozita,string paga,string departamenti)
        {
            if (File.Exists("punetoret.xml") == false)
            {
                XmlTextWriter xmlTw = new XmlTextWriter("punetoret.xml", Encoding.UTF8);
                xmlTw.WriteStartElement("punetoret");
                xmlTw.Close();
            }
            XmlDocument objXml = new XmlDocument();
            objXml.Load("punetoret.xml");

            XmlElement rootNode = objXml.DocumentElement;
            XmlElement userNode= objXml.CreateElement("punetori");
            userNode.SetAttribute("id", (getNumberOfWorkers()+1).ToString());
            XmlElement nameNode = objXml.CreateElement("name");
            XmlElement surnameNode = objXml.CreateElement("surname");
            XmlElement usernameNode = objXml.CreateElement("username");
            XmlElement passwordNode = objXml.CreateElement("password");
            XmlElement saltNode = objXml.CreateElement("salt");
            XmlElement saltedHashNode = objXml.CreateElement("saltedHash");
            XmlElement pozitaNode = objXml.CreateElement("pozita");
            XmlElement pagaNode = objXml.CreateElement("paga");
            XmlElement departamentiNode = objXml.CreateElement("departamenti");

            string salt = generateSalt();

            nameNode.InnerText = name;
            surnameNode.InnerText = surname;
            usernameNode.InnerText = username;
            saltNode.InnerText = salt;
            saltedHashNode.InnerText = generateSaltedHash(password, salt);
            pozitaNode.InnerText = pozita;
            pagaNode.InnerText = paga;
            departamentiNode.InnerText = departamenti;

            passwordNode.AppendChild(saltNode);
            passwordNode.AppendChild(saltedHashNode);
            userNode.AppendChild(nameNode);
            userNode.AppendChild(surnameNode);
            userNode.AppendChild(usernameNode);
            userNode.AppendChild(passwordNode);
            userNode.AppendChild(pozitaNode);
            userNode.AppendChild(pagaNode);
            userNode.AppendChild(departamentiNode);


            rootNode.AppendChild(userNode);

            objXml.Save("punetoret.xml");

        }

        public static double getNumberOfWorkers()
        {
            var filename = "punetoret.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var punetoretFilepath = Path.Combine(currentDirectory, filename);

            try
            {
                XElement punetoret = XElement.Load($"{punetoretFilepath}");

                IEnumerable<int> numriPunetoreve = from punetori in punetoret.Descendants("punetori")
                                                   select (int)punetori.Attribute("id");
                return numriPunetoreve.Count();
            }
            catch
            {
                return 0;
            }
        }

        public static string getSaltByUsername(string username)
        {
            var filename = "punetoret.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var punetoretFilepath = Path.Combine(currentDirectory, filename);

            try
            {
                XElement punetoret = XElement.Load($"{punetoretFilepath}");

                IEnumerable<string> salt = from punetori in punetoret.Descendants("punetori")
                                           where punetori.Element("username").Value.ToString().Equals(username)
                                           select punetori.Element("password").Element("salt").Value;

                return salt.Single();
            }
            catch
            {
                return null;
            }
        }

        public static bool isUser(string username,string password)
        {
            var filename = "punetoret.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var punetoretFilepath = Path.Combine(currentDirectory, filename);

            try
            {
                XElement punetoret = XElement.Load($"{punetoretFilepath}");

                IEnumerable<XElement> passwordInfo = from punetori in punetoret.Descendants("punetori")
                                                     where punetori.Element("username").Value.ToString().Equals(username)
                                                     select punetori.Element("password");

               
                string saltPassword = password + passwordInfo.First().Element("salt").Value;
                byte[] byteSaltPassword = Encoding.UTF8.GetBytes(saltPassword);

                SHA1CryptoServiceProvider objHash = new SHA1CryptoServiceProvider();

                byte[] byteSaltedHashPassword = objHash.ComputeHash(byteSaltPassword);

                string base64SaltedHashPassword = Convert.ToBase64String(byteSaltedHashPassword);

                if (base64SaltedHashPassword.Equals(passwordInfo.First().Element("saltedHash").Value))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
            
        }

        public static XElement getUserInfo(string username)
        {
            var filename = "punetoret.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var punetoretFilepath = Path.Combine(currentDirectory, filename);

            try
            {
                XElement punetoret = XElement.Load($"{punetoretFilepath}");
                IEnumerable<XElement> infoPuntoret = from puntori in punetoret.Descendants("punetori")
                                                     where puntori.Element("username").Value.ToString().Equals(username)
                                                     select puntori;

                return infoPuntoret.First();
            }
            catch
            {
                return null;
            }
        }
    }
}
