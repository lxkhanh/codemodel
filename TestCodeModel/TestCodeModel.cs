using CodeModel;
using CodeModel.writer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestCodeModel.Model;
using TestCodeModel.Util;

namespace TestCodeModel
{
    [TestFixture]
    public class TestCodeModel
    {
        private string baseDirectory;
        [SetUp]
        public void Init()
        {
            baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        }

        [Test]
        public void AnnotationSample()
        {
            string expectResult = File.ReadAllText(baseDirectory+"../../expectOutput/AnnotationSample.java");
            
            JCodeModel cm = new JCodeModel();
            JDefinedClass cls = cm._class("Test");
            JMethod m = cls.method(JMod.PUBLIC, cm.VOID, "foo");
            m.annotate(typeof(java.lang.Deprecated));

            JFieldVar field = cls.field(JMod.PRIVATE, cm.DOUBLE, "y");
            field.annotate(typeof(java.lang.Deprecated));
            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/AnnotationSample.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/AnnotationSample.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void AnnotationUseTest()
        {
            JCodeModel cm = new JCodeModel();
            JDefinedClass cls = cm._class("Test");
            // JMethod m =
            cls.method(JMod.PUBLIC, cm.VOID, "foo");

            // Annotating a class
            // Using the existing Annotations from java.lang.annotation package
            JAnnotationUse use = cls.annotate(cm.@ref(typeof(java.lang.annotation.Retention)));

            // declaring an enum class and an enumconstant as a membervaluepair
            JDefinedClass enumcls = cls._enum("Iamenum");
            JEnumConstant ec = enumcls.enumConstant("GOOD");
            JEnumConstant ec1 = enumcls.enumConstant("BAD");
            JEnumConstant ec2 = enumcls.enumConstant("BAD");
            ec1.Equals(ec2);

            use.param("value", ec);
            // adding another param as an enum
            use.param("value1", RetentionPolicy.RUNTIME);

            // Adding annotation for fields
            // will generate like
            // @String(name = "book") private double y;
            //
            JFieldVar field = cls.field(JMod.PRIVATE, cm.DOUBLE, "y");

            // Adding more annotations which are member value pairs
            JAnnotationUse ause = field.annotate(typeof(java.lang.annotation.Retention));
            ause.param("name", "book");
            ause.param("targetNamespace", 5);

            // Adding arrays as member value pairs
            JAnnotationArrayMember arrayMember = ause.paramArray("names");
            arrayMember.param("Bob");
            arrayMember.param("Rob");
            arrayMember.param("Ted");

            JAnnotationArrayMember arrayMember1 = ause.paramArray("namesno");
            arrayMember1.param(4);
            arrayMember1.param(5);
            arrayMember1.param(6);

            JAnnotationArrayMember arrayMember2 = ause.paramArray("values");
            // adding an annotation as a member value pair
            arrayMember2.annotate(typeof(Target)).param("type", typeof(int));
            arrayMember2.annotate(typeof(Target)).param("type", typeof(float));

            // test typed annotation writer
            //XmlElementW tt = new XmlElementW();
            //XmlElementW w = cls.annotate2(tt);
            //w.ns("##default").value("foobar");

            // adding an annotation as a member value pair
            JAnnotationUse myuse = ause.annotationParam("foo", typeof(Target));
            myuse.param("junk", 7);

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/AnnotationUseTest.java", FileMode.Create)));
        }

        [Test]
        public void AnonymousClassTest()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/AnonymousClassTest.java");

            JCodeModel cm = new JCodeModel();
            JDefinedClass cls = cm._class("Test");
            JMethod m = cls.method(JMod.PUBLIC, cm.VOID, "foo");

            JDefinedClass c = cm.anonymousClass(cm.@ref(typeof(Iterator)));
            c.method(0, cm.VOID, "bob");
            c.field(JMod.PRIVATE, cm.DOUBLE, "y");
            m.body().decl(cm.@ref(typeof(object)), "x", JExpr._new(c));

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/AnonymousClassTest.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/AnonymousClassTest.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void ForEachTest()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/ForEachTest.java");

            JCodeModel cm = new JCodeModel();
            JDefinedClass cls = cm._class("Test");

            JMethod m = cls.method(JMod.PUBLIC, cm.VOID, "foo");
            m.body().decl(cm.INT, "getCount");

            // This is not exactly right because we need to
            // support generics
            JClass arrayListclass = cm.@ref(typeof(ArrayList));
            JVar list = m.body().decl(arrayListclass, "alist", JExpr._new(arrayListclass));

            JClass integerclass = cm.@ref(typeof(java.lang.Integer));
            JForEach @foreach = m.body().forEach(integerclass, "count", list);
            JVar count1 = @foreach.var();
            @foreach.body().assign(JExpr.@ref("getCount"), JExpr.lit(10));

            // printing out the variable
            JFieldRef out1 = cm.@ref(typeof(Model.System)).staticRef("out");
            // JInvocation invocation =
            @foreach.body().invoke(out1, "println").arg(count1);

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/ForEachTest.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/ForEachTest.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void InnerClassTest()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/InnerClassTest.java");

            JCodeModel codeModel = new JCodeModel();
            JDefinedClass aClass = codeModel._class("org.test.DaTestClass");

            JDefinedClass otherClass = codeModel._class("org.test.OtherClass");
            otherClass.method(JMod.PUBLIC, aClass, "getOuter");
            File.WriteAllText(baseDirectory + "../../output/InnerClassTest.java", CodeModelTestsUtils.declare(otherClass));

            string result = File.ReadAllText(baseDirectory + "../../output/InnerClassTest.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void JAnnotationUseTest()
        {
            JCodeModel codeModel = new JCodeModel();
            JDefinedClass testClass = codeModel._class("Test");
            JAnnotationUse suppressWarningAnnotation = testClass.annotate(typeof(java.lang.SuppressWarnings));
            suppressWarningAnnotation.param("value", JExpr.lit("unused"));

            Assert.AreEqual("@java.lang.SuppressWarnings(\"unused\")", CodeModelTestsUtils.generate(suppressWarningAnnotation));
        }

        [Test]
        public void JCodeModelTest()
        {
            JCodeModel cm = new JCodeModel();
            cm.parseType("java.util.ArrayList<java.lang.String[]>[]");
        }

        [Test]
        public void JExprTestLitDouble()
        {
            Assert.IsTrue(CodeModelTestsUtils.toString(
                JExpr.lit(java.lang.Double.POSITIVE_INFINITY)).EndsWith(
                "POSITIVE_INFINITY"));
            Assert.IsTrue(CodeModelTestsUtils.toString(
                    JExpr.lit(java.lang.Double.NEGATIVE_INFINITY)).EndsWith(
                    "NEGATIVE_INFINITY"));
            Assert.IsTrue(CodeModelTestsUtils.toString(JExpr.lit(java.lang.Double.NaN))
                    .EndsWith("NaN"));
        }

        [Test]
        public void JExprTestLitFloat()
        {
            Assert.IsTrue(CodeModelTestsUtils.toString(
                JExpr.lit(java.lang.Float.POSITIVE_INFINITY)).EndsWith(
                "POSITIVE_INFINITY"));
            Assert.IsTrue(CodeModelTestsUtils.toString(
                    JExpr.lit(java.lang.Float.NEGATIVE_INFINITY)).EndsWith(
                    "NEGATIVE_INFINITY"));
            Assert.IsTrue(CodeModelTestsUtils.toString(JExpr.lit(java.lang.Float.NaN))
                    .EndsWith("NaN"));
        }

        [Test]
        public void JMethodTest()
        {
            JCodeModel cm = new JCodeModel();
		    JDefinedClass cls = cm._class("Test");
		    JMethod m = cls.method(JMod.PUBLIC, cm.VOID, "foo");

		    JVar foo = m.param(typeof(String), "foo");

		    Assert.AreEqual(1, m.@params().Count);
		    Assert.AreSame(foo, m.@params()[0]);
        }

        [Test]
        public void ExtendsClassTest()
        {
            JCodeModel cm = new JCodeModel();
		    JDefinedClass c = cm._package("foo")._class(0, "Foo");
		    c._extends(cm.@ref(typeof(Bar)));
		    cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/ExtendsClassTest.java", FileMode.Create)));
        }

        [Test]
        public void PackageAnnotationTest()
        {
            JCodeModel cm = new JCodeModel();
		    cm._package("foo").annotate(typeof(java.lang.annotation.Inherited));
            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory+ "../../output/PackageAnnotationTest.java", FileMode.Create)));
        }

