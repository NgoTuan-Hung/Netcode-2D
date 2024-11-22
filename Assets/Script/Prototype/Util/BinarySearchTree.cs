using System;
using UnityEngine;

public class Node<T>
{
	public T Data;
	public Node<T> Left;
	public Node<T> Right;

	public Node(T data)
	{
		Data = data;
		Left = null;
		Right = null;
	}
}

/// <summary>
/// Binary Search Tree for any type implementing IComparable.
/// We search base on the identifier of the object which must be int.
/// You can implement other type of identifier if you want (float, string, etc.)
/// </summary>
/// <remarks>
/// <b>This shouldn't be used with duplicate key</b>
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinarySearchTree<T> where T : IComparable<T>
{
	private Node<T> root;
	internal Node<T> Root { get => root; set => root = value; }

	public BinarySearchTree()
	{
		root = null;
	}

	public void Insert(T data)
	{
		InsertRecursive(ref root, data);
	}

	private void InsertRecursive(ref Node<T> current, T data)
	{
		if (current == null)
		{
			current = new Node<T>(data);
			return;
		}

		if (data.CompareTo(current.Data) < 0)
		{
			InsertRecursive(ref current.Left, data);
		}
		else
		{
			InsertRecursive(ref current.Right, data);
		}
	}
	
	public T Search(T data)
	{
		return SearchRecursive(Root, data);
	}
	
	public T SearchRecursive(Node<T> current, T data)
	{
		if (current == null) return default;
		
		if (current.Data.CompareTo(data) == 0) return current.Data;
		else if (data.CompareTo(current.Data) < 0) return SearchRecursive(current.Left, data);
		else return SearchRecursive(current.Right, data);
	}

	/// <summary>
	/// Special case where we don't know what the data is, only the key
	/// and we cannot create an empty object with the key (MonoBehaviour).
	/// You can specify a function to return the key from current node.
	/// </summary>
	/// <param name="getKey"></param>
	/// <param name="key"></param>
	/// <returns></returns>
	public T Search(Func<T, int> getKey, int key)
	{
		return SearchRecursive(root, getKey, key);
	}

	/// <summary>
	/// Recursive method to search for a node based on specific attribute
	/// </summary>
	/// <param name="current"></param>
	/// <param name="getKey"></param>
	/// <param name="key"></param>
	/// <returns></returns>
	private T SearchRecursive(Node<T> current, Func<T, int> getKey, int key)
	{
		if (current == null) return default;

		if (getKey(current.Data) == key)
		{
			return current.Data;
		}
		else if (key < getKey(current.Data))
		{
			return SearchRecursive(current.Left, getKey, key);
		}
		else
		{
			return SearchRecursive(current.Right, getKey, key);
		}
	}

	public void Travel(Action<T> action)
	{
		TravelRecursive(root, action);
	}
	private void TravelRecursive(Node<T> current, Action<T> action)
	{
		if (current != null)
		{
			action(current.Data);
			TravelRecursive(current.Left, action);
			TravelRecursive(current.Right, action);
		}
	}
}