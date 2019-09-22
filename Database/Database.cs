using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Database
    {
        string _connString = null;
        string connString
        {
            get
            {
                if(_connString != null)
                {
                    return _connString;
                }
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                return _connString = keyVaultClient.GetSecretAsync("https://keyvaultsecrets.vault.azure.net/secrets/TinyUrlDatabseConnString/1206aa647dab4cf1a5e1f74761f48bfd").GetAwaiter().GetResult().Value;
            }
        }

        public async Task<string> GetLongUrlAsync(string Shorturl)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"select Longurl from UrlMapping where shorturl = @Shorturl;";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Shorturl", Shorturl);
                    using (SqlDataReader oReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
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
        public async Task<string> PutLongUrl(string Shorturl ,string Longurl)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"INSERT INTO [dbo].[UrlMapping] ([Shorturl],[Longurl]) VALUES (@Shorturl, @Longurl);";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Shorturl", Shorturl);
                    cmd.Parameters.AddWithValue("@Longurl", Longurl);
                    await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                }
                conn.Close();
            }
            return Shorturl;
        }

        public async Task<bool> DoesShortUrlExistAsync(string guid)
        {
            try
            {
                await GetLongUrlAsync(guid).ConfigureAwait(false);
                return true;
            }
            catch(InvalidOperationException)
            {
                return false;
            }
        }

        public async Task UpdateShortUrl(string shortUrl ,string longUrl)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"UPDATE [dbo].[UrlMapping] SET [Longurl] = @Longurl WHERE  [Shorturl] = @Shorturl;";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Shorturl", shortUrl);
                    cmd.Parameters.AddWithValue("@Longurl", longUrl);
                    await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                }
                conn.Close();
            }
        }
    }
}
