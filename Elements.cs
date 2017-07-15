using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CBT
{
    class Elements
    {
        #region ctor
        public Elements()
        {
            doc = new XDocument(XDocument.Load(@"database.xml"));
            situations = new XElement(doc.Root);
            countElement = new XElement(situations.Element("Count"));
            actualCount = new XElement(countElement.Element("Count"));
        }

        #endregion

        #region Properties

        public XDocument doc { get; set; }
        public XElement situations { get; set; }
        public XElement countElement { get; set; }
        public XElement actualCount { get; set; }

        #endregion
    }
}