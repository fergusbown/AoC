using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2020_21 : IDayChallenge
{
    private IReadOnlyCollection<(IReadOnlyCollection<string> Ingredients, IReadOnlyCollection<string> Allergens)> foods;

    public Day_2020_21(string inputData)
    {
        this.foods = ParseInput(inputData);
    }

    public string Part1()
    {
        AnalyseFood(out var inertCount, out _);
        return inertCount.ToString();
    }

    public string Part2()
    {
        AnalyseFood(out _, out var dangerousList);
        return dangerousList;
    }

    private static IReadOnlyCollection<(IReadOnlyCollection<string> Ingredients, IReadOnlyCollection<string> Allergens)> ParseInput(string inputData)
    {
        return inputData
            .StringsForDay()
            .Select(x =>
            {
                var parts = x[..^1].Split(" (contains ");
                IReadOnlyCollection<string> ingredients = parts[0].Split(' ');
                IReadOnlyCollection<string> allergens = parts[1].Split(", ");
                return (ingredients, allergens);
            })
            .ToArray();
    }

    private void AnalyseFood(out int inertCount, out string dangerousIngredients)
    {
        Dictionary<string, HashSet<string>> allergenToPossibleIngredients = new();

        foreach ((var ingredients, var allergens) in this.foods)
        {
            foreach (var allergen in allergens)
            {
                if (allergenToPossibleIngredients.TryGetValue(allergen, out var possibleIngredients))
                {
                    possibleIngredients.IntersectWith(ingredients);
                }
                else
                {
                    allergenToPossibleIngredients.Add(allergen, new(ingredients));
                }
            }
        }

        var allIngredients = foods.SelectMany(f => f.Ingredients);
        var ingredientsThatMayContainAllergens = allergenToPossibleIngredients.SelectMany(a => a.Value);
        var inertIngredients = new HashSet<string>(allIngredients.Except(ingredientsThatMayContainAllergens));

        inertCount = allIngredients.Count(i => inertIngredients.Contains(i));

        Dictionary<string, string> allergenToIngredient = new();

        while(allergenToPossibleIngredients.Count > 0)
        {
            foreach((var allergen, var ingredients) in allergenToPossibleIngredients)
            {
                if (ingredients.Count == 1)
                {
                    var resolvedIngredient = ingredients.First();
                    allergenToIngredient.Add(allergen, resolvedIngredient);
                    allergenToPossibleIngredients.Remove(allergen);

                    foreach (var unresolvedIngredients in allergenToPossibleIngredients.Values)
                    {
                        unresolvedIngredients.Remove(resolvedIngredient);
                    }
                }
            }
        }

        dangerousIngredients = string.Join(',', allergenToIngredient
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => kvp.Value));
    }
}
