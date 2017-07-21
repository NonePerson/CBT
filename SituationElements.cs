using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CBT
{
    class SituationElements
    {
        #region ctor
        public SituationElements()
        {
            doc = new XDocument(XDocument.Load(@"database.xml"));
            situations = new XElement(doc.Root);
            allSituations = new List<XElement>(situations.Elements("Situation"));
            countElement = new XElement(situations.Element("Count"));
        }

        #endregion

        #region Properties

        public XDocument doc { get; set; }
        public XElement situations { get; set; }
        public List<XElement> allSituations { get; set; }
        public XElement thoughts { get; set; }
        public XElement feelings { get; set; }
        public XElement behaviours { get; set; }
        public XElement countElement { get; set; }

        #endregion
    }
}
