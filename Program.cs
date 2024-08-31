class Program
{







    static void Main(string[] args)
    {
        // The input string to be analyzed
        string inputString = @"
        effect {
            Name: ""Damage"",
            Params: {
                Amount: Number,
            },
            Action: (targets, context) => {
                    for target in targets {
                    i = 0;
                    while (i < Amount)
                    {
                        target.Power -= 1;
                        i+=1;
                    }
                };
            }
        }

        effect {
            Name: ""Draw"",
            Action: (targets, context) => {
                     topCard = context.Deck.Pop();
                context.Hand.Add(topCard);
                context.Hand.Shuffle();

            }
        }

        effect {
            Name: ""ReturnToDeck"",
            Action: (targets, context) => {
                    for target in targets {
                    owner = target.Owner;
                    deck = context.DeckOfPlayer(owner);
                    deck.Push(target);
                    deck.Shuffle();
                    context.Board.Remove(target);
                };
            }
        }

        card {
            Type: ""Oro"",
            Name: ""Beluga"",
            Faction: ""Northern Realms"",
            Power: 10,
            Range: [""Melee"", ""Ranged""],
            OnActivation: [
                {
                    Effect: {
                        Name: ""Damage"",
                        Amount: 5,
                    },
                    Selector: {
                        Source: ""board"",
                        Single: false,
                        Predicate: (unit) => unit.Faction == ""Northern Realms""
                    },
                    PostAction: {
                        Effect: {
                            Name: ""ReturnToDeck"",
                        },
                        Selector: {
                            Source: ""parent"",
                            Single: false,
                            Predicate: (unit) => unit.Power < 1
                        },
                        PostAction: {
                            Effect: {
                                Name: ""Draw"",
                            },
                        },
                    },
                },
            ]
        }";

        // Create a Lexer object with the input string
        Lexer lexer = new Lexer(inputString);

        // Call the Tokenizar method to tokenize the input
        List<Token> tokens = lexer.Tokenizar();

        // Print each token
        foreach (var token in tokens)
        {
            Console.WriteLine($"Type: {token.TokenType}, Value: {token.TokenValue}");
        }

        Console.WriteLine("\n----------------------------------\n");

        Parser parser = new Parser(tokens);

        List<ASTNode> nodes = parser.Parse();

        foreach (var node in nodes)
        {
            node.Print(0);
        }
    }
}