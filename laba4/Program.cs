using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace laba4
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                DeveloperFactory factory = SelectFactory();
                Developer developer = (Developer)factory.create_Entity();
                developer.SelectId();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nPress [Esc] to exit.");
                Console.WriteLine("Press [Enter] to Continue.\n");
                Console.ForegroundColor = ConsoleColor.White;
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        static DeveloperFactory SelectFactory()
        {
            Console.WriteLine("Select Category");
            Console.WriteLine("===========================");
            Console.WriteLine("1. Student");
            Console.WriteLine("2. Teacher");
            Console.WriteLine("3. Course");
            Console.WriteLine("===========================");
            while (true)
            {
                Console.Write(">"); string name = Console.ReadLine();
                if (name == "1") return new StudentFactory();
                else
                if (name == "2") return new TeacherFactory(); else
                     if (name == "3") return new CourseFactory();
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }



    }

    interface Developer
    {
        void SelectId()
        {
            Console.WriteLine("Select Action");
            Console.WriteLine("===========================");
            Console.WriteLine("1. Show information");
            Console.WriteLine("2. Add information");
            Console.WriteLine("===========================");
            while (true)
            {
                Console.Write(">"); string id = Console.ReadLine();
                if (id == "1")
                {
                    this.get_Information_from_DB();
                    break;
                }
                else

                    if (id == "2")
                {
                    this.add_Information_on_DB();
                    break;
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: try again");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        void add_Information_on_DB();
        void get_Information_from_DB();
    }

    abstract class WorkDB
    {
        private string OnetoMany(string id, string table)
        {
            string[] lines = System.IO.File.ReadAllLines("database_lab4.txt");
            foreach (string line in lines)
            {
                var l = line.Split('.');
                if (l[0] == table && l[1] == id)
                    return l[2]; 
            }
            return "Error";
            }

        protected string getDB(string id, string group, string table_name=null,string table = null, string table_name2=null, string table2=null)
        {
            string[] lines = System.IO.File.ReadAllLines("database_lab4.txt");
            string print;
            if (id == "all")
            {
                 print = "Group \t \t  Id \t  Name \t\t\t\t";
                if (table_name != null) print += table_name;
                if (table_name2 != null) print +="\t\t\t"+ table_name2;
                    print += "\n";
                foreach (string line in lines)
                {
                    var l = line.Split('.');
                    if (l[0] == group)
                    { 
                        print = print + l[0] + " \t| " + l[1] + " \t| " + l[2] + "\t\t";
                        var named = l[3].Split(',');
                        foreach(string idn in named)
                        {
                            if (table != null) print = print + OnetoMany(idn, table) + ", "; else print += idn;
                        }
                        if (table_name2 != null)
                        {
                            var named2 = l[4].Split('!');
                            print += "\t\t";
                            foreach (string idn in named2)
                            {
                                if (table2 != null) print = print + OnetoMany(idn, table2) + ", "; else print += idn;
                            }
                        }
                        print += "\n";
                    }  
                }
                return print;
            }
            else
            {
                foreach (string line in lines)
                {
                    var l = line.Split('.');
                    if (l[1] == id && l[0] == group)
                    {
                        print="Name:"+l[2]+"\t  ";
                        if (table_name != null)
                        {
                            print += table_name + ": ";
                            var named = l[3].Split(',');
                            foreach (string idn in named)
                            {
                                if (table != null) print += OnetoMany(idn, table) + " "; else print +=idn;
                            }
                            if (table_name2 != null)
                            {
                                print += "\t" + table_name2+": ";
                                var named2 = l[4].Split('!');
                                foreach (string idn in named2)
                                {
                                    if (table2 != null) print = print + OnetoMany(idn, table2) + ", "; else print += idn;
                                }
                            }
                            print += "\n";
                        }
                        return print;
                    }
                }
                return "Error: not found";
            }
        }

        protected string addDB(string name, string group,string table1=null,string table2 = null)
        {
            int counter = 0;
            string str = "";
            string[] lines = System.IO.File.ReadAllLines("database_lab4.txt");
            foreach (string line in lines)
            {
                counter++;
                var l = line.Split('.');
                if (l[0] == group)
                {
                    str = l[1];
                }
                else
                if (str != "")
                {
                    str = group + "." + Convert.ToString(Convert.ToInt32(str) + 1) + "." + name;
                    counter--;
                    break;
                }
                if( lines.Length == counter && str != "")
                {
                    str = group + "." + Convert.ToString(Convert.ToInt32(str) + 1) + "." + name;
                    break;
                }
            }
            if (str != "")
            {
                if (table1 != null) str += "." + table1;
                if (table2 != null) str += "." + table2;
                var line = System.IO.File.ReadAllLines("database_lab4.txt").ToList();
                line.Insert(counter, str);
                File.WriteAllLines("database_lab4.txt", line);
                return "Done";
            }
            else
                return "Error";
        }
    }

    class Student : WorkDB, Developer
    {
        private string group = "student";
        private string table = "course";
        void Developer.get_Information_from_DB()
        {
            Console.Write("Write Id Student >>");
            var id = Console.ReadLine();
            Console.WriteLine(getDB(id, group,table,table));
        }

        void Developer.add_Information_on_DB()
        {
            Console.Write("Write Name Student >>");
            var name = Console.ReadLine();
            Console.Write(addDB(name,group));
        }
    }

    class Course : WorkDB, Developer
    {
        private string group = "course";
        private string table = "teacher";
        private string table2 = "student";
        void Developer.get_Information_from_DB()
        {
            Console.WriteLine("Print «all» to see all Сourses");
            Console.Write("Write Id Course >>");
            var id = Console.ReadLine();
          Console.WriteLine(getDB(id, group,"teachers",table,"student",table2));
        }

        void Developer.add_Information_on_DB()
        {
            Console.Write("Write Name Course >>");
            var name = Console.ReadLine();
            Console.Write("Who teacher this Course? write id separated by comma >>");
            var teacher = Console.ReadLine();
            Console.Write("Who is taking this course? write id separated by ! >>");
            var students = Console.ReadLine();
            Console.Write(addDB(name, group,teacher,students));
        }
    }

    class Teacher :WorkDB, Developer
    {
        private string group = "teacher";
        private string table = "course";
        void Developer.get_Information_from_DB()
        {
            Console.Write("Write Id Teacher >>");
            var id = Console.ReadLine();
            Console.WriteLine(getDB(id, group,"courses", table,"years")); ;
        }

        void Developer.add_Information_on_DB()
        {
            Console.Write("Write Name Teacher >>");
            var name = Console.ReadLine();
            Console.Write("Write years Teacher >>");
            var years = Console.ReadLine();
            Console.Write("Write course Teacher >>");
            var course = Console.ReadLine();
            Console.Write(addDB(name, group,course,years));
        }
    }

 interface DeveloperFactory
    {
       Object create_Entity();
    }

    public class StudentFactory : DeveloperFactory
    {
        Object DeveloperFactory.create_Entity()
        {
            return new Student();
        }
    }

    public class TeacherFactory : DeveloperFactory
    {
        Object DeveloperFactory.create_Entity()
        {
            return new Teacher();
        }
    }

    public class CourseFactory : DeveloperFactory
    {
        Object DeveloperFactory.create_Entity()
        {
            return new Course();
        }
    }

}
