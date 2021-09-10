using CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCodeModel.Model
{
    public class XmlElementW : JAnnotationWriter<XmlElement>
    {
        string valueField;
        string nsField;
        public XmlElementW value(string s)
        {
            valueField = s;
            return this;
        }

        public XmlElementW ns(string s)
        {
            nsField = s;
            return this;
        }

        public JAnnotationUse AnnotationUse
        {
            get { throw new NotImplementedException(); }
        }

        public Type AnnotationType
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class XmlElement : Attribute
    {
    }
}
