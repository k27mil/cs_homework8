using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_homework8
{
    public class Song
    {
        private string _name;
        private string _author;
        private Song _prev;

        public Song()
        {
            _name = "Без названия";
            _author = "Неизвестен";
            _prev = null;
        }

        public Song(string name, string author)
        {
            _name = name;
            _author = author;
            _prev = null;
        }

        public Song(string name, string author, Song prev)
        {
            _name = name;
            _author = author;
            _prev = prev;
        }

        public string Title()
        {
            return $"{_name} — {_author}";
        }

        public void Print()
        {
            Console.WriteLine(Title());
        }

        public override bool Equals(object obj)
        {
            if (obj is Song other)
            {
                return _name == other._name && _author == other._author;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (_name, _author).GetHashCode();
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Song mySong = new Song();
            Song song1 = new Song("s1", "a");
            Song song2 = new Song("s2", "a", song1);

            mySong.Print();
            song1.Print();
            song2.Print();
            Console.WriteLine($"song1 и song2 равны: {song1.Equals(song2)}");
        }
    }
}
