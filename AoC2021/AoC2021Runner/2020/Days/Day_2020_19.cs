namespace AoC2021Runner;

internal class Day_2020_19 : IDayChallenge
{
    private readonly string inputData;

    public Day_2020_19(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var rules, var messages) = ParseInput(this.inputData);
        return $"{messages.Count(m => rules[0].Matches(m))}";
    }

    public string Part2()
    {
        (var rules, var messages) = ParseInput(this.inputData);

        // muck with the parsed rules to handle the recursion

        /*
        var modifiedInput = this.inputData
            .Replace("8: 42", "8: 42 | 42 8")
            .Replace("11: 42 31", "11: 42 31 | 42 11 31");
        */
        var rule42 = rules[42];
        var rule31 = rules[31];

        // new version of rule 8 translates to arbitrary number of matches to rule 42
        rules[8].OverrideMatchRule((startIndex, input) =>
        {
            HashSet<int> result = new();
            Stack<int> pending = new();
            pending.Push(startIndex);

            while (pending.TryPop(out var start))
            {
                foreach (var newStart in rule42.PartialMatches(start, input))
                {
                    if (result.Add(newStart))
                    {
                        pending.Push(newStart);
                    }
                }
            }

            return result;
        });

        // new version of rule 11 translates to n matches of rule 42, followed by n matches of rule 31
        rules[11].OverrideMatchRule((startIndex, input) =>
        {
            int trys = (input.Length - startIndex) / 2;
            HashSet<int> result = new();

            for (int t = 1; t <= trys; t++)
            {
                var list = Enumerable.Repeat(rule42, t).Concat(Enumerable.Repeat(rule31, t)).ToArray();
                var rule = new ListRule(list);

                result.UnionWith(rule.PartialMatches(startIndex, input));
            }

            return result;
        });


        return $"{messages.Count(m => rules[0].Matches(m))}";
    }

    private static (Dictionary<int, IRule> rules, string[] messages) ParseInput(string input)
    {
        string[] parts = input.Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);

        string[] ruleStrings = parts[0].StringsForDay();

        // easier to have an extra white space at the end when parsing
        Queue<string> pendingRules = new(ruleStrings.Select(s => $"{s} "));
        Dictionary<int, IRule> rules = new();

        while (pendingRules.TryDequeue(out var pendingRule))
        {
            if (!TryBuildRule(pendingRule, rules))
            {
                pendingRules.Enqueue(pendingRule);
            }
        }

        if (parts.Length > 1)
        {
            return (rules, parts[1].StringsForDay());
        }
        else
        {
            return (rules, Array.Empty<string>());
        }

        static bool TryBuildRule(string ruleString, Dictionary<int, IRule> rules)
        {
            var ruleSpan = ruleString.AsSpan();
            int? number = null;
            int ruleId = 0;

            List<IRule> currentRuleSet = new();
            List<IRule> currentRule = new();

            while (!ruleSpan.IsEmpty)
            {
                var nextCharacter = ruleSpan[0];
                ruleSpan = ruleSpan[1..];

                switch (nextCharacter)
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
                        number = ((number ?? 0) * 10) + (nextCharacter - '0');
                        break;
                    case ':':
                        ruleId = number!.Value;
                        number = null;
                        break;
                    case '"':
                        rules.Add(ruleId, new BasicRule(ruleSpan[0]));
                        return true;
                    case '|':
                        currentRuleSet.Add(new ListRule(currentRule));
                        currentRule.Clear();
                        break;
                    default:
                        if (number is not null)
                        {
                            if (!rules.TryGetValue(number.Value, out var rule))
                            {
                                return false;
                            }

                            currentRule.Add(rule);
                            number = null;
                        }
                        break;
                }
            }

            currentRuleSet.Add(new ListRule(currentRule));

            if (currentRuleSet.Count == 1)
            {
                rules.Add(ruleId, currentRuleSet[0]);
            }
            else
            {
                rules.Add(ruleId, new ChoiceRule(currentRuleSet));
            }

            return true;
        }
    }

    private interface IRule
    {
        IEnumerable<int> PartialMatches(int startIndex, string input);

        bool Matches(string input);

        void OverrideMatchRule(Func<int, string, IEnumerable<int>> newPartialMatches);
    }

    private abstract class AbstractRule : IRule
    {
        private Func<int, string, IEnumerable<int>>? newPartialMatches = null;

        public bool Matches(string input)
            => DoPartialMatches(0, input).Any(i => i == input.Length);

        public IEnumerable<int> PartialMatches(int startIndex, string input)
        {
            return (newPartialMatches ?? DoPartialMatches).Invoke(startIndex, input);
        }

        public void OverrideMatchRule(Func<int, string, IEnumerable<int>> newPartialMatches)
        {
            this.newPartialMatches = newPartialMatches;
        }

        protected abstract IEnumerable<int> DoPartialMatches(int startIndex, string input);
    }

    private class BasicRule : AbstractRule
    {
        private readonly char match;

        public BasicRule(char match)
        {
            this.match = match;
        }

        protected override IEnumerable<int> DoPartialMatches(int startIndex, string input)
        {
            if (startIndex < input.Length && input[startIndex] == match)
            {
                yield return startIndex + 1;
            }
        }
    }

    private class ListRule : AbstractRule
    {
        private readonly IReadOnlyCollection<IRule> rules;

        public ListRule(IReadOnlyCollection<IRule> rules)
        {
            this.rules = rules.ToArray();
        }

        protected override IEnumerable<int> DoPartialMatches(int startIndex, string input)
        {
            var matches = rules.First().PartialMatches(startIndex, input);

            foreach (var rule in rules.Skip(1))
            {
                List<int> nextMatches = new();

                foreach (var match in matches)
                {
                    nextMatches.AddRange(rule.PartialMatches(match, input));
                }

                matches = nextMatches;
            }

            return matches;
        }
    }

    private class ChoiceRule : AbstractRule
    {
        private readonly IReadOnlyCollection<IRule> choices;

        public ChoiceRule(IReadOnlyCollection<IRule> choices)
        {
            this.choices = choices.ToArray();
        }

        protected override IEnumerable<int> DoPartialMatches(int startIndex, string input)
        {
            foreach (var rule in choices)
            {
                foreach (var index in rule.PartialMatches(startIndex, input))
                {
                    yield return index;
                }
            }
        }
    }
}
