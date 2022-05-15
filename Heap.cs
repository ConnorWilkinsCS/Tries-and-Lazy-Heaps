using System.Text;

// Binomial Heap
// Implementation:  Leftmost-child, right-sibling

public class BinomialHeap<T> where T : IComparable<T>
{

    class Node : IComparable<Node>
    {
        public T Item { get; set; }
        public int Degree { get; set; }
        public Node? LeftmostChild { get; set; }
        public Node? RightSibling { get; set; }

        // Constructor

        public Node(T item)
        {
            Item = item;
            Degree = 1; // degree includes self
            LeftmostChild = null;
            RightSibling = null;
        }

        public int CompareTo(Node? other)
        {
            if (other is null || other.Item is null) return 1;
            else return Item.CompareTo(other.Item);
        }


        private string? Truncate(string? str, int length) => str?.Length > length - 1
            ? string.Concat(str.AsSpan(0, length), "…")
            : str;

        public override string ToString()
        {
            return $"[{Item} ({Degree})]";
        }
    }


    private List<Node> rootList;


    // Size
    // Returns the total number of elements in the heap, across all binomial trees.
    // Time complexity:  O(1)
    public int Size { get; private set; }

    // IsEmpty
    // Returns true if there are no elements left in the heap.
    // Time complexity:  O(1)
    public bool IsEmpty { get => this.Size == 0; }

    // FrontNode
    // Returns the item with the highest priority. If the heap is empty, returns null (technically default(T)).
    // Time complexity:  O(1)
    private Node? FrontNode { get; set; }

    // Front
    // Calls private FrontNode to return the Node with the highest priority
    // Public Front then returns the item of the Node
    // Time complexity:  O(1)
    public T? Front
    {
        get
        {
            if (this.FrontNode is null) return default;
            else return this.FrontNode.Item;
        }
    }
    // Constructor
    // Time complexity:  O(1)

    public BinomialHeap()
    {
        this.rootList = new List<Node>();
        this.FrontNode = null;
        this.Size = 0;
    }

    public BinomialHeap(IEnumerable<T> items) : this()
    {
        foreach (var item in items) {
            this.Add(item);
        }

        this.Coalesce();
    }


    private int ComputeLog(int n) => (int)Math.Floor(Math.Log2(n));


    // Add
    // Inserts an item into the binomial heap
    // Time complexity:  O(1)

    public void Add(T item)
    {
        var newNode = new Node(item);
        this.rootList.Add(newNode);

        // Keep track of the always highest
        if (this.FrontNode is null || newNode.CompareTo(this.FrontNode) <= 0)
        {
            this.FrontNode = newNode;
        }

        this.Size++;
    }


    // Private Coalesce (`internal` for testing, to be able to call from Main)
    // This method merges the subtree list which is constructed 
    // in Remove(), prior to merging Coalesce checks for and removes
    // duplicates to organize the Max Heap in order
    // Time complexity:  O(log n)
    internal void Coalesce()
    {
        // =================== Local function declarations ===================

        void Merge(Node?[] list, Node root)
        {
            // Check if a node at the destination already exists (i.e. root is '2', and there's already a '2')
            var index = ComputeLog(root.Degree);
            var existing = list[index];

            if (existing is not null)
            {
                // Figure out which of the two is higher priority
                var (newRoot, otherRoot) = (root.CompareTo(existing) < 0) ? (root, existing) : (existing, root);

                // Merge trees
                var temp = newRoot.LeftmostChild;      // (numbers are from example Figma diagram):
                newRoot.LeftmostChild = otherRoot;     // <--- 1's left becomes 3
                otherRoot.RightSibling = temp;         // <--- 3's right becomes 2

                // we would need to keep track of degree here, otherwise the recurse will break...
                // I think this is how we'd do that? 
                newRoot.Degree += otherRoot.Degree;

                // newRoot now contains both trees that wanted to occupy the i'th spot in our "binary addition" array
                list[index] = null;     // free up the spot that the 'existing' was in
                Merge(list, newRoot);   // place them where they now belong, but recursively in case something is there
            }
            else
            {
                // if there isn't a '2' (for example) in our "binary addition array" already then this root can just
                // take the spot
                list[index] = root;
            }

            // Maintain FrontNode reference as we come into contact with every root node along the way
            if (FrontNode is null || root.CompareTo(FrontNode) <= 0)
                FrontNode = root;
        }

        // =================== Execute and return ===================

        int size = ComputeLog(Size) + 1;
        var coalesceArray = new Node?[size];

        foreach (var node in this.rootList)
        {
            Merge(coalesceArray, node);
        }

        // if the array looks like '10100' then we end up with just '11' in the rootlist
        this.rootList = coalesceArray.Where(node => node is not null).ToList()!;
    }


    // Remove
    // Removes the item with the highest priority from the binomial heap
    // Time complexity:  O(log n)

