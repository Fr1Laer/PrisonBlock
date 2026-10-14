using System;
using System.Collections.Generic;
using System.Text;

namespace PrisonBlock.Core;

public interface IRepository<T> where T : Prisoner
{
    void Add(T item);
    bool Remove(T item);
    IEnumerable<T> GetAll();
    T? Find(Func<T, bool> predicate);
}
