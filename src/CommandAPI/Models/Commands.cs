namespace CommandAPI.Models
{
    public class Command
    {
        public int Id { get; set; }     
        public string howTo { get; set; }    
        public string Platform {get; set;}
        public string CommandLine {get; set;}
    }
}