using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using MenuSystem;

namespace Calculator
{
    class Program
    {
        private static double _currentValue = 0.0;

        static void Main(string[] args)
        {
            Console.Clear();
            
            var mainMenu = new Menu("Main Menu", MenuDepth.Main, _currentValue);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Binary Operations", BinaryMenu),
                new MenuItem("2", "Unary Operations", UnaryMenu),
            });
            mainMenu.RunMenu();
        }
        

        private static double BinaryMenu()
        {
            var binMenu = new Menu("Binary Operations", MenuDepth.SubMenu, _currentValue);
            binMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("+", "Addition", Addition),
                new MenuItem("-", "Subtraction", Substraction),
                new MenuItem("*", "Multiplication", Multiplication),
                new MenuItem("/", "Division", Division),
                new MenuItem("^", "X to the power of Y", XpowerY),
                new MenuItem("C", "Clear Current Value", Clear),
            });
            var value = binMenu.RunMenu();
            return (double) value;
        }

        private static double UnaryMenu()
        {
            var unaMenu = new Menu("Unary Operations", MenuDepth.SubMenu, _currentValue);
            unaMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("~", "Negation", Negation),
                new MenuItem("sq", "Square Root", SquareRoot),
                new MenuItem("2", "Square", Square),
                new MenuItem("A", "Absolute Value", Absolute),
                new MenuItem("C", "Clear Current Value", Clear),
            });
            var value = unaMenu.RunMenu();
            return (double) value;
        }

        private static double Addition()
        {
            Console.Write($"{_currentValue} + ");
            var number = Console.ReadLine()?.Trim();
            double.TryParse(number, out var value);
            _currentValue = _currentValue + value;
            Console.WriteLine($"= {_currentValue}");
            return _currentValue;
        }

        private static double Substraction()
        {
            Console.Write($"{_currentValue} - ");
            var number = Console.ReadLine()?.Trim();
            double.TryParse(number, out var value);
            _currentValue = _currentValue - value;
            Console.WriteLine($"= {_currentValue}");
            return _currentValue;
        }

        private static double Multiplication()
        {
            Console.Write($"{_currentValue} * ");
            var number = Console.ReadLine()?.Trim();
            double.TryParse(number, out var value);
            _currentValue = _currentValue * value;
            Console.WriteLine($"= {_currentValue}");
            return _currentValue;
        }

        private static double Division()
        {
            Console.Write($"{_currentValue} / ");
            var number = Console.ReadLine()?.Trim();
            double.TryParse(number, out var value);
            _currentValue = _currentValue / value;
            Console.WriteLine($"= {_currentValue}");
            return _currentValue;
        }

        private static double XpowerY()
        {
            Console.Write("Input X: ");
            var X = double.Parse((Console.ReadLine()?.Trim()));
            Console.Write("Input Y: ");
            var Y = double.Parse((Console.ReadLine()?.Trim()));
            Console.WriteLine($"{X}^{Y} = {Math.Pow(X, Y)}");
            return _currentValue;
        }
        private static double Negation()
        {
            _currentValue = _currentValue * -1;
            Console.WriteLine($"Negated {_currentValue}");
            return _currentValue;
        }
        private static double SquareRoot()
        {
            if (_currentValue < 0)
            {
                Console.WriteLine("Cannot find the square root of negative numbers");
                return _currentValue;
            }
            Console.WriteLine($"SquareRoot of {_currentValue} =");
            _currentValue = Math.Sqrt(_currentValue);
            Console.WriteLine($" {_currentValue}");
            return _currentValue;
        }
        private static double Square()
        {
            Console.WriteLine($"Square of {_currentValue} =");
            _currentValue = _currentValue * _currentValue;
            Console.WriteLine($" {_currentValue}");
            return _currentValue;
        }
        private static double Absolute()
        {
            Console.WriteLine($"Absolute value of {_currentValue} =");
            _currentValue = Math.Abs(_currentValue);
            Console.WriteLine($" {_currentValue}");
            return _currentValue;
        }
        private static double Clear()
        {
            _currentValue = 0.0;
            return _currentValue;
        }
    }
}