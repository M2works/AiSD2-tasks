
using System.Collections.Generic;

namespace ASD
{

public interface IList : IEnumerable<int>
    {
    // Je�li element v jest na li�cie to zwraca true
    // Je�li elementu v nie ma na li�cie to zwraca false
    bool Search(int v);

    // Je�li element v jest na li�cie to zwraca false (elementu nie dodaje)
    // Je�li elementu v nie ma na li�cie to dodaje go do listy i zwraca true
    bool Add(int v);

    // Je�li element v jest na li�cie to usuwa go z listy i zwraca true
    // Je�li elementu v nie ma na li�cie to zwraca false
    bool Remove(int v);
    }

//
// dopisa� klas� opisuj�c� element listy
//
public class node
    {
        public node(int v)
        {
            value = v;
            next = null;
        }
        public int value;
        public node next;
    }

// Zwyk�a lista (nie samoorganizuj�ca si�)
public class SimpleList : IList
    {
        // doda� niezb�dne pola
        private node head, tail;
        public SimpleList()
        {
            head = tail = null;
        }
    // Lista si� nie zmienia
    public bool Search(int v)
        {
            if (head == null) return false;
            node p = head;
            while(p!=null)
            {
                if (p.value == v) return true;
                p = p.next;
            }
            return false;
        }

    // Element jest dodawany na koniec listy
    public bool Add(int v)
        {
            if (Search(v)) return false;
            node p = new node(v);
            if(head==null)
            {
                head = tail = p;
                return true;
            }
            tail.next = p;
            tail = p;
            return true;
        }

    // Pozosta�e elementy nie zmieniaj� kolejno�ci
    public bool Remove(int v)
        {
            if (!Search(v)) return false;
            node p = head;
            if(p.value==v)
            {
                head = head.next;
                return true;
            }
            while (p.next.value != v)
                p = p.next;
            node q = p.next;
            p.next = q.next;
            if (q == tail) tail = p;
            return true;
        }

    // Wymagane przez interfejs IEnumerable<int>
    public IEnumerator<int> GetEnumerator()
        {
            for(node p = head;p!= null;p=p.next)
                yield return p.value;
        }

    // Wymagane przez interfejs IEnumerable<int> - nie zmmienia� (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class SimpleList


// Lista z przesnoszeniem elementu, do kt�rego by� dost�p na pocz�tek
public class MoveToFrontList : IList
    {

        // doda� niezb�dne pola
        private node head;
        public MoveToFrontList()
        {
            head = null;
        }
    // Znaleziony element jest przenoszony na pocz�tek
    public bool Search(int v)
        {
            if (head == null) return false;
            node p = head;
            if (p.value == v)
                return true;
            while (p.next != null && p.next.value!=v)
                p = p.next;
            if (p.next == null) return false;
            node q = p.next;
            p.next = q.next;
            q.next = head;
            head = q;
            return true;
        }

    // Element jest dodawany na pocz�tku, a je�li ju� by� na li�cie to jest przenoszony na pocz�tek
    public bool Add(int v)
        {
            if (Search(v)) return false;
           
            node p = new node(v);
            if(head==null)
            {
                head = p;
                return true;
            }
            p.next = head;
            head = p;
            return true;
        }

    // Pozosta�e elementy nie zmieniaj� kolejno�ci
    public bool Remove(int v)
        {
            if (!Search(v)) return false;
            head = head.next;
            return true;
        }

    // Wymagane przez interfejs IEnumerable<int>
    public IEnumerator<int> GetEnumerator()
        {
            for (node p = head; p != null; p = p.next)
                yield return p.value;
        }

    // Wymagane przez interfejs IEnumerable<int> - nie zmmienia� (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class MoveToFrontList


} // namespace ASD
