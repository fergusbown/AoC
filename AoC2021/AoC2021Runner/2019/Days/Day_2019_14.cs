using System.Xml.Linq;
using Generator.Equals;

namespace AoC2021Runner;

internal partial class Day_2019_14 : IDayChallenge
{
    private readonly IReadOnlyCollection<Element> recipes;

    public Day_2019_14(string inputData)
    {
        this.recipes = ParseInput(inputData);
    }

    public string Part1()
    {
        return Element.FuelCost(recipes).ToString();
    }

    public string Part2()
    {
        return Element.FuelProducable(recipes, 1_000_000_000_000).ToString();
    }

    private static IReadOnlyCollection<Element> ParseInput(string inputData)
    {
        Dictionary<string, Element> elements = new();
        
        var recipes = inputData
            .StringsForDay()
            .Select(r => r.Split(new String[] { " ", ",", "=>"}, StringSplitOptions.RemoveEmptyEntries));

        Dictionary<string, Element> result = new();

        foreach (string[] recipe in recipes)
        {
            List<Ingredient> ingredients = new();
            for (int i = 0; i < recipe.Length-2; i+=2)
            {
                ingredients.Add(new Ingredient(GetElement(recipe[i + 1]), int.Parse(recipe[i])));
            }

            Element element = GetElement(recipe[^1]);
            element.Production = int.Parse(recipe[^2]);
            element.Ingredients = ingredients;
        }

        return result.Values;

        Element GetElement(string name)
        {
            if (!result.TryGetValue(name, out Element? element))
            {
                element = new Element(name);
                result.Add(name, element);
            }

            return element;
        }
    }

    private class Element
    {
        public Element(string name)
        {
            this.Name = name;
            this.Ingredients = new List<Ingredient>();
        }

        public string Name { get; }

        public int Production { get; set; }

        public IList<Ingredient> Ingredients { get; set; }

        public static int FuelCost(IReadOnlyCollection<Element> elements)
        {
            var availableIngredients = elements.ToDictionary(e => e, _ => 0);
            return elements.Single(e => e.Name == "FUEL").Cost(1, availableIngredients);
        }

        public static int FuelProducable(IReadOnlyCollection<Element> elements, long oreAvailable)
        {
            var availableIngredients = elements.ToDictionary(e => e, _ => 0);
            var fuel = elements.Single(e => e.Name == "FUEL");

            int cost;
            int fuelProduced = 0;
            long remainingOre = oreAvailable;
            while((cost = fuel.Cost(1, availableIngredients)) <= remainingOre)
            {
                fuelProduced++;
                remainingOre -= cost;
            }

            return fuelProduced;
        }

        public int Cost(int required, IDictionary<Element, int> availableIngredients)
        {
            if (Production == 0) // ORE
            {
                return required;
            }

            if (availableIngredients[this] >= required)
            {
                availableIngredients[this] -= required;
                return 0;
            }

            required -= availableIngredients[this];
            availableIngredients[this] = 0;

            int runs = required / Production;
            int remaining = required % Production;

            if (remaining != 0)
            {
                runs++;
            }

            int result = 0;
            foreach (var ingredient in Ingredients)
            {
                result += ingredient.Element.Cost(ingredient.Required * runs, availableIngredients);
            }

            if (remaining != 0)
            {
                availableIngredients[this] += Production - remaining;
            }

            return result;
        }
    }

    private class Ingredient
    {
        public Ingredient(Element element, int required)
        {
            Element = element;
            Required = required;
        }

        public Element Element { get; }

        public int Required { get; }
    }
}
