using System;
using System.Threading;

namespace MenuSystem
{
    public class MenuItem
    {
        public MenuItem(string shortCut, string buttonName, Func<double> runMethod)
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

        public string ShortCut { get; private set; }
        public string ButtonName { get; private set; }
        public  Func<double> RunMethod { get; private set; }

        public override string ToString()
        {
            return ShortCut + ")" + ButtonName;
        }
    }
}