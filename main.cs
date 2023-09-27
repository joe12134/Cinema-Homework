
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Cinema_Booking_System
{
    internal class Program
    {




        class Theatre
        {
            //gonna be 2d array for theatre seats :)
            public string[,] seatingArray;
            //to check if a movie can use this theatre for showing

            public int seatsAvailable;
            //when constructing a theatre give the constructor class the dimensions please

            public Theatre(int width, int height)
            {
                //create a nice array of the movie theatre
                seatingArray = new string[width, height];
                seatsAvailable = width * height;

                for (int i = 0; i < seatingArray.GetLength(0); i++)
                {
                    for (int ii = 0; ii < seatingArray.GetLength(1); ii++)
                    {
                        //could maybe make this an array of a Seat class
                        seatingArray[i, ii] = "O";

                    }
                }

            }

            //if they need seating for a theatre (to check its not full or decide what seat they want to pick, then they can do that using this method)

            public void DisplaySeating()
            {

                for (int i = 0; i < seatingArray.GetLength(0); i++)
                {
                    string row = "";
                    //put this in a varable cuz i need it multiple times
                    int arrayLength = seatingArray.GetLength(1);
                    for (int ii = 0; ii < arrayLength; ii++)
                    {
                        row += seatingArray[i, ii];
                        //display the numbers at the top to help them pick the row and column
                        if (i == 0)
                        {

                            //add the few bits of whitespace to the front of the row numbers
                            string valToOutput = (ii == 0 ? "  " : "") + (ii + 1);
                            //if at the end of the array then do newline so the first line of row variable isnt appeneded to the end of this
                            valToOutput = ii == arrayLength - 1 ? valToOutput + "\n" : valToOutput + "";

                            Console.Write(valToOutput);



                        }
                    }

                    //column numbers
                    Console.Write(i + 1);
                    //output row
                    Console.WriteLine(" " + row);
                }
            }

            //returns "booked" if booked and, "cancelled" if cancelled


        }




        //each movie has a theatre
        class Movie
        {
            public Theatre showingTheatre;
            public string ageRating;
            public string title;
            public int ticketsSold = 0;
            public DateTime movieDate;
            public Movie(Theatre theatre, string movieTitle, string ageReq, DateTime date)
            {

                showingTheatre = theatre;
                title = movieTitle;
                ageRating = ageReq;
                movieDate = date;
            }
        }



        class Booking
        {
            public Movie yourMovie;
            public double price = 0.00;
            public int people = 0;
            public List<Movie> currentMovies;

            public void ChooseMovie()
            {
                Movie newMovie = null;
                while (newMovie == null)
                {
                    for (int i = 0; i < currentMovies.Count; i++)
                    {
                        Movie movie = currentMovies[i];
                        if (movie.ticketsSold + people >= movie.showingTheatre.seatsAvailable)
                        {
                            Console.WriteLine($"{i + 1}: This movie is sold out and cannot be displayed");
                        }
                        Console.WriteLine($"{i + 1}:  {movie.title},  Age Rating: {movie.ageRating}, Tickets Sold: {movie.ticketsSold}/{movie.showingTheatre.seatsAvailable}, Showing time {movie.movieDate.ToShortDateString()} at {movie.movieDate.ToShortTimeString()}");
                    }
                    try
                    {
                        Console.WriteLine("Enter the number of the movie you would like to watch, hit x to cancel: ");

                        string movieChoice = Console.ReadLine().Trim();
                        if (CheckCancelled(movieChoice))
                        {
                            return;
                        }
                        int movieNum = int.Parse(movieChoice) - 1;

                        if (movieNum <= currentMovies.Count - 1)
                        {


                            newMovie = currentMovies[movieNum];
                            yourMovie = currentMovies[movieNum];


                            if (yourMovie.ticketsSold + people >= yourMovie.showingTheatre.seatsAvailable)
                            {
                                Console.WriteLine($"This movie is sold out and cannot be purchased from.");
                                continue;
                            }


                        }
                        else
                        {
                            { Console.WriteLine("Movie does not exist"); }
                        }

                    }
                    catch
                    {
                        Console.WriteLine("An error occured, please try again.");
                    }
                }

            }




            public Booking(List<Movie> currentMoviesa, int peoplea)
            {
                people = peoplea;


                currentMovies = currentMoviesa;
                for (int i = 1; i <= people; i++)
                {
                    string booked = "false";
                    int age = 0;
                    while (booked == "false")
                    {
                        if (yourMovie == null)
                        {

                            ChooseMovie();
                        }



                        string ageRating = yourMovie.ageRating;
                        if (age == 0)
                        {
                            Console.WriteLine($"You have chosen {yourMovie.title}, Person {i}, please enter your age.");
                            try
                            {
                                age = int.Parse(Console.ReadLine().Trim());
                            }
                            catch
                            {
                                Console.WriteLine("Invalid input");
                                continue;
                            }

                        }

                        int realAgeRating = 0;
                        switch (ageRating)
                        {
                            case "U":
                                realAgeRating = 0;
                                break;
                            case "PG":
                                realAgeRating = 0;
                                break;
                            default:
                                realAgeRating = int.Parse(ageRating);
                                break;
                        }
                        if (age >= realAgeRating)

                        {
                            Console.WriteLine("You are old enough to watch this movie, lets choose seating!");
                            if (age < 12)


                            {

                                price += 3.50;

                            }
                            else
                            {
                                price += 7.00;
                            }


                            string response = ChooseSeat();
                            if (response == "cancelled")
                            {
                                Console.Clear();
                                return;

                            }
                            else if (response == "booked")
                            {

                                booked = "true";



                            }
                        }
                        else
                        {
                            Console.WriteLine("You are not old enough to watch this movie");
                            ChooseMovie();

                        }





                    }




                }



                //add here display movies, get input on movies, do whatever the task says on google, check if the 
                //specific movie is in full capacity with Movie.Theatre.seats <= Movie.ticketsSold
                Console.WriteLine($"Your price will be: £{price.ToString("0.00")}");
                Console.WriteLine("Refreshing...");
                Thread.Sleep(3000);
                Console.Clear();



            }


            //misc function to check if any inputs contain x which cancel the operation
            public bool CheckCancelled(string input)
            {
                if (input.ToLower().Trim() == "x")
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }


            public string ChooseSeat()
            {

                try
                {
                    string[,] seatingArray = yourMovie.showingTheatre.seatingArray;

                    yourMovie.showingTheatre.DisplaySeating();
                    Console.WriteLine("Type x to cancel booking at any time");
                    //get the row from the person
                    Console.Write("Enter Row: ");

                    string rowString = Console.ReadLine().Trim();

                    if (CheckCancelled(rowString) == true)
                    {
                        Console.WriteLine("Cancelled seat choosing");
                        return "cancelled";


                    }
                    int row = int.Parse(rowString) - 1;


                    Console.Write("Enter Column: ");
                    string columnString = Console.ReadLine().Trim();

                    if (CheckCancelled(columnString) == true)
                    {
                        Console.WriteLine("Cancelled booking");
                        return "cancelled";


                    }
                    int column = int.Parse(columnString) - 1;


                    if (seatingArray[row, column] == "X")
                    {
                        Console.WriteLine("This seat is currently taken, please try again");
                        return ChooseSeat();
                    }
                    else
                    {
                        Console.Clear();
                        seatingArray[row, column] = "X";
                        Console.WriteLine($"This seat is free, you are now booked in seat {row + 1}:{column + 1}, welcome to the theatre");

                        yourMovie.ticketsSold += 1;
                        return "booked";
                    }

                }
                catch
                {
                    Console.WriteLine("Invalid input please try again");
                    return ChooseSeat();
                }


            }
        }



        //admin can create new movies, but they have to have the correct password
        class Admin
        {

            public List<Movie> CreateMovie(List<Movie> movieList, Dictionary<string, Theatre> theatres)
            {

                Console.WriteLine("Enter Movie Title");
                string title = Console.ReadLine().Trim();
                Console.WriteLine("Enter Movie Rating");
                string ageRating = Console.ReadLine().Trim();



                bool accepted = false;
                while (accepted == false)
                {
                    try
                    {
                        Console.WriteLine("Enter Movie Theatre\nA, B, or C, enter \"X\" to CANCEL.");
                        string theatre = Console.ReadLine().ToUpper().Trim();
                        if (theatre == "X")
                        {
                            return movieList;
                        }
                        Console.WriteLine("Enter movie date as dd/mm/yyyy");
                        DateTime movieDate = DateTime.Parse(Console.ReadLine().Trim());
                        if (movieDate < DateTime.Now)
                        {
                            Console.WriteLine("Cant have a movie in the past");
                            continue;
                        }
                       
                        Movie mov = new Movie(theatres[theatre], title, ageRating, movieDate);
                        movieList.Add(mov);
                        accepted = true;
                        return movieList;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid Theatre ID or date");

                    }
                }



                return movieList;
            }

            public List<Movie> DeleteMovie(List<Movie> movieList)
            {


                for (int i = 0; i < movieList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {movieList[i].title}");
                }
                Console.WriteLine("Type the number of the movie you would like to remove, then hit enter, type \"x\" to cancel");
                string movieNumString = Console.ReadLine().Trim();
                if (movieNumString != "x")
                {
                    try
                    {
                        int movieNumInt = int.Parse(movieNumString);
                        movieList.Remove(movieList[movieNumInt - 1]);
                        Console.WriteLine($"{movieList[movieNumInt - 1].title} has been removed");

                    }
                    catch
                    {
                        Console.WriteLine("Operation failed.");
                    }
                }



                return movieList;
            }




            public void LogOut()
            {
                Console.Clear();
                Console.WriteLine("Logging out..");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        //main
        static void Main(string[] args)
        {
            //make movie dictionary 

            Dictionary<string, Theatre> theatres = new Dictionary<string, Theatre>();
            theatres.Add("A", new Theatre(5, 5));
            theatres.Add("B", new Theatre(5, 9));
            theatres.Add("C", new Theatre(9, 5));

            List<Movie> moviesShowing = new List<Movie>();
            moviesShowing.Add(new Movie(theatres["A"], "The Rental", "U", DateTime.Now + new TimeSpan (25,0,0) ));
            moviesShowing.Add(new Movie(theatres["B"], "The Mortgage", "U" , DateTime.Now + new TimeSpan(25, 0, 0)));
            moviesShowing.Add(new Movie(theatres["C"], "The Debt", "U", DateTime.Now + new TimeSpan(25, 0, 0)));
            moviesShowing.Add(new Movie(theatres["A"], "The Toast", "PG", DateTime.Now + new TimeSpan(30, 0, 0)));
            moviesShowing.Add(new Movie(theatres["B"], "The Rental 2", "18",DateTime.Now + new TimeSpan(30, 0, 0)));



            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to cinema system\nIf you are an admin and want to edit movies type \"yes\" and hit enter\nElse, hit enter.");
                string decision = Console.ReadLine().Trim().ToLower();
                if (decision == "yes")
                {
                    //admin 
                    Console.WriteLine("Enter username: ");
                    string user = Console.ReadLine();
                    Console.WriteLine("Enter password: ");
                    string pass = Console.ReadLine();
                    if (pass == "password" && user == "username")
                    {
                        Admin admin = new Admin();
                        Console.WriteLine("Welcome administrator");
                        bool loggedOut = false;
                        while (!loggedOut)
                        {
                            Console.WriteLine("What would you like to do?\n1: Logout\n2: Create a movie\n3: Delete a movie.");
                            switch (Console.ReadLine().Trim())
                            {
                                case "1":
                                    loggedOut = true;
                                    admin.LogOut();
                                    break;
                                case "2":
                                    moviesShowing = admin.CreateMovie(moviesShowing, theatres);
                                    break;
                                case "3":
                                    moviesShowing = admin.DeleteMovie(moviesShowing);
                                    break;
                                default:
                                    continue;



                            }

                        }


                    }
                    else
                    {
                        Console.WriteLine("Incorrect details :(");
                        Thread.Sleep(1000);
                    }

                }
                else if (decision == "shutdown")
                {
                    Environment.Exit(0);
                }
                else
                {

                    Console.WriteLine("Welcome to the Cinema Booking System, your booking will commence now!");

                    Console.WriteLine("Input how many people are booking: ");

                    try
                    {
                        int bookers = int.Parse(Console.ReadLine().Trim());
                        Booking newBooking = new Booking(moviesShowing, bookers);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input");
                    }



                }



            }
        }
    }
}
