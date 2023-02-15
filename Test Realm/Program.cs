using Realms;
using Diabets;
using Diabets.Models;
using Realms.Exceptions;
// See https://aka.ms/new-console-template for more information
namespace Realm_Demo
{
    class Program
    {
        public static string Pathgetter()
        {

            string path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string actualPath = path.Substring(0, path.LastIndexOf("bin"));
            actualPath = actualPath.Substring(0, actualPath.LastIndexOf("/"));
            actualPath = actualPath.Substring(0, actualPath.LastIndexOf("/"));
            string projectPath = new Uri(actualPath).LocalPath;
            path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, projectPath);
            return path;
        }
        public static Realm RealmCreate()
        {
            Console.WriteLine(Pathgetter());
            string pathToDb = Pathgetter();
            var config = new RealmConfiguration(pathToDb + "/Test Realm/my.realm")
            {
                IsReadOnly = false,
            };
            Realm localRealm;
            try
            {
                localRealm = Realm.GetInstance(config);
            }
            catch (RealmFileAccessErrorException ex)
            {
                Console.WriteLine($@"Error creating or opening the
        realm file. A! {ex.Message}");
                return null;
            }
            return localRealm;
        }
        static async Task Main(string[] args)
        {
            Realm LocalRealm = RealmCreate();

            try
            {
                
                string dbFullPath = LocalRealm.Config.DatabasePath;

                //Create a new Employee and link it to a new Company

                var Food1 = new Food { Name = "Ap", Calories = 500, Carbohydrates = 20, Fat = 1000, Protein = 10 };
                var Foodentry1 = new FoodEntry { Food = Food1, Amout = 20 };
                var Foodentries1 = new FoodEntries { Timestamp = new DateTimeOffset(2011, 6, 10, 15, 24, 16, TimeSpan.Zero) };
                Foodentries1.FoodEntry.Add(Foodentry1);

                //Let's create an other employee containing a mistake that we will fix later on
                var Food2 = new Food { Name = "Ca", Calories = 5000, Carbohydrates = 2010, Fat = 100, Protein = 10 };
                var Foodentry2 = new FoodEntry { Food = Food2, Amout = 30 };
                var Foodentries2 = new FoodEntries { Timestamp = new DateTimeOffset(2011, 6, 10, 15, 24, 20, TimeSpan.Zero) };
                Foodentries2.FoodEntry.Add(Foodentry2);

                //THIS IS HOW TO WRITE IN REALM DB IN 3 DIFFERENT WAYS:

                // 1) Update and persist objects with a thread-safe transaction
                await LocalRealm.WriteAsync((tmpRealm) =>
                {
                    //tmpRealm.RemoveAll(); //Clean the db
                    tmpRealm.Add(Foodentries1);
                    tmpRealm.Add(Foodentries2);
                });

         
                //Refresh the Realm instance
                LocalRealm.Refresh();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                //Cleaning
                //realm.Dispose();
                //Realm.DeleteRealm(config); //Delete the db
            }

            Console.WriteLine("\r\nPress any key...");
            Console.ReadKey();
        }
    }
}
