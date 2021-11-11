using System;

namespace Domain
{
    public class Config
    {
        public int ConfigId { get; set; }

        public string ConfigStr { get; set; } = default!;
        
        public string CreationTime { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
    }
}