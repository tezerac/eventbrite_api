using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMvc.Infrastructure;
using WebMvc.Models;

namespace WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        public CatalogService(IOptionsSnapshot<AppSettings> settings,
            IHttpClient httpClient)
        {
            _settings = settings;
            _apiClient = httpClient;
            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/catalog/";

        }

        public async Task<IEnumerable<SelectListItem>> GetCatagories()
        {

            var getcatagoriesUri = ApiPaths.Catalog.GetCatagories(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getcatagoriesUri);

            var events = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var brands = JArray.Parse(dataString);
            foreach (var brand in brands.Children<JObject>())
            {
                events.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }
            return events;
        }

        public async Task<IEnumerable<SelectListItem>> GetSubCatagories()
        {
            var getsubcatagoriesUri = ApiPaths.Catalog.GetSubCatagories(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getsubcatagoriesUri);

            var events = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var types = JArray.Parse(dataString);

            foreach (var type in types.Children<JObject>())
            {
                events.Add(new SelectListItem()
                {
                    Value = type.Value<string>("id"),
                    Text = type.Value<string>("type")
                });
            }
            return events;
        }

     
    
    public async Task<Catalog> GetEvents(int page, int take, int? brand, int? type)
        {
            var alleventsUri = ApiPaths.Catalog.GetAllEvents(_remoteServiceBaseUrl, page, take, brand, type);
            // new request
            var dataString = await _apiClient.GetStringAsync(alleventsUri);

            // Deserialize - converting back into catalog
            //seederulization - packaging 

            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            // controller
            return response;
        }   
    }
}
