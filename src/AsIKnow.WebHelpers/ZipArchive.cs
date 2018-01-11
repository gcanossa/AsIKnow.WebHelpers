using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.IO.Compression;

namespace AsIKnow.WebHelpers
{
    public class ZipArchive : IArchive
    {
        protected MemoryStream _ms;
        protected System.IO.Compression.ZipArchive _archive;

        public ZipArchive() : this(null)
        {}

        public ZipArchive(byte[] archive)
        {
            if (archive == null)
                _ms = new MemoryStream();
            else
            {
                _ms = new MemoryStream();
                _ms.Write(archive, 0, archive.Length);
            }

            _archive = new System.IO.Compression.ZipArchive(_ms, ZipArchiveMode.Update, true);
        }
        public void AddOrUpdateItem(string name, byte[] data)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            ZipArchiveEntry entry;
            if (!ItemExists(name))
                entry = _archive.CreateEntry(name);
            else
                entry = _archive.GetEntry(name);

            using (Stream wr = entry.Open())
            {
                wr.Write(data, 0, data.Length);
            }
        }

        public void Dispose()
        {
            _archive.Dispose();
            _ms.Dispose();
        }

        public byte[] GetItem(string name)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            ZipArchiveEntry entry;
            if (!ItemExists(name))
                throw new ArgumentOutOfRangeException(nameof(name), "Item not found.");
            else
                entry = _archive.GetEntry(name);

            using (MemoryStream ms = new MemoryStream())
            using (Stream rs = entry.Open())
            {
                rs.CopyTo(ms);
                ms.Flush();

                return ms.ToArray();
            }
        }

        public bool ItemExists(string name)
        {
            return _archive.GetEntry(name) != null;
        }

        public IEnumerable<string> ListItems()
        {
            return _archive.Entries.Select(p=>p.FullName);
        }

        public void RemoveItem(string name)
        {
            if (ItemExists(name))
                _archive.GetEntry(name).Delete();
        }

        public byte[] ToArray()
        {
            _archive.Dispose();
            _ms.Flush();
            return _ms.ToArray();
        }
    }
}
