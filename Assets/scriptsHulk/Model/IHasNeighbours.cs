using System.Collections.Generic;
using System.Collections;

//interface that should be implemented by grid nodes used in E. Lippert's generic path finding implementation
public interface IHasNeighbours
{
    IEnumerable Neighbours { get; }
}
