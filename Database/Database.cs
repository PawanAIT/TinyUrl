using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Database
    {
        string connString
        {
            get
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                return keyVaultClient.GetSecretAsync("https://keyvaultsecrets.vault.azure.net/secrets/TinyUrlDatabseConnString/1206aa647dab4cf1a5e1f74761f48bfd").GetAwaiter().GetResult().Value;
            }
        }

        public string GetLongUrl(string Shorturl)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"select Longurl from UrlMapping where shorturl = @Shorturl;";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Shorturl", Shorturl);
                    using (SqlDataReader oReader = cmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            return oReader["Longurl"].ToString();
                        }
                    }
                }
                conn.Close();
            }

            throw new InvalidOperationException();
        }
        public string PutLongUrl(string Shorturl ,string Longurl)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"INSERT INTO [dbo].[UrlMapping] ([Shorturl],[Longurl]) VALUES (@Shorturl, @Longurl);";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Shorturl", Shorturl);
                    cmd.Parameters.AddWithValue("@Longurl", Longurl);
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
            return Shorturl;
        }
    }
}
