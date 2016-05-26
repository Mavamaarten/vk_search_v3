using System.Collections.Generic;

namespace vk_search_v3.Model
{
    class Playlist
    {
        public long Id { get; set; }
        public List<Track> Tracks { get; set; }
        public string Name { get; set; }

        public Playlist(string name)
        {
            Name = name;
            Tracks = new List<Track>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