    public void Remove()
    {
        // find the highest priority node to be removed
        var toRemove = this.FrontNode;

        if (toRemove is null)
        {
            return;
        }
        else
        {
            // if node has degree of more than 1, it must have at least one left child
            if (toRemove.Degree > 1)
            {
                Node curr = toRemove.LeftmostChild!;

                // for the degree number k there are k subtrees to be split from root
                for (int i = 0; i < ComputeLog(toRemove.Degree); i++) // Loop through subtrees
                {
                    // add subtrees to rootList
                    var temp = curr.RightSibling!;

                    // Need to remove right sibling as it becomes a root to maintain the expectation that a root never
                    // has a right child
                    curr.RightSibling = null;
                    rootList.Add(curr);

                    curr = temp;
                }
            }

            // remove highest item now
            rootList.Remove(toRemove);
            this.FrontNode = null;

            // revalidate our currHighest
            Coalesce();
        }
    }

   
    // MakeEmpty
    // Creates an empty binomial heap
    // Time complexity:  O(1)

    public void MakeEmpty()
    {
        // Recreate the rootList
        this.rootList = new List<Node>();
        this.Size = 0;
    }


    private static string leftSpacer  = "   │";
    private static string firstSpacer = "    ";

    public void PrintDebug()
    {
        // =================== Local function declarations ===================

        string MakeNode(Node node, bool rightCheck = false)
        {
            var (l, r) = (node == this.FrontNode) ? ("{{", "}}") : ("[", "]");

            // The 'rootList' items should never have a right child: use a different bracket as a warning in the case
            // that they do
            if (rightCheck && node.RightSibling is not null)
                r = "!";

            return node.ToString().Replace("[", l).Replace("]", r);
        }

        string[] PrintSubtree(Node? curr)
        {
            if (curr is null)
                return new string[] { "" };

            string self = MakeNode(curr);

            // If I have no children, just return me directly
            if (curr.LeftmostChild is null && curr.RightSibling is null)
                return new string[] { self };

            // Generate the branches
            var leftSubtree = PrintSubtree(curr.LeftmostChild);  // left subtree goes to the right
            var rightSubtree = PrintSubtree(curr.RightSibling);  // right subtree goes above

            var sb = new List<string>();

            // Append the right subtree first, since it goes above
            foreach (var line in rightSubtree)
            {
                sb.Add(line);
            }

            // Append the leftSubtree
            string leftTreeSpacer = (curr.RightSibling is not null) ? " │ " : "   ";

            sb.Add(leftTreeSpacer); // one extra "blank" line
            for (int i = 0; i < leftSubtree.Length; i++)
            {
                string start = (i < leftSubtree.Length - 1)
                    ? leftTreeSpacer + new string(' ', Math.Max(0, self.Length - leftTreeSpacer.Length)) + "   "
                    : $"{self}───";

                sb.Add($"{start}{leftSubtree[i]}");
            }

            return sb.ToArray();
        }

        // =================== Execute iteratively ===================

        var sb = new StringBuilder();
        bool first = true; // we don't need the column for the first root node

        foreach (var node in this.rootList)
        {
            // Generate the subtree and the self-node
            string[] subtree = PrintSubtree(node.LeftmostChild);

            string self = MakeNode(node, true);
            int selfPad = self.Length;

            // Start with a spacer line (4 space, line)
            if (!first)
            {
                sb.Append(leftSpacer);
                sb.AppendLine();
            }

            // For every line of the subtree, add it + padding
            for (int i = 0; i < subtree.Length; i++)
            {
                // Append line-by-line with padding, with different behaviour for the last line
                string start;
                if (i < subtree.Length - 1)
                {
                    start = !first ? leftSpacer : firstSpacer;
                    start += new string(' ', Math.Max(0, self.Length - leftSpacer.Length) + 1) + "      ";
                }
                else
                {
                    start = $" {self}";
                    if (node.LeftmostChild is not null || node.RightSibling is not null)
                        start += "──────";
                }

                sb.AppendLine($"{start}{subtree[i]}");
            }

            if (first) first = false;
        }

        Console.WriteLine(sb.ToString());

    }

}

//--------------------------------------------------------------------------------------

// Used by class BinomialHeap<T>
// Implements IComparable and overrides ToString (from Object)

public class PriorityClass : IComparable<PriorityClass>
{
    private int priorityValue;
    private char letter;

    public PriorityClass(int priority, char letter)
    {
        this.letter = letter;
        priorityValue = priority;
    }

    public int CompareTo(PriorityClass? other)
    {
        if (other is null) return -1;
        else if (this.priorityValue > other.priorityValue) return -1;
        else if (this.priorityValue < other.priorityValue) return 1;
        else return 0;
    }

    public override string ToString()
    {
        return $"{letter} p={priorityValue}";
    }
}