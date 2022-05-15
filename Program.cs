using System.Text;

class Program
{
    public static void Main()
    {
        // Required to get the rounded box-drawing characters
        Console.OutputEncoding = Encoding.UTF8;

        TestPartA();
        TestPartB();

        Console.WriteLine("Done!");
    }


    public static void TestPartA()
    {
        Console.WriteLine("Running tests for part A (ternary trie):");

        var trie1 = new Trie<int>();

        trie1.Insert("Spongebob", 50);
        trie1.Insert("Patrick", 60);
        trie1.Insert("Plankton", 70);
        trie1.Insert("Sandy", 80);

        trie1.PrintDebug();

        Console.WriteLine("\nRemoving 'asdvrvwervas'...\n");
        trie1.Remove("asdvrvwervas");

        Console.WriteLine();
        trie1.PrintDebug();

        Console.WriteLine("\nRemoving ''...\n");
        trie1.Remove("");

        foreach (var key in new string[] { "Spongebob", "Patrick", "Plankton", "Sandy", "Test" })
        {
           Console.WriteLine($"Attempting to get key {key}: {trie1.Get(key)}");
        }

        Console.WriteLine("\nRemoving 'Spongebob'...\n");

        trie1.Remove("Spongebob");

        foreach (var key in new string[] { "Spongebob", "Patrick", "Plankton", "Sandy", "Test" })
        {
           Console.WriteLine($"Attempting to get key {key}: {trie1.Get(key)}");
        }

        Console.WriteLine();

        trie1.PrintDebug();
    }


    public static void TestPartB()
    {
        Console.WriteLine("Running tests for part B (binomial heap):");
        Console.WriteLine("Generating a new heap:");

        BinomialHeap<PriorityClass> bh1 = new BinomialHeap<PriorityClass>(new PriorityClass[] {
            new PriorityClass(1, 'a'),
            new PriorityClass(2, 'b'),
            new PriorityClass(3, 'c'),
            new PriorityClass(4, 'd'),
            new PriorityClass(5, 'e'),
            new PriorityClass(6, 'f'),
            new PriorityClass(7, 'g'),
            new PriorityClass(8, 'h'),
            new PriorityClass(9, 'i'),
            new PriorityClass(10, 'j'),
            new PriorityClass(11, 'k'),
            new PriorityClass(12, 'l'),
            new PriorityClass(13, 'm'),
            new PriorityClass(14, 'n'),
            new PriorityClass(15, 'o'),
            new PriorityClass(16, 'p'),
            new PriorityClass(17, 'q'),
            new PriorityClass(18, 'r'),
            new PriorityClass(19, 's'),
            new PriorityClass(20, 't'),
            new PriorityClass(21, 'u'),
            new PriorityClass(22, 'v'),
            new PriorityClass(23, 'w'),
            new PriorityClass(24, 'x'),
            new PriorityClass(25, 'y'),
            new PriorityClass(26, 'z'),
        });

        bh1.PrintDebug();

        Console.WriteLine("Popping from that 2 times (will coalesce after each one):");
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine("Will pop:");
            Console.WriteLine((bh1.Front?.ToString() ?? "[NULL FRONT]") + "\n");
            bh1.Remove();
            Console.WriteLine("After pop:");
            bh1.PrintDebug();
            Console.WriteLine("\n");
        }
    }
}