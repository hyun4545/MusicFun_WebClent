using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicFun.Models
{
    public class MusicList
    {
        public int Id { get; set; }
        public string user_id { get; set; }
        public string song_title { get; set; }
        public string author { get; set; }
        public string file_name { get; set; }
        public System.DateTime add_time { get; set; }
        public string song_time { get; set; }
        public System.DateTime last_time { get; set; }
        public int download_times { get; set; }
    }
}