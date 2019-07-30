using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
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
                longUrl = database.GetLongUrl(shortUrl);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            lruCache.Add(shortUrl, longUrl);

            return Redirect(new System.Uri(longUrl));
        }

        // POST api/values
        public string Post(URL uRL)
        {
            string LongUrl = uRL.LongUrl, CustomValue = uRL.CustomValue;
            Database.Database database = new Database.Database();
            if (!String.IsNullOrEmpty(CustomValue))
            {
                if (database.GetLongUrl(CustomValue) != "")
                {
                    return "Custom URL Already taken !";
                }
                else
                {
                    return database.PutLongUrl(Shorturl: CustomValue, Longurl: LongUrl);
                }
            }
            else
            {
                string guid = Guid.NewGuid().ToString().Substring(0, 8);
                while (database.GetLongUrl(guid) != "")
                {
                    guid = Guid.NewGuid().ToString().Substring(0, 8);
                }
                return database.PutLongUrl(Shorturl: guid, Longurl: LongUrl);
            }
        }
        [HttpGet]
        public IEnumerable<string> Show()
        {
            return new string[] { "Pawan", "Kumar", "vishwakarma" };
        }
    }
}
