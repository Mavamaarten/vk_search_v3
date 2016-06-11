using System.Threading.Tasks;

namespace vk_search_v3.API
{
    interface IAlbumArtProvider
    {
        Task<string> GetAlbumArtUrl(string query);
    }
}