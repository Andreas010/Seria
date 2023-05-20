using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andy010.Seria;

internal class IntTypeHandler : SeriaTypeHandler<int>
{
    public override int Deserialize(BinaryReader reader)
    {
        return reader.ReadInt32();
    }

    public override void Serialize(int value, BinaryWriter writer)
    {
        writer.Write(value);
    }
}
