﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Grean.AtomEventStore
{
    public class AtomInMemory
    {
        private readonly Dictionary<UuidIri, StringBuilder> entries;

        public AtomInMemory()
        {
            this.entries = new Dictionary<UuidIri, StringBuilder>();
        }

        public XmlWriter CreateWriterFor(AtomEntry atomEntry)
        {
            var id = GetIdFrom(atomEntry.Links);
            if (this.entries.ContainsKey(id))
                throw new InvalidOperationException(
                    string.Format(
                        "Will not create a new XmlWriter for the supplied AtomEntry, because a an AtomEntry with the ID {0} was already written.",
                        id.ToString()));

            var sb = new StringBuilder();            
            this.entries.Add(id, sb);
            return XmlWriter.Create(sb);
        }

        public XmlReader CreateReaderFor(UuidIri id)
        {
            var sr = new StringReader(this.entries[id].ToString());
            return XmlReader.Create(
                sr,
                new XmlReaderSettings { CloseInput = true });
        }

        private static UuidIri GetIdFrom(IEnumerable<AtomLink> links)
        {
            var selfLink = links.Single(l => l.IsSelfLink);
            return new Guid(selfLink.Href.ToString());
        }
    }
}
