using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extentions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeaders(this HttpResponse http, int currentPage,  int itemsPerPage,  int totalItems, int totalPages)
        {
            var paginationheader = new PaginationHeader(currentPage, itemsPerPage, totalItems,  totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            http.Headers.Add("Pagination", JsonSerializer.Serialize(paginationheader, options));
            http.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}