        public void PackageJavadocTest()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/PackageJavadocTest.java");

            JCodeModel cm = new JCodeModel();
            cm._package("foo").javadoc().add("String");
            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/PackageJavadocTest.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/PackageJavadocTest.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void VarArgsTest()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/VarArgsTest.java");

            JCodeModel cm = new JCodeModel();
            JDefinedClass cls = cm._class("Test");
            JMethod m = cls.method(JMod.PUBLIC, cm.VOID, "foo");
            m.param(typeof(java.lang.String), "param1");
            m.param(typeof(java.lang.Integer), "param2");
            JVar var = m.varParam(typeof(java.lang.Object), "param3");
            
            // checking for param after varParam it behaves ok
            //JVar[] var1 = m.varParam(Float.class, "param4");
            JClass stringRef = cm.@ref(typeof(java.lang.String));
            JClass stringArray = stringRef.array();
            //JVar param5 =
            m.param(typeof(java.lang.String), "param5");
            
            JForLoop forloop = m.body()._for();
            
            JVar count = forloop.init(cm.INT, "count", JExpr.lit(0));
            
            forloop.test(count.lt(JExpr.direct("param3.length")));
            forloop.update(count.incr());
            
            JFieldRef outSystem = cm.@ref(typeof(java.lang.System)).staticRef("out");
            
            //JVar typearray = 
            m.listVarParam();
            
