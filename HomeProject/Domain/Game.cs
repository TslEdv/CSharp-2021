using System;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }
        
        public string GameState { get; set; } = default!;

        public string CreationTime { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
    }
}