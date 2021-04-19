using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Node _next;
    private Queue<Vector2> _pastPos;
    private int _maxLength;
    private SpriteRenderer _spRenderer;

    public Node Next
    {
        get { return _next; }
        set { _next = value; }
    }

    public int MaxLength
    {
        set { _maxLength = value; }
    }

    private void Start()
    {
        _next = null;
        _pastPos = new Queue<Vector2>();
        _spRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdatePastPos(Vector2 pos)
    {
        transform.position = pos;
        _pastPos.Enqueue(pos);
        if (_pastPos.Count < _maxLength)
        {
            return;
        }
        else
        {
            Vector2 removedPos = _pastPos.Dequeue();
            if(_next != null)
            {
                _next.UpdatePastPos(removedPos);
            }
        }
    }

    public Vector2 GetLastPos()
    {
        return _pastPos.Peek();
    }

    public void SetSortOrder(int order)
    {
        if(order >= 2)
        {
            if (_spRenderer != null)
            {
                _spRenderer.sortingOrder = order;
            }
            if (_next != null)
            {
                _next.SetSortOrder(order - 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Door"))
        {
            _spRenderer.sortingOrder = -1;
            if(_next == null)
            {
                Head.Instance.AllStopped();
            }
        }

        if(coll.CompareTag("EnemyBullet"))
        {
            Bullet b = coll.GetComponent<Bullet>();
            if(b.IsCollided)
            {
                return;
            }else
            {
                b.IsCollided = true;
            }
            int numToRemove = b.bulletDamage;
            Head.Instance.RemoveNode(numToRemove);
            Destroy(coll.gameObject);
        }
    }

    public void Die()
    {
        if(_next != null)
        {
            _next.Die();
            _next = null;
        } 
        Destroy(gameObject);
    }
}
