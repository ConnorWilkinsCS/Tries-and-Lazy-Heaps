using System.Text;

public interface ITrie<T>
{
    bool Insert(string key, T value);
    T? Get(string key);
}


class Trie<T> : ITrie<T>
{
    protected class Node
    {
        public char ch;                   // Character of the key
        public T? value;                  // Value at Node; otherwise default
        public Node? low, middle, high;   // Left, middle, and right subtrees

        // Node
        // Creates an empty Node
        // All children are set to null
        // Time complexity:  O(1)

        public Node(char ch)
        {
            this.ch = ch;
            this.value = default(T);
            this.low = this.middle = this.high = null;
        }

        public bool IsDefault { get => this.value?.Equals(default(T)) ?? true; }
    }

    // =============================== Members and Properties ===============================

    protected Node? root;                              // Root node of the Trie

    public int Size { get; private set; }            // Number of values in the Trie
    public bool IsEmpty { get => root is null; }     // `true` if there is nothing in the Trie


    // =============================== Constructors ===============================

    // Trie
    // Creates an empty Trie
    // Time complexity:  O(1)

    public Trie()
    {
        this.root = null;
        this.Size = 0;
    }


    // =============================== Methods ===============================

    // Public Insert
    // Calls the private Insert which carries out the actual insertion
    // Returns true if successful; false otherwise

    public bool Insert(string key, T value) => Insert(ref this.root, key, 0, value);

    // Private Insert
    // Inserts the key/value pair into the Trie
    // Returns true if the insertion was successful; false otherwise
    // Note: Duplicate keys are ignored
    // Time complexity:  O(n+L) where n is the number of nodes and 
    //                                L is the length of the given key

    private bool Insert(ref Node? p, string key, int i, T value)
    {
        if (p == null)
            p = new Node(key[i]);

        // Current character of key inserted in left subtree
        if (key[i] < p.ch)
            return Insert(ref p.low, key, i, value);

        // Current character of key inserted in right subtree
        else if (key[i] > p.ch)
            return Insert(ref p.high, key, i, value);

        // Key found
        else if (i + 1 == key.Length)
        {
            // But key/value pair already exists
            if (!p.IsDefault)
            {
                return false;
            }
            else
            {
                // Place value in node
                p.value = value;
                this.Size += 1;
                return true;
            }
        }

        else
            // Next character of key inserted in middle subtree
            return Insert(ref p.middle, key, i + 1, value);
    }


    // Public Remove
    // Calls the private Remove which carries out the actual removal
    // Returns true if successful; false otherwise
    public bool Remove(string key)
    {
        if (this.root is null)
            return false;
        else
            return Remove(ref this.root, key, 0);
    }

    // Remove
    // Remove the given key (value) from the Trie
    // Returns true if the removal was successful; false otherwise 
    private bool Remove(ref Node? p, string key, int i)
    {

        // We hit the end without finding the terminal node to remove
        if (p is null || i >= key.Length)
            return false;

        bool result;

        // Follow the same path as Insert
        if (key[i] < p.ch)
        {
            result = Remove(ref p.low, key, i);
        }
        else if (key[i] > p.ch)
        {
            result = Remove(ref p.high, key, i);
        }
        else if (i + 1 == key.Length)
        {
            // If there's nothing here (we did not find a value on the final node of this key), then this key did not
            // have a value present (ie. deleting 'Sponge' when 'Spongebob' exists)
            if (p.IsDefault) result = false;
            else
            {
                // Remove the current node and return immediately. Since we are terminating here, we don't need to do
                // the children check later in the method
                p = null;
                this.Size -= 1;
                return true;
            }
        }
        else
        {
            result = Remove(ref p.middle, key, i + 1);
        }

        // If
        // - we just removed a child of this node (result == true),
        // - there's nothing stored in this node, *and*
        // - there's no children of this node that didn't get removed;
        // we may remove this node as well

        if (
            result &&
            p.IsDefault &&
            (p.low ?? p.middle ?? p.high) is null
        )
            p = null;

        return result;
    }


    // Value
    // Returns the value associated with a key; otherwise default
    // Time complexity:  O(d) where d is the depth of the trie

    public T? Get(string key)
    {
        int i = 0;
        Node? p = this.root;

        while (p != null)
        {
            // Search for current character of the key in left subtree
            if (key[i] < p.ch)
                p = p.low;

            // Search for current character of the key in right subtree           
            else if (key[i] > p.ch)
                p = p.high;

            else // if (p.ch == key[i])
            {
                // Return the value if all characters of the key have been visited 
                if (++i == key.Length)
                    return p.value;

                // Move to next character of the key in the middle subtree   
                p = p.middle;
            }
        }

        return default(T);   // Key too long
    }


    // Contains
    // Returns true if the given key is found in the Trie; false otherwise
    // Time complexity:  O(d) where d is the depth of the trie

