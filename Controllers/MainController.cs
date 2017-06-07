using Lib.Web.Mvc;
using MusicFun.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MusicFun.Controllers
{
    [RoutePrefix("Main")]
    public class MainController : Controller
    {
        
        private const string MAIN_URL = " http://webapplication5020170423112144.azurewebsites.net/api/Account/";
      
        // GET: Main
        public async Task<ActionResult> Index()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL+"getUser");            
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            string objText;
            using (var twitpicResponse = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {                    
                    objText = reader.ReadToEnd();

                }

            }
            TempData["user"] = objText;
            return View();

        }
        public class MyResponse
        {
            
            public int status { get; set; }
            public string message { get; set; }
        }
        public FileResult Download()
        {
            var path = Path.Combine(Server.MapPath("~/App_Data"),"MusicFun.apk");
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "MusicFun.apk";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public async Task<ContentResult> Register(member m)
        {
            var json = new JavaScriptSerializer().Serialize(m);
          
           string url = MAIN_URL+"Register";
            HttpClient client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;           
            response = await client.PostAsync(new Uri(url), content);           
            string result = await response.Content.ReadAsStringAsync();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            MyResponse resp = (MyResponse)json_serializer.Deserialize(result, typeof(MyResponse)); 
              Response.Cookies.Add(new HttpCookie("access_token",resp.message));             
            return Content(result);
        }

        public async Task<ContentResult> Login(member2 m)
        {
            var json = new JavaScriptSerializer().Serialize(m);
            string url = MAIN_URL+"Login";
            HttpClient client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), content);         
            string result = await response.Content.ReadAsStringAsync();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            MyResponse resp = (MyResponse)json_serializer.Deserialize(result, typeof(MyResponse));
            Response.Cookies.Add(new HttpCookie("access_token", resp.message));
            return Content(result);
        }

        public PartialViewResult LoginView() {
            return PartialView("Login");
        }

        public ActionResult UploadList()
        {
            return PartialView("UploadList");
        }

       [HttpPost]
        public async Task<int> ListSongsCount(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL + url);
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)await request.GetResponseAsync())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

                }

            }
            return myojb.Count();
           }

        [HttpPost]
        public async Task<int> ListSongsCount2(int list_id)
        {

            string url = MAIN_URL+"GetListSong";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(list_id.ToString()), "list_id");
            
            var response = await client.PostAsync(url, mulContent);            
            string result = await response.Content.ReadAsStringAsync();            
           
            IEnumerable<MusicList> myojb;
            var stream = await response.Content.ReadAsStreamAsync();
                using ( var reader = new StreamReader(stream))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

                }
            return myojb.Count();

        }


        public ActionResult ListSongs(string url)
        {
            System.Diagnostics.Debug.WriteLine(url);
            System.Diagnostics.Debug.WriteLine("EWFEWQFEW");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL + url);           
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";           
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));
                    
                }
               

            }
            
            return PartialView("MusicList", myojb);
        }
       
        public async Task<ActionResult> OtherListSongs(int list_id)
        {
            string url = MAIN_URL+"GetListSong";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            IEnumerable<MusicList> myojb;
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(list_id.ToString()), "list_id");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);            
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = reader.ReadToEnd();
                myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

            }
            return PartialView("MusicList", myojb);
        }
        [HttpPost]
        public async Task<ContentResult> AddList(string list_name)
        {
            string url = MAIN_URL+"AddList";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
         
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");            
            mulContent.Add(new StringContent(list_name), "list_name");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);            
            return Content((response.StatusCode==HttpStatusCode.OK).ToString());
        }
        public ActionResult MusicList(string keyword)           
        {
           
                
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL+"ListMusic");          
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";           
            request.Accept = "application/json";
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));
                   
                }
                if (!string.IsNullOrEmpty(keyword)) {
                    myojb = myojb.Where(c => c.song_title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                }
              
            }
            return PartialView("MusicList",myojb);
        }
        public  class member
        {            
            public string email { get; set; }
            public string name { get; set; }
            public string password { get; set; }
        }
        public class member2
        {
          
            public string email { get; set; }            
            public string password { get; set; }
        }
        // GET: Main
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return PartialView("List");
        }
       
        public class HttpPostedFileAbstraction : TagLib.File.IFileAbstraction
        {
            private HttpPostedFileBase file;

            public HttpPostedFileAbstraction(HttpPostedFileBase file)
            {
                this.file = file;
            }

            public string Name
            {
                get { return file.FileName; }
            }

            public System.IO.Stream ReadStream
            {
                get { return file.InputStream; }
            }

            public System.IO.Stream WriteStream
            {
                get { throw new Exception("Cannot write to HttpPostedFile"); }
            }

            public void CloseStream(System.IO.Stream stream) { }
        }

        
        public async Task<ContentResult> Modify()
        {
            string song_id = Request.Form["id"];
            string author = Request.Form["author"];
            string song_name = Request.Form["song_name"];
            string url = MAIN_URL+"Modify/"+song_id;
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(author), "author");
            mulContent.Add(new StringContent(song_name), "song_name");
            HttpResponseMessage response;
            response = await client.PostAsync(url, mulContent);            
            string result = await response.Content.ReadAsStringAsync();

            return Content(response.StatusCode.ToString());
        }
        public async Task<ContentResult> Delete()
        {
            string song_id = Request.Form["id"];            
            string url = MAIN_URL+"Delete/" + song_id;
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
          
            HttpResponseMessage response;
            response = await client.GetAsync(url);
            // response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            return Content(response.StatusCode.ToString());
        }
        public async Task<ContentResult> Upload3()
        {

            string url = MAIN_URL+"upload3";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            var file = Request.Files["file"];
            
            var stream = file.InputStream;
            var buffer = new BufferedStream(stream,int.Parse(stream.Length.ToString()));
            var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Seek(0, SeekOrigin.Begin);
            HttpContent fileContent = new StreamContent(memory);            
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");            
            mulContent.Add(fileContent, "file", file.FileName) ;
            TagLib.File myFile = TagLib.File.Create(new HttpPostedFileAbstraction(file));
            int duration = (myFile.Properties.Duration.Minutes * 60 + myFile.Properties.Duration.Seconds) * 1000;
            duration += myFile.Properties.Duration.Milliseconds;
            
            var author = Encoder(myFile.Tag.Performers.FirstOrDefault());
            var duration_s = Encoder(duration.ToString()); 
            var title = Encoder(myFile.Tag.Title);
            if (title == "") {
               
                int i = myFile.Name.LastIndexOf(".");
                if (i > 0)
                {
                    title = myFile.Name.Remove(i); 
                }
                
            }
            mulContent.Add(new StringContent(duration_s), "song_duration");
            mulContent.Add(new StringContent(title), "song_name");
            mulContent.Add(new StringContent(author), "author");
            HttpResponseMessage response;
            response = await client.PostAsync(url, mulContent);           
            string result = await response.Content.ReadAsStringAsync();            
            return Content(response.StatusCode.ToString());
        }

        public async Task<ContentResult> addSongToList(int list_id, int song_id)
        {
            string url = MAIN_URL+"AddListSong";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);

            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(list_id.ToString()), "list_id");
            mulContent.Add(new StringContent(song_id.ToString()), "song_id");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);
            // response.EnsureSuccessStatusCode();
            return Content((response.StatusCode == HttpStatusCode.OK).ToString());
        }
        public  JsonResult getSongsList()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL+"MusicLists");
            
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";            
            IEnumerable<SongLists> myojb;
            using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<SongLists>)js.Deserialize(objText, typeof(IEnumerable<SongLists>));

                }
            }
            return Json(myojb, JsonRequestBehavior.AllowGet); 
        }
        public async Task<ActionResult> getSongList()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL+"MusicLists");           
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";
            Dictionary<string,int> count=new Dictionary<string, int>();
            List<ListInfo> listInfo = new List<ListInfo>();
            IEnumerable<SongLists> myojb;
            using (var twitpicResponse = (HttpWebResponse)await request.GetResponseAsync())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<SongLists>)js.Deserialize(objText, typeof(IEnumerable<SongLists>));

                }
                foreach (var a in myojb)
                {
                    listInfo.Add(new ListInfo() { songLists = a, list_count = await ListSongsCount2(a.Id) });
                }

            }
            count.Add("CurrentList",await ListSongsCount("CurrentList"));
            count.Add("AlwaysList", await ListSongsCount("AlwaysList"));
            count.Add("LatestList", await ListSongsCount("LatestList"));
            var tupleModel = new Tuple<List<ListInfo>, Dictionary<string, int>>(listInfo,count);
            return View("List",tupleModel);
        }
        public string Encoder(string s) {
            if (s != null)
            {
                return HttpUtility.UrlEncode(s, System.Text.Encoding.UTF8);

            }
            else {

                return "";
            }
            
        }
        public PartialViewResult Logout() {
            if (Request.Cookies["access_token"] != null)
            {
                HttpCookie myCookie = new HttpCookie("access_token");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            return PartialView("Login");
        }
        [Route("Stream/{id}")]
        public RangeFileContentResult MusicStream(int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MAIN_URL+"GetStream/"+id);           
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);           
            var response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            return new RangeFileContentResult(ReadFully(stream), "audio/mp3", "oo.mp3", DateTime.Now);
           
            }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[10 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
       
    }
}