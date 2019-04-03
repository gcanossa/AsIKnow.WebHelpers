using System;
using System.Collections.Generic;
using System.Text;

namespace AsIKnow.WebHelpers.Data
{
    public interface IArchive : IDisposable
    {
        void AddOrUpdateItem(string name, byte[] data);
        bool ItemExists(string name);
        byte[] GetItem(string name);
        void RemoveItem(string name);
        IEnumerable<string> ListItems();

        byte[] ToArray();
    }
}
