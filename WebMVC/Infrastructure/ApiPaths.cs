using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Infrastructure
{
    public class ApiPaths
    {
        public static class Basket
        {
            public static string GetBasket(string baseUri, string basketId)
            {
                return $"{baseUri}/{basketId}";
            }

            public static string UpdateBasket(string baseUri)
            {
                return baseUri;
            }

            public static string CleanBasket(string baseUri, string basketId)
            {
                return $"{baseUri}/{basketId}";
            }
        }
        public static class Catalog
        {
            //Our goal MVC see all my cataloge
            public static string GetAllEvents(string baseUri,
                //page - page size 
                //if u fill brand it ok or not
                //take how many its take
                int page, int take, int? brand, int? type)

                // location??
            {
                var filterQs = string.Empty;

                // one of these two things || or
                if (brand.HasValue || type.HasValue)
                {
                    var catagorynameQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                    var SubCatagoryQs = (type.HasValue) ? type.Value.ToString() : "null";
                    
                   
                    filterQs = $"/brand/{catagorynameQs}/type/{SubCatagoryQs}";
                }
                // pass the Uri to Ihttpclient

                return $"{baseUri}events{filterQs}?pageIndex={page}&pageSize={take}";
            }

            public static string GetEvent(string baseUri, int id)
            {
           // Localhost..... baseUrl
                return $"{baseUri}/events/{id}";
            }
            public static string GetCatagories(string baseUri)
            {
                return $"{baseUri}catagories";
            }

            public static string GetSubCatagories(string baseUri)
            {
                return $"{baseUri}subcatagories";
            }
        }
        public static class Order
        {
            public static string GetOrder(string baseUri, string orderId)
            {
                return $"{baseUri}/{orderId}";
            }

            //public static string GetOrdersByUser(string baseUri, string userName)
            //{
            //    return $"{baseUri}/userOrders?userName={userName}";
            //}
            public static string GetOrders(string baseUri)
            {
                return baseUri;
            }
            public static string AddNewOrder(string baseUri)
            {
                return $"{baseUri}/new";
            }
        }

    }
}