    public bool Contains(string key)
    {
        int i = 0;
        Node? p = root;

        while (p != null)
        {
            // Search for current character of the key in left subtree
            if (key[i] < p.ch)
                p = p.low;

            // Search for current character of the key in right subtree           
            else if (key[i] > p.ch)
                p = p.high;

            else // if (p.ch == key[i])
            {
                // Return true if the key is associated with a non-default value; false otherwise 
                if (++i == key.Length)
                    return !p.IsDefault;

                // Move to next character of the key in the middle subtree   
                p = p.middle;
            }
        }
        return false;        // Key too long
    }


    // Public Print
    // Calls private Print to carry out the actual printing

    public void Print() => Print(root, "");

    // Private Print
    // Outputs the key/value pairs ordered by keys
    // Time complexity:  O(n) where n is the number of nodes

    private void Print(Node? p, string key)
    {
        if (p != null)
        {
            Print(p.low, key);
            if (!p.IsDefault)
                Console.WriteLine(key + p.ch + " " + p.value);
            Print(p.middle, key + p.ch);
            Print(p.high, key);
        }
    }


    // PrintDebug
    // Outputs a drawing of the underlying ternary tree in a readable format appropriate for troubleshooting.
    // Time complexity: TODO

    public void PrintDebug()
    {

        // Takes in 3 nodes and returns the string that would be used to branch between them
        string DetermineBranch(Node? h, Node? m, Node? l) => (h, m, l) switch
        {
            // Only one branch has a child
            (not null, null, null) => "─╯ ",
            (null, not null, null) => "───",
            (null, null, not null) => "─╮ ",
            // Two of the branches have a child
            (not null, not null, null) => "─┴─",
            (not null, null, not null) => "─┤ ",
            (null, not null, not null) => "─┬─",
            // All of the branches have a child
            (not null, not null, not null) => "─┼─",
            _ => throw new ArgumentException(),
        };

        // Takes an array of strings and appends them all to the given StringBuilder after transforming them according
        // to the given functions. The `aboveCount` and `belowCount` parameters are used to determine when to switch
        // between which transformer to use.
        void AppendAboveBelow(
            ref StringBuilder sb,
            IList<string> lines,
            int aboveCount,
            int belowCount,
            Func<string, string> hTransform,
            Func<string, string> mTransform,
            Func<string, string> lTransform
        )
        {
            int i;

            for (i = 0; i < aboveCount; i++)
                sb.AppendLine(hTransform(lines[i]));

            sb.AppendLine(mTransform(lines[i++]));

            for (int j = 0; j < belowCount; j++)
                sb.AppendLine(lTransform(lines[i + j]));
        }


        (string?, int, int) Recurse(Node? curr)
        {
            // ---------------- Generate recursive strings ----------------

            if (curr is null)
                return (null, 0, 0);

            // Generate own node
            string selfStr = String.Format("[{0}{1}]", curr.ch, curr.IsDefault ? "" : $" {curr.value.ToString()}");

            if (curr.high is null && curr.middle is null && curr.low is null)
                return (selfStr, 0, 0);

            // Generate high, middle, and low branches
            (string? h, int hAbove, int hBelow) = Recurse(curr.high);
            (string? m, int mAbove, int mBelow) = Recurse(curr.middle);
            (string? l, int lAbove, int lBelow) = Recurse(curr.low);

            // ---------------- Now we can glue them together ----------------

            StringBuilder sb = new StringBuilder();

            // Figure out the padding to the left of everything
            string selfPad = new string(' ', selfStr.Length);
            string branch = DetermineBranch(curr.high, curr.middle, curr.low);

            // ---------------- Append top ----------------
            if (h is not null)
            {
                AppendAboveBelow(
                    ref sb,
                    h.Split(Environment.NewLine),
                    hAbove,
                    hBelow,
                    line => $"{selfPad}   {line}",
                    line => $"{selfPad} ╭─{line}",
                    line => $"{selfPad} │ {line}"
                );
            }

            // ---------------- Append middle ----------------
            if (m is not null)
            {
                // high and low separators only need to be drawn in front of the high and low branches if there exist
                // nodes in that direction
                string hFront = h is not null ? " │ " : "   ";
                string lFront = l is not null ? " │ " : "   ";

                AppendAboveBelow(
                    ref sb,
                    m.Split(Environment.NewLine),
                    mAbove,
                    mBelow,
                    line => $"{selfPad}{hFront}{line}",
                    line => $"{selfStr}{branch}{line}",
                    line => $"{selfPad}{lFront}{line}"
                );
            }
            else
            {
                sb.AppendLine($"{selfStr}{branch}");
            }

            // ---------------- Append bottom ----------------
            if (l is not null)
            {
                AppendAboveBelow(
                    ref sb,
                    l.Split(Environment.NewLine),
                    lAbove,
                    lBelow,
                    line => $"{selfPad} │ {line}",
                    line => $"{selfPad} ╰─{line}",
                    line => $"{selfPad}   {line}"
                );
            }

            // Calculate the top and bottom aboveness
            return (
                sb.ToString(),
                hAbove + hBelow + mAbove + (h is not null ? 1 : 0),
                lBelow + lAbove + mBelow + (l is not null ? 1 : 0)
            );
        }

        (string? result, _, _) = Recurse(this.root);
        Console.WriteLine(result ?? "[null root]");
    }

}