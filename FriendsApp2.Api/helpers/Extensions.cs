using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FriendsApp2.Api.helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static void AddPagination(this HttpResponse response,
                int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var camelCaseFormatting = new JsonSerializerSettings();
            camelCaseFormatting.ContractResolver = new CamelCasePropertyNamesContractResolver();

            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatting));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
        public static int CalculateAge(this DateTime theDatetime)
        {
            var age = DateTime.Today.Year - theDatetime.Year;
            if (theDatetime.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }
    }
}