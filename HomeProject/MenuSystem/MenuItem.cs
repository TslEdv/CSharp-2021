using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public MenuItem(string shortCut, string buttonName, Func<string, string> runMethod)
        {
            if (string.IsNullOrEmpty(shortCut))
            {
                throw new ApplicationException("ShortCut cannot be empty!");
            }

            if (string.IsNullOrEmpty(buttonName))
            {
                throw new ApplicationException("ButtonName cannot be empty!");
            }

            ShortCut = shortCut.Trim();
            ButtonName = buttonName.Trim();
            RunMethod = runMethod;
        }

        public string ShortCut { get; set; }
        private string ButtonName { get; set; }
        
        public readonly Func<string, string> RunMethod;

        public override string ToString()
        {
            return ShortCut + ")" + ButtonName;
        }
    }
}