using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using RestAPI.BLL;
using RestAPI.Common;

namespace RestAPI.Controllers
{
    [EnableCors("http://localhost:3000", "*", "*")]
    public class CsvController : ApiController
    {
        [Route("api/v1/Csv/{entity}/{id:int}/{timeStamp}")]
        [HttpGet]
        public IHttpActionResult Search(string entity, int id, long timeStamp)
        {
            IHttpActionResult result;
            try
            {
                //TODO : use DI
                IEnquiry enquiryBll = new Enquiry();
                var searchResult = enquiryBll.SearchEntity(new Entity(id, entity, timeStamp));


                result = Ok(searchResult != null ? searchResult.Changes : new JObject());
            }
            catch (Exception ex)
            {
                result = Content(HttpStatusCode.BadRequest, "Failed to query database. " + ex.Message);
            }

            return result;
        }

        [HttpPost]
        public IHttpActionResult UploadCsv()
        {
            IHttpActionResult result;
            try
            {
                //TODO : use DI
                IEnquiry enquiryBll = new Enquiry();


                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        using (var streamReader = new StreamReader(postedFile.InputStream))
                        {
                            enquiryBll.ReadFromStream(streamReader);
                        }
                    }

                    result = Ok("File uploaded successfully");
                }
                else
                {
                    result = Content(HttpStatusCode.BadRequest, "Failed to uploaded File.");
                }
            }
            catch (Exception ex)
            {
                result = Content(HttpStatusCode.BadRequest, "Failed to uploaded File. " + ex.Message);
            }

            return result;
        }
    }
}