            //JInvocation invocation =
            forloop.body().invoke(outSystem, "println").arg(
                    JExpr.direct("param3[count]"));
            
            JMethod main = cls.method(JMod.PUBLIC | JMod.STATIC, cm.VOID, "main");
            main.param(stringArray, "args");
            main.body().directStatement("new Test().foo(new String(\"Param1\"),new Integer(5),null,new String(\"Param3\"),new String(\"Param4\"));" );//new String("Param1"))"");//                "new Integer(5),+//                "null," +//                "new String("first")," +//                " new String("Second"))");

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/VarArgsTest.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/VarArgsTest.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void GenerateClass1()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/GenerateClass1.java");
            JCodeModel cm = new JCodeModel();

            JDefinedClass  jDefinedClass = cm._class("ProjectEntry");
            JMethod jInitMethod = jDefinedClass.method(JMod.PUBLIC | JMod.STATIC, cm.VOID, "init");
            JMethod jDisposeMethod = jDefinedClass.method(JMod.PUBLIC | JMod.STATIC, cm.VOID, "dispose");
            for (int i = 0; i < 2; i++ )
            {
                string info = "Package" + i + ".Doc" + (i + 1);
                jInitMethod.body().staticInvoke(cm.directClass(info), "init");
                jDisposeMethod.body().staticInvoke(cm.directClass(info), "dispose");
            }

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/GenerateClass1.java", FileMode.Create)));

            string result = File.ReadAllText(baseDirectory + "../../output/GenerateClass1.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void GenerateClass2()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/GenerateClass2.java");
            JCodeModel cm = new JCodeModel();

            JDefinedClass jDefinedClass = cm._class("Doc_NewDocument1");          
            // method init
            JMethod jInitMethod = jDefinedClass.method(JMod.PUBLIC | JMod.STATIC, typeof(void), "init");
            jDefinedClass.field(JMod.PRIVATE | JMod.STATIC, typeof(bool), "flag");
            jInitMethod.body().assign(JExpr.@ref("flag"), JExpr.FALSE);
            CodeModelTestsUtils.GenerateInitMethod(cm, jInitMethod);

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/GenerateClass2.java", FileMode.Create)));
            string result = File.ReadAllText(baseDirectory + "../../output/GenerateClass2.java");
            Assert.AreEqual(expectResult, result);
        }

        [Test]
        public void GenerateClass3()
        {
            string expectResult = File.ReadAllText(baseDirectory + "../../expectOutput/GenerateClass3.java");
            JCodeModel cm = new JCodeModel();

            JDefinedClass jDefinedClass = cm._class("Doc_NewDocument1");
            // method init
            JMethod jInitMethod = jDefinedClass.method(JMod.PUBLIC | JMod.STATIC, typeof(void), "proc_main");
            jInitMethod._throws(cm.directClass("java.lang.Exception"));
            JVar jvarName = jInitMethod.body().decl(cm.@ref(typeof(java.lang.String)), "name", JExpr.lit("Company"));
            JConditional jcondition = jInitMethod.body()._if(JExpr.@ref("RuntimeEnvironment").invoke("getInstance").invoke("isNotTerminated"));
            
            JTryBlock tryBlock = jcondition._then()._try();
            JInvocation invocationTry = tryBlock.body().staticInvoke(cm.directClass("emotive.otf.runtime.functions.LoggingLib"), "WriteOtxIdIfNeeded");
            invocationTry.arg("OtxProject1@NewOtxProject1Package1@NewDocument1@main");
            invocationTry.arg("Activity Assignment_baf5f1f814e04d9dadc03682083dd145 (Assignment1) will be executed");
            tryBlock.body().assign(jvarName, JExpr.lit("Emotice ltd"));

            JCatchBlock catchBlock = tryBlock._catch(cm.directClass("java.lang.Exception"));
            catchBlock.param("ex");
            catchBlock.body().assign(jvarName, JExpr.lit(""));

            tryBlock._finally();

            JBlock elseBlock = jcondition._else();
            elseBlock.assign(jvarName, JExpr.lit("Emotive"));
            JVar max = elseBlock.decl(cm.INT, "max");
            JVar array = elseBlock.decl(cm._ref(typeof(int[])), "a", JExpr.newArray(cm.INT).add(JExpr.lit(1)).add(JExpr.lit(9)).add(JExpr.lit(10)));
            elseBlock.assign(max, JExpr.component(array, JExpr.lit(0)));
            JForLoop forLoop = elseBlock._for();
            JVar initVar = forLoop.init(cm.INT, "i", JExpr.lit(1));
            forLoop.test(initVar.lt(array.@ref("length")));
            forLoop.update(initVar.incr());
            forLoop.body()._if(array.component(initVar).gt(max))._then()
                .assign(max, array.component(initVar));

            cm.build(new SingleStreamCodeWriter(new FileStream(baseDirectory + "../../output/GenerateClass3.java", FileMode.Create)));
            string result = File.ReadAllText(baseDirectory + "../../output/GenerateClass3.java");
            Assert.AreEqual(expectResult, result);
        }
    }
}
