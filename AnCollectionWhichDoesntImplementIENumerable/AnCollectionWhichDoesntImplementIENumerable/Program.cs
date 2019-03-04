using System;

namespace AnCollectionWhichDoesntImplementIENumerable
{
    class Program
    {
        static void Main(string[] args)
        {
            var myClasses = new Film[]
            {
                new Film("Jackie Chan's bizzare adventure"), new Film("Stargate The Film"), new Film("Men In Black 1"), 
            };

            var cinema = new Cinema(myClasses);


            foreach (var film in cinema)
            {
                Console.WriteLine(film.Title);
            }
        }
    }

    class Film
    {
        public string Title { get; }

        public Film(string title)
        {
            this.Title = title;
        }
    }

    class Cinema
    {
        private Film[] Collection { get; set; }

        public Cinema(Film[] myArray)
        {
            this.Collection = myArray;
        }

        public CinemaEnum GetEnumerator()
        {
            return new CinemaEnum(this.Collection);
        }
    }

    class CinemaEnum
    {
        private int position = -1;

        private Film[] Collection { get; }

        public CinemaEnum(Film[] list)
        {
            this.Collection = list;
        }

        public Film Current
        {
            get { return this.Collection[this.position]; }
        }


        public bool MoveNext()
        {
            this.position++;
            return (this.position < this.Collection.Length);
        }
    }
}
