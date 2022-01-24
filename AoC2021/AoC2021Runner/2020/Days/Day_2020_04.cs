namespace AoC2021Runner;

internal class Day_2020_04 : IDayChallenge
{
    private readonly Dictionary<string, string>[] inputData;

    public Day_2020_04(string inputData)
    {
        this.inputData = inputData
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => s.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            .Select(s => s.ToDictionary(v => v.Split(':')[0], v => v.Split(':')[1]))
            .ToArray();
    }

    public string Part1()
    {
        return inputData.Where(dict => dict.Count >= 7 &&
            dict.ContainsKey("byr") &&
            dict.ContainsKey("iyr") &&
            dict.ContainsKey("eyr") &&
            dict.ContainsKey("hgt") &&
            dict.ContainsKey("hcl") &&
            dict.ContainsKey("ecl") &&
            dict.ContainsKey("pid"))
            .Count().ToString();
    }

    public string Part2()
    {
        return inputData.Where(dict => dict.Count >= 7 &&
            dict.TryGetValue("byr", out var byr) && InRange(byr, 1920, 2002) &&
            dict.TryGetValue("iyr", out var iyr) && InRange(iyr, 2010, 2022) &&
            dict.TryGetValue("eyr", out var eyr) && InRange(eyr, 2020, 2030) &&
            dict.TryGetValue("hgt", out var hgt) && ValidHeight(hgt) &&
            dict.TryGetValue("hcl", out var hcl) && ValidHairColour(hcl) &&
            dict.TryGetValue("ecl", out var ecl) && ValidEyeColour(ecl) &&
            dict.TryGetValue("pid", out var pid) && ValidPassportNumber(pid))
            .Count().ToString();

        static bool InRange(string value, int min, int max)
        {
            return int.TryParse(value, out var intValue) && intValue >= min && intValue <= max;
        }

        static bool ValidHeight(string value)
        {
            if (value.Length > 2)
            {
                var units = value[^2..];

                if (units == "cm")
                {
                    return InRange(value[..^2], 150, 193);
                }
                else if (units == "in")
                {
                    return InRange(value[..^2], 59, 76);
                }
            }

            return false;
        }

        static bool ValidHairColour(string value)
        {
            if (value.Length == 7 && value[0] == '#')
            {
                foreach (var ch in value[1..])
                {
                    switch (ch)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            break;
                        default:
                            return false;
                    }
                }

                return true;
            }

            return false;
        }

        static bool ValidEyeColour(string value)
        {
            return value switch
            {
                "amb" => true,
                "blu" => true,
                "brn" => true,
                "gry" => true,
                "grn" => true,
                "hzl" => true,
                "oth" => true,
                _ => false,
            };
        }

        static bool ValidPassportNumber(string value)
        {
            return value.Length == 9 && int.TryParse(value, out _);
        }
    }
}