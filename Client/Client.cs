using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace Client
{
    class Client
    {
        public static WebSocket client;
        public static DESCryptoServiceProvider des;
        public static void InitDes()
        {
            des = new DESCryptoServiceProvider();
            des.GenerateKey();
            des.GenerateIV();
        }
        public static void OnMes(object sender, MessageEventArgs e)
        {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(e.Data);
            Desparam config = new Desparam();
            config.Key = des.Key;
            config.Four = des.IV;
            var serializeConf = JsonConvert.SerializeObject(config);
            var byteConf = Encoding.UTF8.GetBytes(serializeConf);
            var byteEncrypt = RSA.Encrypt(byteConf, true);
            client.Send("3");
            client.Send(byteEncrypt);
        }
        public static void SendMes(string s)
        {
            var encryptor = des.CreateEncryptor();
            if(s!="1")
            {
                client.Send("0");
                client.Send(s);
            }
            else
            {
                string cs = "DataSource=Companies.db;";
                var con = new SQLiteConnection(cs);
                con.Open();
                string stm = "SELECT * FROM Companies";
                var cmd = new SQLiteCommand(stm,con);
                var rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    object[] arr = new object[13];
                    rdr.GetValues(arr);
                    Onerow row = new Onerow(arr);
                    var jsobj = JsonConvert.SerializeObject(row);
                    var bytes = Encoding.UTF8.GetBytes(jsobj);
                    var cryptmes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                    client.Send("1");
                    client.Send(cryptmes);
                }
                con.Close();
            }
        }
        static void Main(string[] args)
        {
            InitDes();
            client = new WebSocket("ws://25.74.30.162:8080");
            client.OnMessage += OnMes;
            client.Connect();
            string s = Console.ReadLine();
            while(s!="0")
            {
                SendMes(s);
                s = Console.ReadLine();
            }
        }
    }
}
