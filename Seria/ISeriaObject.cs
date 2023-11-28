using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seria;

public interface ISeriaObject
{
    public void Serialize(SeriaBuffer buffer);
}
