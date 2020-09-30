using System.Text.Json.Serialization;

namespace ScraperTechTest.Model
{
    public class Dish
    {
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }

        [JsonIgnore]
        public string DishPage { get; set; }
    }
}
