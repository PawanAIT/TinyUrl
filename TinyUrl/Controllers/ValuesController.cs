using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Database;
using TinyUrl.Models;

namespace TinyUrl.Controllers
{

    public class ValuesController : ApiController
    {
        static LRUCache<string, string> lruCache { get; } = new LRUCache<string, string>(1000);

        [Route("{shortUrl}")]
        public async Task<IHttpActionResult> Get(string shortUrl)
        {
            string CachedLongurl = lruCache.Get(shortUrl);

            if (!string.IsNullOrEmpty(CachedLongurl))
            {
                return Redirect(new System.Uri(CachedLongurl));
            }

            // if not found in cache
            var database = new Database.Database();

            string longUrl;
            try
            {
                longUrl = await database.GetLongUrlAsync(shortUrl);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            lruCache.Add(shortUrl, longUrl);

            return Redirect(new System.Uri(longUrl));
        }

        // POST api/values
        public async Task<string> Post(URL uRL)
        {
            string LongUrl = uRL.LongUrl, CustomValue = uRL.CustomValue;
            Database.Database database = new Database.Database();
            if (!String.IsNullOrEmpty(CustomValue))
            {
                try
                {
                   await database.GetLongUrlAsync(CustomValue).ConfigureAwait(false);
                   return "Custom url already taken.";
                }
                catch (InvalidOperationException) 
                {
                    return await database.PutLongUrl(Shorturl: CustomValue, Longurl: LongUrl);
                }
            }
            else
            {
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString().Substring(0, 8);
                } while (await database.DoesShortUrlExistAsync(guid).ConfigureAwait(false));

                return await database.PutLongUrl(Shorturl: guid, Longurl: LongUrl);
            }
        }
       
    }
}
