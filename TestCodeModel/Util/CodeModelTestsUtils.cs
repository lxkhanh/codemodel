using CodeModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestCodeModel.Util
{
    public class CodeModelTestsUtils
    {
        public const string JArrayListClass = "java.util.ArrayList";
        public const string JStringClass = "java.lang.String";
        public const string CoreLibClass = "emotive.otf.runtime.functions.CoreLib";
        public const string DelegateClass = "emotive.otf.runtime.functions.Delegate";
        public const string Delegate = "delegate_";
        public const string ProcedurePre = "proc_";
        public const string VisibilityClass = "emotive.otf.runtime.functions.Visibility";

        public static String toString(JExpression expression)
        {
            if (expression == null)
            {
                throw new Exception();
            }
            StringWriter stringWriter = new StringWriter();
            JFormatter formatter = new JFormatter(stringWriter);
            expression.generate(formatter);
            return stringWriter.ToString();
        }

        public static String declare(JDeclaration declaration)
        {
            if (declaration == null)
            {
                throw new Exception("Declaration must not be null.");
            }
            StringWriter stringWriter = new StringWriter();
            JFormatter formatter = new JFormatter(stringWriter);
            declaration.declare(formatter);
            return stringWriter.ToString();
        }

        public static String generate(JGenerable generable)
        {
            if (generable == null)
            {
                throw new Exception("Generable must not be null.");
            }
            StringWriter stringWriter = new StringWriter();
            JFormatter formatter = new JFormatter(stringWriter);
            generable.generate(formatter);
            return stringWriter.ToString();
        }


        public static void GenerateInitMethod(JCodeModel cm, JMethod jInitMethod)
        {
            string documentName = "Document";
            string packageName = "Package";
            string projectName = "Project";
            string referencesVariable = "adfgsd5874k";
            jInitMethod.body().decl(cm.directClass(JArrayListClass).narrow(cm.directClass(JStringClass)), referencesVariable, JExpr._new(cm.directClass(JArrayListClass).narrow(cm.directClass(JStringClass))));
            for (int i = 0; i < 2; i++ )
            {
                jInitMethod.body().add(
                    JExpr.@ref(referencesVariable).invoke("add").arg(JExpr.lit("Package" + i + "." + "Document" + i)));
            }

            jInitMethod.body().add(
                cm.directClass(CoreLibClass).staticInvoke("documentReferenceCache").arg(
                    JExpr.lit(packageName + "." + documentName)).arg(JExpr.@ref(referencesVariable)));
            for (int i = 0; i < 2; i++ )
            {
                string proName = "pro" + i;
                string validityName = "validityName" + i;
                string procedureName = packageName + "." + documentName + "." + proName;
                GenerateProcedureCache(jInitMethod, Delegate + proName, procedureName,"OtxName", "PackageName",  ProcedurePre + proName, projectName, "PUBLIC", validityName);
            }

            for (int i = 0; i < 2; i++ )
            {
                JCodeModel jCodeModel1 = new JCodeModel();
                string signatureName = "Package.Document" + i;
                JInvocation prosigCacheInvocation = jInitMethod.body().staticInvoke(jCodeModel1.directClass(CoreLibClass), "ProcedureSignatureCache");
                JDefinedClass visibilityEnum = jCodeModel1._class(VisibilityClass);
                prosigCacheInvocation.arg(signatureName);
                prosigCacheInvocation.arg(visibilityEnum.enumConstant("PUBLIC"));
            }
        }

        private static void GenerateProcedureCache(JMethod jInitMethod, string signatureDelegateReference, string signatureFullName, string otxName, string otxPackage, string procedureName, string projectName, string procedureVisibility, string validityName)
        {
            JCodeModel jCodeModel = new JCodeModel();
            JInvocation procedureCacheInvocation = jInitMethod.body().staticInvoke(jCodeModel.directClass(CoreLibClass), "procedureCache");
            procedureCacheInvocation.arg(signatureFullName);
            JType delegateType = jCodeModel.parseType(DelegateClass);
            procedureCacheInvocation.arg(JExpr.cast(delegateType, JExpr.direct(signatureDelegateReference).invoke("build").arg(jCodeModel._class("DocumentReference").dotclass()).arg(procedureName)));
            procedureCacheInvocation.arg(otxName);
            procedureCacheInvocation.arg(otxPackage);
            procedureCacheInvocation.arg(projectName);
            JDefinedClass visibilityEnum = jCodeModel._class(VisibilityClass);
            procedureCacheInvocation.arg(visibilityEnum.enumConstant(procedureVisibility));
            procedureCacheInvocation.arg(validityName);
        }
    }
}
