using System;
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
        [Route("{shortUrl}")]
        public async Task<IHttpActionResult> Get(string shortUrl)
        {
            Database.Database database = new Database.Database();
            string LongUrl = database.GetLongUrl(shortUrl);
           
            if (LongUrl == "")
            {
                return Redirect("Https://localhost/views/home/error.html");
            }
            System.Uri uri = new System.Uri(LongUrl);
           return Redirect(uri);
        }

        // POST api/values
        public string Post(URL uRL)
        {
            string LongUrl= uRL.LongUrl, CustomValue=uRL.CustomValue;
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
                string guid = Guid.NewGuid().ToString().Substring(0,8);
                while(database.GetLongUrl(guid) != "")
                {
                    guid = Guid.NewGuid().ToString().Substring(0, 8);
                }
                return database.PutLongUrl(Shorturl: guid, Longurl: LongUrl);
            }
        }
    }
}
