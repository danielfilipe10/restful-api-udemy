using MagicVilla_Web.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace MagicVilla_Web.Services.Interfaces
{
    public interface IBaseService
    {
        APIResponse ResponseModel { get; set; }

        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
