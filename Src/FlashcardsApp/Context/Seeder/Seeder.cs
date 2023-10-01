using FlashcardsApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace FlashcardsApp.Context.Seeder
{
    public class Seeder
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<FlashcardAppDbContext>();

                context.Database.EnsureCreated();

                if (!context.Decks.Any())
                {

                    string jsonFilePath = "./Context/Seeder/ExampleData.json"; 

                    try
                    {
                        string jsonContent = File.ReadAllText(jsonFilePath, Encoding.UTF8);

                        List<Deck> decks = JsonConvert.DeserializeObject<List<Deck>>(jsonContent);
                        context.Decks.AddRange(decks);
                        context.SaveChanges();


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                }
                
            }
        }
    }
    
}
