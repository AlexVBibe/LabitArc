using System;

namespace Labit.Interfaces
{
    public interface IClosable
    {
        Action<bool> OnClose { get; set; }
    }
}
