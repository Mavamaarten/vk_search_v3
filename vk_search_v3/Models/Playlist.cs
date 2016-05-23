using System.Collections.Generic;

namespace vk_search_v3.Models
{
    class Playlist
    {
        public long Id { get; set; }
        public List<Track> Tracks { get; set; }
        public string Name { get; set; }
    }
}
