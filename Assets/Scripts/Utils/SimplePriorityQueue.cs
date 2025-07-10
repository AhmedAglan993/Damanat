using System.Collections.Generic;

public class SimplePriorityQueue<T>
{
    private List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public T Dequeue()
    {
        T item = elements[0].item;
        elements.RemoveAt(0);
        return item;
    }

    public bool Contains(T item)
    {
        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}
